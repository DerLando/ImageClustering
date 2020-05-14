using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageClusteringLibrary.Data;

namespace ImageClusteringLibrary.Algorithms
{
    /// <summary>
    /// A solver that calculates K-means image clustering
    /// </summary>
    public static class KMeansSolver
    {
        /// <summary>
        /// Solves k-means clustering of the given image
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="superPixelCount"></param>
        /// <param name="superPixelCompactness"></param>
        /// <param name="clusterCount"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<ImagePixel> Solve(Bitmap bitmap, int superPixelCount, int superPixelCompactness, int clusterCount)
        {
            // solve super pixels
            var superPixels = SuperPixelSolver.Solve(bitmap, superPixelCount, superPixelCompactness);

            return Solve(bitmap, superPixels, clusterCount);
        }

        /// <summary>
        /// Solve override to use if you already have superpixels calculated
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="superPixels"></param>
        /// <param name="clusterCount"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<ImagePixel> Solve(Bitmap bitmap, IEnumerable<SuperPixelData> superPixels,
            int clusterCount)
        {
            // create segmentator from super pixels
            var segmentator = new KMeansSegmentator(superPixels, clusterCount);

            // run optimization
            do
            {
                // we do nothing
            } while (segmentator.Next() > 1);

            return segmentator.GetPixels();
        }
    }
}
