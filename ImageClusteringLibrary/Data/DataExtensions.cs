using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageClusteringLibrary.Data
{
    public static class DataExtensions
    {
        /// <summary>
        /// Calculates the arithmetic mean of a collection of <see cref="Vector2{int}"/>
        /// </summary>
        /// <param name="positions"></param>
        /// <returns></returns>
        public static Vector2<int> Mean(this IEnumerable<Vector2<int>> positions)
        {
            var posArray = positions.ToArray();
            int xPosition = 0;
            int yPosition = 0;

            foreach (var position in posArray)
            {
                xPosition += position.X;
                yPosition += position.Y;
            }

            xPosition /= posArray.Length;
            yPosition /= posArray.Length;

            return new Vector2<int>(xPosition, yPosition);
        }
    }
}
