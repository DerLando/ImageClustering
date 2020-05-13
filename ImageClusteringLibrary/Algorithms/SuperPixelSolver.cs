using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageClusteringLibrary.Data;
using ImageClusteringLibrary.Data.Collections;
using ImageClusteringLibrary.IO;

namespace ImageClusteringLibrary.Algorithms
{
    /// <summary>
    /// Solver that calculates superpixel clustering from images
    /// </summary>
    public static class SuperPixelSolver
    {
        /// <summary>
        /// Calculate a normalized distance between two <see cref="PixelLabxy"/>
        /// </summary>
        /// <param name="a">first pixel</param>
        /// <param name="b">second pixel</param>
        /// <param name="m">compactness of super pixel</param>
        /// <param name="S">grid interval</param>
        /// <returns></returns>
        private static double PixelDistance(in PixelLabxy a, in PixelLabxy b, double m, double S)
        {
            var d_lab = Math.Sqrt((a.Lab.L - b.Lab.L) * (a.Lab.L - b.Lab.L) +
                                  (a.Lab.a - a.Lab.a) * (a.Lab.a - a.Lab.a) +
                                  (a.Lab.b - a.Lab.b) * (a.Lab.b - a.Lab.b));
            var d_xy = a.Position.DistanceTo(b.Position);

            return d_lab + (m / S) * d_xy;
        }

        /// <summary>
        /// Calculates the norm of a color in CIELAB space
        /// this is basically the length of the l,a,b vector
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static double ColorNorm(in ColorCielab c)
        {
            return Math.Sqrt(c.L * c.L + c.a * c.a + c.b * c.b);
        }

        /// <summary>
        /// Compute the gradient value of the pixel at the given x,y coordinates
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static double PixelGradient(Bitmap bitmap, int x, int y)
        {
            var n1 = ColorNorm(bitmap.GetCielabColor(x + 1, y) - bitmap.GetCielabColor(x - 1, y));
            var n2 = ColorNorm(bitmap.GetCielabColor(x, y + 1) - bitmap.GetCielabColor(x, y - 1));

            return n1 * n1 + n2 * n2;
        }

        /// <summary>
        /// Gets the position with the smallest gradient in a 3x3 neighborhood
        /// for the given position
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static Position GetSmallestGradient(Bitmap bitmap, in Position position)
        {
            // best fit
            Position result = position;
            var smallestGradient = 1000000.0;

            // check 3x3 neighbours
            var grid = PositionHelper.GetNeighboringPositions(position, 3);
            foreach (var gridPosition in grid)
            {
                // check if bad grid position
                if (position.Vector.X < 0 | position.Vector.Y < 0) continue;

                var gradient = PixelGradient(bitmap, gridPosition.Vector.X, gridPosition.Vector.Y);
                if (gradient < smallestGradient)
                {
                    smallestGradient = gradient;
                    result = gridPosition;
                }
            }

            return result;
        }

        private static int GetClosestIndex(Bitmap bitmap, in PixelLabxy testPixel, int[] indices, SuperPixelCollection pixels, double m, double S)
        {
            var smallestDistance = 1000000.0;
            var result = -1;

            foreach (var index in indices)
            {
                var superPixel = pixels.GetSuperPixel(index);
                var distance = PixelDistance(testPixel, bitmap.GetCielabPixel(superPixel.Centroid), m, S);

                if(distance > smallestDistance) continue;

                smallestDistance = distance;
                result = index;
            }

            return result;
        }

        /// <summary>
        /// Solve for a given image
        /// </summary>
        /// <param name="bitmap">Image to solve for</param>
        /// <param name="pixelCount">Number of super pixels to generate</param>
        /// <param name="compactness">Variable to determine how compact superpixels should be
        /// valid values are between 1 and 20, 10 is generally a good middle-ground to choose</param>
        public static SuperPixelCollection Solve(Bitmap bitmap, int pixelCount, int compactness)
        {
            // initial values
            var K = pixelCount;
            var N = bitmap.Width * bitmap.Height;
            var m = compactness;
            var S = (int)Math.Sqrt(N / K);
            // double S, forced to be odd
            var S2 = (S * 2) % 2 == 1 ? S * 2 : (S * 2) + 1;

            // translate image pixels to labxy pixels
            var pixels = bitmap.ToCielabPixels();

            // calculate point grid with k regularly spaced points
            var grid = PositionHelper.CalculateGrid(bitmap.GetRectangle(), K);

            // iterate over grid
            for (int i = 0; i < K; i++)
            {
                // assign neighbor with smallest gradient to grid position
                grid[i] = GetSmallestGradient(bitmap, grid[i]);
            }

            // initialize initial superpixel collection from grid
            var superPixels =
                new SuperPixelCollection((from centroid in grid select new SuperPixel(centroid)).ToArray(), S);

            // safety iterator
            var safetyIterator = 0;

            // run optimization loop on superpixel collection
            while (true)
            {
                // reset super pixel containers
                superPixels.ResetPixels();

                // iterate over all pixels
                foreach (var pixel in pixels)
                {
                    // query 2Sx2S grid around the current pixel for all superpixel centroids in it
                    var indices = superPixels.GetContainingSuperPixelIndices(pixel.Position);

                    // get the closest index
                    var closestIndex = GetClosestIndex(bitmap, pixel, indices, superPixels, m, S);

                    if (closestIndex == -1)
                    {
                        //TODO: THis is bad, need to fix is!
                        continue;
                    }

                    // Add pixel to superpixel collection at the closest superpixel index
                    superPixels.AddPosition(closestIndex, pixel.Position);
                }

                // update centroids
                if(!superPixels.UpdatePixelCentroids()) break;

                // check safety
                if (safetyIterator > 100)
                {
                    break;
                }

                safetyIterator++;

            }

            return superPixels;
        }
    }
}
