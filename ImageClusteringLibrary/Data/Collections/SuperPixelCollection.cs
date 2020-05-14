using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageClusteringLibrary.Data.Collections
{
    /// <summary>
    /// Abstraction over an array of super pixels.
    /// This allows for easy-to-use queries
    /// </summary>
    public class SuperPixelCollection
    {
        /// <summary>
        /// Super pixels stored inside this collection
        /// </summary>
        private SuperPixel[] _superPixels;

        /// <summary>
        /// Approximate size of individual pixels
        /// </summary>
        private int _pixelSize;

        public SuperPixelCollection(SuperPixel[] superPixels, int pixelSize)
        {
            _superPixels = superPixels;
            _pixelSize = pixelSize;
        }

        public SuperPixel GetSuperPixel(int index) => _superPixels[index];

        /// <summary>
        /// Add a pixel position to the super pixel at the given index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="position"></param>
        public void AddPosition(int index, in Position position)
        {
            _superPixels[index].AddPosition(position);
        }

        /// <summary>
        /// Calculates if a given position could be contained in a superpixel and returns its index.
        /// This is done for all super pixels.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public int[] GetContainingSuperPixelIndices(in Position position)
        {
            // empty list to store indices of super pixels
            var indices = new List<int>();

            // iterate over super pixels
            for (int i = 0; i < _superPixels.Length; i++)
            {
                // test if super pixel could plausibly contain the position
                if(!_superPixels[i].IsValidPositionCandidate(position, _pixelSize)) continue;

                // add pixel index to output list
                indices.Add(i);
            }

            Debug.Assert(indices.Count < 5);
            return indices.ToArray();
        }

        public void ResetPixels()
        {
            foreach (var superPixel in _superPixels)
            {
                superPixel.Reset();
            }
        }

        public bool UpdatePixelCentroids()
        {
            var changed = 0;

            foreach (var superPixel in _superPixels)
            {
                if (superPixel.UpdateCentroid())
                {
                    changed += 1;
                }
            }

            return _superPixels.Length / changed < 20; // less then 20% change
        }

        public Position[][] GetPixelBoundaryPositions()
        {
            // empty list to store pixel boundaries
            var boundaries = new ConcurrentBag<Position[]>();

            // iterate over super pixels
            Parallel.ForEach(_superPixels, (superPixel) => { boundaries.Add(superPixel.GetBoundaryPositions()); });

            return boundaries.ToArray();
        }

        public IReadOnlyCollection<SuperPixel> GetPixels()
        {
            return Array.AsReadOnly(_superPixels);
        }

    }
}
