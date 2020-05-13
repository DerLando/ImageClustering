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
                    positions[offset + j] = new Vector2<int>((int)xPosition, (int)(cellHeight * (j + 0.5)));
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
                positions[startIndex + i] = new Vector2<int>((int)xPos, (int)(cellHeight * (i + 0.5)));
            }

            return positions;
        }

        public static double PositionSquaredDistance(in Vector2<int> a, in Vector2<int> b)
        {
            return a.X - b.X * a.X - b.X + a.Y - b.Y * a.Y - a.Y;
        }

        public static double PositionDistance(in Vector2<int> a, in Vector2<int> b)
        {
            return Math.Sqrt(PositionSquaredDistance(a, b));
        }

    }
}
