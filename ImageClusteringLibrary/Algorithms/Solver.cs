using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageClusteringLibrary.Data;
using ImageClusteringLibrary.IO;

namespace ImageClusteringLibrary.Algorithms
{
    public class Solver
    {
        private PixelCluster[] _clusters;
        private ImagePixel[] _pixels;
        private Random _rand;
        private Bitmap _originalImage;
        public ClusterRandomFunction Function = ClusterRandomFunction.HueShift;

        public Solver(string filePath, int clusterCount)
        {
            Initialize(filePath, clusterCount);
        }

        // restarts the solver
        public void Restart(int clusterCount)
        {
            _clusters = CalculateInitialClusters(clusterCount);

            this.Next();
        }

        #region Flow

        private void Initialize(string filePath, int clusterCount, int seed = 10000)
        {
            try
            {
                var bitmap = new Bitmap(filePath);
                Initialize(bitmap, clusterCount);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
                return;
            }
        }

        private void Initialize(Bitmap bitmap, int clusterCount, int seed = 10000)
        {
            // save original image reference
            _originalImage = bitmap;

            // initialize random seed
            _rand = new Random(seed);

            // populate internal pixels array
            _pixels = bitmap.ToPixels();

            // populate internal cluster array
            _clusters = CalculateInitialClusters(clusterCount);

            // TODO: pick any random pixel, get its ColorHsv value
            // TODO: Rotate this colorHsv clusterCount times
            // TODO: Create a cluster from each of the rotated colorHsv values
            // TODO: Iterate over all pixels, calculate nearest color cluster and assign
            // TODO: Run k-nearest optimization
        }

        /// <summary>
        /// Run the next k-means optimization step
        /// </summary>
        public void Next()
        {
            // reset all cluster pixels
            ResetClusterPixels();

            // assign the pixels to the best fitting clusters
            AssignPixelsToClusters();

            // update cluster centroids
            UpdateClusterCentroids();

            //TODO: Remove pixels and clusters from internal arrays, if cluster has not changed in update
        }

        #endregion


        /// <summary>
        /// Returns the index of a random pixel in the _pixels array
        /// </summary>
        /// <returns></returns>
        private int GetRandomPixelIndex()
        {
            return _rand.Next(0, _pixels.Length);
        }

        /// <summary>
        /// Calculates a 'good' cluster centroid location from an array of locations
        /// to choose from and an array of aready picked centroids
        /// </summary>
        /// <param name="existingCentroids"></param>
        /// <returns></returns>
        private RgbVector CalculateRandomClusterCentroid(RgbVector[] existingCentroids)
        {
            // get initial index
            var randIndex = GetRandomPixelIndex();
            var testCentroid = _pixels[randIndex].ColorRgb;

            // safety iterator
            int i = 0;


            // early exit
            if (existingCentroids.Length == 0) return testCentroid;

            var closestIndex = testCentroid.ClosestIndex(existingCentroids);
            var closestCentroid = existingCentroids[closestIndex];

            // loop until we find a nice candidate
            while (testCentroid.DistanceToSquared(closestCentroid) > 625) // 25 * 25
            {
                testCentroid = _pixels[GetRandomPixelIndex()].ColorRgb;
                closestCentroid = existingCentroids[testCentroid.ClosestIndex(existingCentroids)];

                // safety iterator
                if (i > 10000)
                {
                    throw new ArgumentOutOfRangeException();
                    break;
                }

                i++;
            }

            return testCentroid;
        }

        /// <summary>
        /// Calculates the initial cluster configuration
        /// All clusters have their centroid set, but no pixels assigned
        /// </summary>
        /// <param name="clusterCount"></param>
        /// <returns></returns>
        private PixelCluster[] CalculateInitialClusters(int clusterCount)
        {
            switch (Function)
            {
                case ClusterRandomFunction.RgbDistance:
                    // start as a list, so we don't initialize wierd rgbvector defaults
                    var clusters = new List<PixelCluster>();

                    for (int i = 0; i < clusterCount; i++)
                    {
                        var centroid =
                            CalculateRandomClusterCentroid((from cluster in clusters select cluster.ColorCentroid).ToArray());

                        clusters.Add(new PixelCluster { ColorCentroid = centroid });
                    }

                    return clusters.ToArray();
                    break;
                case ClusterRandomFunction.HueShift:

                    // other algorithm
                    var firstCentroid = CalculateRandomClusterCentroid(new RgbVector[] { });
                    var centroids = new RgbVector[clusterCount];
                    // rotate centroid color
                    centroids[0] = firstCentroid;
                    for (int i = 1; i < clusterCount; i++)
                    {
                        centroids[i] = centroids[i - 1].AsHsvRotated(clusterCount);
                    }

                    var clustersHue = from centroid in centroids select new PixelCluster { ColorCentroid = centroid };

                    return clustersHue.ToArray();

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Resets the pixel collection of all clusters to be empty
        /// </summary>
        private void ResetClusterPixels()
        {
            foreach (var pixelCluster in _clusters)
            {
                pixelCluster.Pixels = new List<ImagePixel>();
            }
        }

        /// <summary>
        /// Helper method to assign all pixels in _pixels array
        /// to the best fitting cluster
        /// </summary>
        private void AssignPixelsToClusters()
        {
            var centroids = (from cluster in _clusters select cluster.ColorCentroid).ToArray();
            foreach (var imagePixel in _pixels)
            {
                _clusters[imagePixel.ColorRgb.ClosestIndex(centroids)].AddPixel(imagePixel);
            }
        }

        /// <summary>
        /// Updates the centroids of all clusters
        /// use this after assigning pixels to clusters
        /// </summary>
        private bool UpdateClusterCentroids()
        {
            var hasNotChanged = true;
            foreach (var pixelCluster in _clusters)
            {
                hasNotChanged &= pixelCluster.UpdateCentroid();
            }

            return hasNotChanged;
        }

        /// <summary>
        /// Gets the current cluster image result
        /// </summary>
        /// <returns></returns>
        public Bitmap GetClusterImage()
        {
            // copy original
            var bitmap = new Bitmap(_originalImage);

            //// lock bits to write only
            //var bitmapData = bitmap.LockBits(bitmap.GetRectangle(), System.Drawing.Imaging.ImageLockMode.WriteOnly,
            //    bitmap.PixelFormat);

            // write all pixels from clusters
            foreach (var pixelCluster in _clusters)
            {
                var clusterColor = pixelCluster.ColorCentroid.AsColor();
                foreach (var pixel in pixelCluster.Pixels)
                {
                    bitmap.SetPixel(pixel.Position.X, pixel.Position.Y, clusterColor);
                }
            }

            //// unlock the bitmap
            //bitmap.UnlockBits(bitmapData);

            // return result
            return bitmap;
        }
    }
}
