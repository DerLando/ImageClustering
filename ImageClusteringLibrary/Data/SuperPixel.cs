using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageClusteringLibrary.Algorithms;

namespace ImageClusteringLibrary.Data
{
    /// <summary>
    /// A collection of pixel positions, forming a logically group
    /// </summary>
    public class SuperPixel
    {
        /// <summary>
        /// Pixel positions related to this super pixel
        /// </summary>
        private List<Position> _positions = new List<Position>();

        /// <summary>
        /// The logical center position of the super pixel
        /// </summary>
        private Position _centroid;

        public Position Centroid => _centroid;

        public SuperPixel(in Position centroid)
        {
            _centroid = centroid;
        }

        public override string ToString()
        {
            return $"SuperPixel: Centroid {_centroid}, PixelCount {_positions.Count}";
        }

        /// <summary>
        /// Add a pixel position to this super pixel
        /// </summary>
        /// <param name="position"></param>
        public void AddPosition(in Position position) { _positions.Add(position);}

        /// <summary>
        /// Updates the centroid of the superpixel to the mean of all pixel
        /// positions stored inside
        /// </summary>
        /// <returns>True if the centroid has changed, false if it stayed the same</returns>
        public bool UpdateCentroid()
        {
            // if no pixels are assigned, we can't do anything
            if(_positions.Count == 0) return false;

            // copy old centroid to compare
            var oldCentroid = _centroid;

            // set centroid to mean of all pixel positions
            _centroid = _positions.Mean();

            // compare old and new centroid
            return oldCentroid.DistanceToSquared(_centroid) < 25; // < 5²
        }

        /// <summary>
        /// Clears all stored pixel positions, the centroid stays the same
        /// </summary>
        public void Reset() { _positions = new List<Position>();}

        public Position[] GetBoundaryPositions()
        {
            // all pixels that have an existing neighbor on all positions in a 3x3 grid around them
            // are by definition inner pixels

            // new list to store boundary pixels
            var boundary = new List<Position>();

            // iterate over positions
            foreach (var position in _positions)
            {
                // get 3x3 grid points around this
                var grid = PositionHelper.GetNeighboringPositions(position, 3);

                // test if all grid points are contained inside of super pixel
                if (grid.All(g => _positions.Contains(g))) continue;

                // add test position to boundary list
                boundary.Add(position);
            }

            // return result
            return boundary.ToArray();
        }

        /// <summary>
        /// Checks if a position is a valid candidate to be contained in this super pixel
        /// </summary>
        /// <param name="position"></param>
        /// <param name="distance">Maximum distance for a position away from the centroid to be considered a valid candidate</param>
        /// <returns>True if the position is a valid candidate, False if not</returns>
        public bool IsValidPositionCandidate(in Position position, int distance)
        {
            return Math.Abs(position.Vector.X - _centroid.Vector.X) <= distance &&
                   Math.Abs(position.Vector.Y - _centroid.Vector.Y) <= distance;
        }
    }
}
