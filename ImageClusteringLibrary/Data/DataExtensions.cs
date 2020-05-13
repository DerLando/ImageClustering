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
        public static Position Mean(this IEnumerable<Position> positions)
        {
            var posArray = positions.ToArray();
            var zero = Position.Zero;

            foreach (var position in posArray)
            {
                zero += position;
            }

            zero /= posArray.Length;

            return zero;
        }

        public static Position MeanParallel(this IEnumerable<Position> positions)
        {
            var posArray = positions.ToArray();
            var count = posArray.Length;

            var xPosition = posArray.AsParallel().Sum(p => p.Vector.X);
            var yPosition = posArray.AsParallel().Sum(p => p.Vector.Y);

            xPosition /= count;
            yPosition /= count;

            return new Position(xPosition, yPosition);
        }
    }
}
