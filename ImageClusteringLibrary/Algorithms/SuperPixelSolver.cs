using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageClusteringLibrary.Data;
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
            var d_xy = Math.Sqrt((a.Position.X - b.Position.X) * (a.Position.X - b.Position.X) +
                                 (a.Position.Y - a.Position.Y) * (a.Position.Y - b.Position.Y));

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
        /// Calculates a grid of regular spaced points in the given
        /// rectangle bounds
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="k">number of points in the grid</param>
        /// <returns></returns>
        public static Vector2<int>[] CalculateGrid(Rectangle rect, int k)
        {
            var positions = new Vector2<int>[k];

            // width * aspect_ratio * height = k

            int kS = (int)Math.Sqrt(k);
            int xCount = rect.Width / k;
            int yCount = k / xCount;
            int cellWidth = rect.Width / xCount;
            int cellHeight = rect.Height / yCount;

            for (int i = 0; i < xCount; i++)
            {
                var offset = i * yCount;
                var xPosition = cellWidth * (i + 0.5);

                for (int j = 0; j < yCount; j++)
                {
                    positions[offset + j] = new Vector2<int>((int)xPosition, (int)(cellHeight * (j + 0.5)));
                }
            }

            return positions;
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
            var n1 = ColorNorm(bitmap.GetCielabPixel(x + 1, y) - bitmap.GetCielabPixel(x - 1, y));
            var n2 = ColorNorm(bitmap.GetCielabPixel(x, y + 1) - bitmap.GetCielabPixel(x, y - 1));

            return n1 * n1 + n2 * n2;
        }

        /// <summary>
        /// Gets the position with the smallest gradient in a 3x3 neighborhood
        /// for the given position
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static Vector2<int> GetSmallestGradient(Bitmap bitmap, in Vector2<int> position)
        {
            // best fit
            Vector2<int> result = position;
            var smallestGradient = 1000000.0;

            // check 3x3 neighbours
            for (int i = -1; i < 2; i++)
            {
                var xPosition = position.X + i;

                for (int j = -1; j < 2; j++)
                {
                    var yPosition = position.Y + i;

                    var gradient = PixelGradient(bitmap, xPosition, yPosition);
                    if (gradient < smallestGradient)
                    {
                        smallestGradient = gradient;
                        result = new Vector2<int>(xPosition, yPosition);
                    }
                }
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
        public static void Solve(Bitmap bitmap, int pixelCount, int compactness)
        {
            // initial values
            var K = pixelCount;
            var N = bitmap.Width * bitmap.Height;
            var m = compactness;
            var S = (int)Math.Sqrt(N / K);

            // calculate grid
            var grid = CalculateGrid(bitmap.GetRectangle(), K);

            // iterate over grid
            for (int i = 0; i < K; i++)
            {
                // assign neighbor with smallest gradient to grid position
                grid[i] = GetSmallestGradient(bitmap, grid[i]);
            }

            // iterate of clusters again
            for (int i = 0; i < K; i++)
            {
                var centroid = new PixelLabxy(bitmap.GetCielabPixel(grid[i].X, grid[i].Y), grid[i]);
                var positions = PositionHelper.GetNeighboringPositions(grid[i], 2 * S);

                // iterate over positions
                foreach (var position in positions)
                {
                    // calculate distance in labxy space
                    var labxy = new PixelLabxy(bitmap.GetCielabPixel(position.X, position.Y), position);
                    var distance = PixelDistance(centroid, labxy, m, S);
                }
                // TODO: Calculate distance to cluster centroid
                // TODO: Choose pixels to add to cluster, depending on distance function result
            }
        }
    }
}
