using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageClusteringLibrary.Data;

namespace ImageClusteringLibrary.Algorithms
{
    public class SuperPixelSegmentor
    {
        /// <summary>
        /// All pixels of the initial image,
        /// Pixels are stored as columns (vertical scanlines)
        /// </summary>
        private readonly LabelData<PixelLabxy>[] _pixels;

        /// <summary>
        /// An array of all centroids of the super pixels
        /// </summary>
        private readonly PixelLabxy[] _superPixelCentroids;

        /// <summary>
        /// The height of the initial image
        /// </summary>
        private readonly int _height;

        /// <summary>
        /// The width of the initial image
        /// </summary>
        private readonly int _width;

        /// <summary>
        /// Compactness value of super pixels
        /// </summary>
        private readonly int m;

        /// <summary>
        /// Super pixel size
        /// </summary>
        private readonly int S;

        /// <summary>
        /// Pixel count
        /// </summary>
        private readonly int N;

        /// <summary>
        /// Super pixel count
        /// </summary>
        private readonly int K;

        /// <summary>
        /// Boolean to determine if all pixels have a label assigned
        /// </summary>
        private bool _isValidConnectivity;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="pixels"></param>
        /// <param name="initialCentroids"></param>
        /// <param name="compactness"></param>
        /// <param name="pixelSize"></param>
        /// <param name="pixelCount"></param>
        public SuperPixelSegmentor(IEnumerable<PixelLabxy> pixels, IEnumerable<Position> initialCentroids, int compactness, int pixelCount, int clusterCount, int imageHeight)
        {
            // initialize pixels array
            _pixels = (from pixel in pixels select new LabelData<PixelLabxy>(pixel)).ToArray();

            // initialize centroids array, we need height for that
            _height = imageHeight;
            _superPixelCentroids = (from centroid in initialCentroids select GetPixel(centroid).Data).ToArray();

            // set other values
            m = compactness;
            S = CalculateSuperPixelSize(pixelCount, clusterCount);
            N = pixelCount;
            K = clusterCount;
            _width = N / _height;

            // Initialize
            Initialize();
        }

        private int CalculateSuperPixelSize(int pixelCount, int superPixelCount)
        {
            var sqrt = (int)Math.Sqrt(pixelCount / superPixelCount);
            return sqrt % 2 == 1 ? sqrt : sqrt + 1;
        }

        private void Initialize()
        {
            // Assign pixel labels
            AssignPixels();

            // update cluster centers
            UpdateSuperPixelCentroids();
        }

        private void ResetPixels()
        {
            Parallel.ForEach(_pixels, (pixel) => pixel.Reset());
        }

        public double Next()
        {
            // reset pixels
            ResetPixels();

            // re-assign
            AssignPixels();

            // update centroids and return error
            return UpdateSuperPixelCentroids();
        }

        /// <summary>
        /// Get the pixel at the specified position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public LabelData<PixelLabxy> GetPixel(in Position position)
        {
            // go to the xth column and get the yth value of it
            return _pixels[_height * position.Vector.X + position.Vector.Y];
        }

        private bool IsPositionInBounds(in Position position)
        {
            if (position.Vector.X < 0 || position.Vector.Y < 0) return false;
            if (position.Vector.X >= _width || position.Vector.Y >= _height) return false;

            return true;
        }

        /// <summary>
        /// Assigns the pixels to the best fitting super pixel
        /// </summary>
        private void AssignPixels()
        {
            int i = 0;
            foreach (var superPixelCentroid in _superPixelCentroids)
            {
                // get grid around centroid
                var grid = PositionHelper.GetNeighboringPositions(superPixelCentroid.Position, (2 * S) - 1);

                // iterate over grid
                foreach (var position in grid)
                {
                    // test bad position, those can happen if we construct a grid close to the image edge
                    if(!IsPositionInBounds(position)) continue;

                    // get the pixel at the grid position
                    var pixel = GetPixel(position);

                    // calculate the distance to the centroid
                    var distance = pixel.Data.DistanceTo(superPixelCentroid, m, S);

                    // test if distance is smaller than tagged distance
                    if(pixel.Distance < distance) continue;

                    // assign distance and index tag
                    pixel.Distance = distance;
                    pixel.Label = i;
                }

                i++;
            }
        }

