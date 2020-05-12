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
        /// Solve for a given image
        /// </summary>
        /// <param name="bitmap">Image to solve for</param>
        /// <param name="pixelCount">Number of super pixels to generate</param>
        /// <param name="compactness">Variable to determine how compact superpixels should be
        /// valid values are between 1 and 20, 10 is generally a good middle-ground to choose</param>
        public static void Solve(Bitmap bitmap, int pixelCount, int compactness)
        {
            var K = pixelCount;
            var N = bitmap.Width * bitmap.Height;
            var m = compactness;
            var S = Math.Sqrt(N / K);
        }
    }
}
