using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private static int GetClosestIndex(in PixelLabxy testPixel, int[] indices, SuperPixelCollection superPixels, ImagePixelLabxyCollection pixels, double m, double S)
        {
            var smallestDistance = 1000000.0;
            var result = -1;

            foreach (var index in indices)
            {
                var superPixel = superPixels.GetSuperPixel(index);
                var distance = PixelDistance(testPixel, pixels.GetPixel(superPixel.Centroid), m, S);

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
        public static IReadOnlyCollection<SuperPixelData> Solve(Bitmap bitmap, int pixelCount, int compactness)
        {
            // initial values
            var K = pixelCount;
            var N = bitmap.Width * bitmap.Height;
            var m = compactness;
            var S = (int)Math.Sqrt(N / K);
            // double S, forced to be odd
            var S2 = (S * 2) % 2 == 1 ? S * 2 : (S * 2) + 1;

            // initialize stopwatch
            var watch = Stopwatch.StartNew();

            // calculate point grid with k regularly spaced points
            var grid = PositionHelper.CalculateGrid(bitmap.GetRectangle(), K);

            // iterate over grid
            for (int i = 0; i < K; i++)
            {
                // assign neighbor with smallest gradient to grid position
                grid[i] = GetSmallestGradient(bitmap, grid[i]);
            }

            // calculate grid creation time
            watch.Stop();
            Console.WriteLine($"Took {watch.ElapsedMilliseconds}ms to calculate initial grid of cluster centroids");
            watch = Stopwatch.StartNew();

            // create the segmentor instance
            var segmentor = new SuperPixelSegmentor(bitmap.ToCielabPixels(), grid, m, N, K, bitmap.Height);

            // calculate segmentor initialization time
            watch.Stop();
            Console.WriteLine($"Took {watch.ElapsedMilliseconds}ms to initialize segmentor");
            watch = Stopwatch.StartNew();

            // keep optimizing until error threshold is reached
            double error = double.MaxValue;
            while (error > 1)
            {
                error = segmentor.Next();
            }

            // calculate error minimization time
            watch.Stop();
            Console.WriteLine($"Took {watch.ElapsedMilliseconds}ms to minimize error");
            watch = Stopwatch.StartNew();

            // finalize inner pixel array
            var result = segmentor.EnforceConnectivity();

            // calculate finalization time
            watch.Stop();
            Console.WriteLine($"Took {watch.ElapsedMilliseconds}ms to finalize output");

            return result;
        }
    }
}
