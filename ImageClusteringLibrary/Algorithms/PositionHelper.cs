using System;
using System.Collections.Generic;
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
        public static Vector2<int>[] GetNeighboringPositions(in Vector2<int> position, int depth)
        {
            var positions = new Vector2<int>[depth * depth];
            var minDepth = -depth / 2;
            var maxDepth = depth / 2;
            var iter = 0;

            for (int i = minDepth; i <= maxDepth; i++)
            {
                var offset = iter * depth;
                var xPosition = position.X + i;
                var innerIter = 0;

                for (int j = minDepth; j <= maxDepth; j++)
                {
                    var yPosition = position.Y + j;

                    positions[offset + innerIter] = new Vector2<int>(xPosition, yPosition);

                    innerIter++;
                }

                iter++;
            }

            return positions;
        }
    }
}