        private List<PixelLabxy>[] CollectPixels()
        {
            // TODO: Make parallel
            _isValidConnectivity = true;

            // initialize pixel containers
            var pixels = new List<PixelLabxy>[K];
            for (int i = 0; i < K; i++)
            {
                pixels[i] = new List<PixelLabxy>();
            }

            // iterate over all pixels
            foreach (var labelPixel in _pixels)
            {
                // can't do anything for unlabeled pixels
                if (!labelPixel.IsLabeled)
                {
                    _isValidConnectivity = false;
                    continue;
                }

                // assign the pixel to the correct position inside the super pixel array
                pixels[labelPixel.Label].Add(labelPixel.Data);
            }

            return pixels;
        }

        /// <summary>
        /// Updates the super pixel centroids to the mean of all assigned pixels
        /// </summary>
        /// <returns>The Error margin E, defined as the combined distances from new centroids to old centroids</returns>
        private double UpdateSuperPixelCentroids()
        {
            var iterator = 0;
            var error = 0.0;

            // iterate over super pixel pixel collections
            foreach (var pixelCollection in CollectPixels())
            {
                var oldCentroid = _superPixelCentroids[iterator];
                _superPixelCentroids[iterator] = PixelLabxy.Mean(pixelCollection.ToArray());

                error += oldCentroid.DistanceTo(_superPixelCentroids[iterator], m, S);

                iterator++;
            }

            return error;
        }

        /// <summary>
        /// Gets the indices of super pixel centroids which are neighbors
        /// to the given position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private int[] GetNeighboringCentroidIndices(in Position position)
        {
            var indices = new ConcurrentStack<int>();
            var copy = position;

            Parallel.For(0, K, (i) =>
            {
                if (PositionHelper.IsPositionInGridAroundOtherPosition(copy, _superPixelCentroids[i].Position,
                    (2 * S) + 1)) return;

                indices.Push(i);
            });

            return indices.ToArray();
        }

        /// <summary>
        /// Finalizes the pixel labeling, by enforcing connectivity
        /// </summary>
        public IReadOnlyCollection<SuperPixelData> EnforceConnectivity()
        {
            // collect all pixels
            var collected = CollectPixels();

            // if the connectivity is nice already, we can take the early exit
            if (_isValidConnectivity) return ConvertCollectedToIReadonly(collected);

            // iterate over image pixels, look for unlabeled
            Parallel.ForEach(_pixels, (pixel) =>
            {
                if (pixel.IsLabeled) return;

                // get neighboring centroid indices
                var neighborIndices = GetNeighboringCentroidIndices(pixel.Data.Position);

                // find biggest neighbor
                var biggestNeighborIndex =
                    neighborIndices.OrderByDescending(index => collected[index].Count).FirstOrDefault();

                // assign label
                pixel.Label = biggestNeighborIndex;
                collected[biggestNeighborIndex].Add(pixel.Data);
            });

            return ConvertCollectedToIReadonly(collected);
        }

        private IReadOnlyCollection<SuperPixelData> ConvertCollectedToIReadonly(List<PixelLabxy>[] collected)
        {
            var pixels = new SuperPixelData[K];

            for (int i = 0; i < K; i++)
            {
                pixels[i] = new SuperPixelData((from pixel in collected[i] select pixel.Position).ToArray(),
                    _superPixelCentroids[i]);
            }

            return Array.AsReadOnly(pixels);
        }

        public IReadOnlyCollection<SuperPixelData> GetPixels()
        {
            return ConvertCollectedToIReadonly(CollectPixels());
        }

    }
}
