using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageClusteringLibrary.Data;

namespace ImageClusteringLibrary.Algorithms
{
    public static class PositionHelper
    {
        /// <summary>
        /// Calculates the grid of neighboring positions around the given position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="depth">number of positions in x and y direction to find, should be odd</param>
        /// <returns></returns>
        public static Position[] GetNeighboringPositions(in Position position, int depth)
        {
            var positions = new Position[depth * depth];
            var minDepth = -depth / 2;
            var maxDepth = depth / 2;
            var iter = 0;

            for (int i = minDepth; i <= maxDepth; i++)
            {
                var offset = iter * depth;
                var xPosition = position.Vector.X + i;
                var innerIter = 0;

                for (int j = minDepth; j <= maxDepth; j++)
                {
                    var yPosition = position.Vector.Y + j;

                    positions[offset + innerIter] = new Position(xPosition, yPosition);

                    innerIter++;
                }

                iter++;
            }

            return positions;
        }

        /// <summary>
        /// Calculates a grid of regular spaced points in the given
        /// rectangle bounds
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="k">number of points in the grid</param>
        /// <returns></returns>
        public static Position[] CalculateGrid(Rectangle rect, int k)
        {
            var positions = new Position[k];

            int kS = (int)Math.Sqrt(k);
            int kPow = k * k;
            int xCount = rect.Width * kS / rect.Height;
            int yCount = k / xCount;
            int cellWidth = rect.Width / xCount;
            int cellHeight = rect.Height / yCount;

            // test if formula adds up
            var isNice = xCount * yCount == k;
            if (!isNice) xCount -= 1;

            for (int i = 0; i < xCount; i++)
            {
                var offset = i * yCount;
                var xPosition = cellWidth * (i + 0.5);

                for (int j = 0; j < yCount; j++)
                {
                    positions[offset + j] = new Position((int)xPosition, (int)(cellHeight * (j + 0.5)));
                }
            }

            // if we had a good x-y configuration we can return here
            if (isNice) return positions;

            var xPos = cellWidth * (xCount + 0.5);
            var remainingCount = k - ((xCount + 1) * yCount - yCount);
            var startIndex = k - remainingCount;
            cellHeight = rect.Height / remainingCount;
            for (int i = 0; i < remainingCount ; i++)
            {
                positions[startIndex + i] = new Position((int)xPos, (int)(cellHeight * (i + 0.5)));
            }

            return positions;
        }

        /// <summary>
        /// Determines if the first position is inside the grid around the second position
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="gridSize"></param>
        /// <returns></returns>
        public static bool IsPositionInGridAroundOtherPosition(in Position a, in Position b, int gridSize)
        {
            var deltaX = Math.Abs(a.Vector.X - b.Vector.X);
            var deltaY = Math.Abs(a.Vector.Y - b.Vector.Y);

            return deltaX <= gridSize && deltaY <= gridSize;
        }

        /// <summary>
        /// Gets all boundary positions in a given collection of positions.
        /// Boundary positions are defined as not having a neighbor on every adjacent position
        /// </summary>
        /// <param name="positions"></param>
        /// <returns></returns>
        public static IEnumerable<Position> GetBoundaryPositions(IEnumerable<Position> positions)
        {
            // store positions as array
            var posArray = positions.ToArray();

            // create a dict do keep track of used positions
            var positionContainment = new Dictionary<Position, bool>();

            // empty list to store boundary positions
            var boundaryPositions = new List<Position>();

            // iterate over positions
            foreach (var position in posArray)
            {
                // create a adjacent grid of positions around current position
                var grid = GetNeighboringPositions(position, 3);

                // array that stores a value for every grid positions containment
                var areContained = new bool[9];

                for (int i = 0; i < 9; i++)
                {
                    // easy way out -> position has been tested and is in dictionary
                    if (positionContainment.ContainsKey(grid[i]))
                        areContained[i] = positionContainment[grid[i]];

                    // if not, we have to calculate containment
                    else
                    {
                        areContained[i] = posArray.Contains(grid[i]);
                        positionContainment[grid[i]] = areContained[i];
                    }
                }

                // if any position is not contained in the grid, we define the tested position as boundary
                if(areContained.All(b => b))
                    continue;

                boundaryPositions.Add(position);
            }

            return boundaryPositions;
        }
    }
}
