using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageClusteringLibrary.Data;

namespace ImageClusteringLibrary.Algorithms
{
    public class KMeansSegmentator
    {
        /// <summary>
        /// underlying super pixels
        /// </summary>
        private readonly LabelData<SuperPixelData>[] _superPixels;

        private RgbVector[] _colorCentroids;

        /// <summary>
        /// Cluster count K
        /// </summary>
        private readonly int K;

        public KMeansSegmentator(IEnumerable<SuperPixelData> superPixels, int clusterCount)
        {
            _superPixels = (from pixel in superPixels select new LabelData<SuperPixelData>(pixel)).ToArray();

            K = clusterCount;

            Initialize();
        }

        private void Initialize()
        {
            // initialize initial color centroids
            _colorCentroids = CalculateInitialColorCentroids();

            // Assign pixel labels
            AssignPixels();

            // Update Centroids
            UpdateColorCentroids();
        }

        private List<RgbVector>[] CollectPixels()
        {
            var pixels = new List<RgbVector>[K];
            for (int i = 0; i < K; i++)
            {
                pixels[i] = new List<RgbVector>();
            }

            // iterate over super pixels
            foreach (var superPixel in _superPixels)
            {
                // can't do anything for unlabeled
                if(!superPixel.IsLabeled) continue;

                // assign to correct position in array
                pixels[superPixel.Label].Add(superPixel.Data.Centroid.Lab.AsColorXyz().AsColorRgb());
            }

            return pixels;
        }

        private double UpdateColorCentroids()
        {
            var iterator = 0;
            var error = 0.0;

            // iterate over super pixel pixel collections
            foreach (var pixelCollection in CollectPixels())
            {
                if(pixelCollection.Count == 0) continue;

                var oldCentroid = _colorCentroids[iterator];
                _colorCentroids[iterator] = RgbVector.Mean(pixelCollection.ToArray());

                error += oldCentroid.DistanceToSquared(_colorCentroids[iterator]);

                iterator++;
            }

            return error;
        }

        private void AssignPixels()
        {
            int i = 0;
            foreach (var superPixel in _superPixels)
            {
                for (int j = 0; j < K; j++)
                {
                    var distance = superPixel.Data.Centroid.Lab.AsColorXyz().AsColorRgb()
                        .DistanceToSquared(_colorCentroids[j]);
                    if(distance > superPixel.Distance) continue;

                    superPixel.Distance = distance;
                    superPixel.Label = j;
                }
            }
        }

        private RgbVector[] CalculateInitialColorCentroids()
        {
            var rand = new Random();
            var centroids = new RgbVector[K];

            for (int i = 0; i < K; i++)
            {
                centroids[i] = _superPixels[rand.Next(0, _superPixels.Length)].Data.Centroid.Lab.AsColorXyz()
                    .AsColorRgb();
            }

            return centroids;
        }

        public IReadOnlyCollection<ImagePixel> GetPixels()
        {
            // TODO: Inefficient
            var pixels = new List<ImagePixel>();

            // iterate over super pixels
            foreach (var superPixel in _superPixels)
            {
                // can't do anything for unlabeled
                if (!superPixel.IsLabeled) continue;

                var color = superPixel.Data.Centroid.Lab.AsColorXyz().AsColorRgb().AsColor();

                // assign to correct position in array
                foreach (var dataPosition in superPixel.Data.Positions)
                {
                    pixels.Add(new ImagePixel(dataPosition.Vector.X, dataPosition.Vector.Y, color));
                }
            }

            return pixels;
        }
    }
}
