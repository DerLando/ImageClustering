using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageClusteringLibrary.Data
{
    public readonly struct RgbVector
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public RgbVector(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public RgbVector(Color color)
        {
            X = color.R;
            Y = color.G;
            Z = color.B;
        }

        public int DistanceToSquared(in RgbVector other)
        {
            // Standard pythagorean distance formula 
            return (X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y) + (Z - other.Z) * (Z - other.Z);
        }

        public static double DotProduct(in RgbVector a, in RgbVector b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        /// <summary>
        /// Calculates the arithmetic mean of a collection of <see cref="RgbVector"/>
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static RgbVector Mean(RgbVector[] vectors)
        {
            var sum = vectors.Aggregate(RgbVector.Zero, (current, rgbVector) => current + rgbVector);

            return sum / vectors.Length;
        }

        /// <summary>
        /// Calculates the index of the closest vector to this
        /// in a given collection of vectors
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        [Pure]
        public int ClosestIndex(RgbVector[] vectors)
        {
            int distance = 160000; // 400 * 400
            int index = -1;

            for (int i = 0; i < vectors.Length; i++)
            {
                var currentDistance = this.DistanceToSquared(vectors[i]);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    index = i;
                }
            }

            return index;
        }

        /// <summary>
        /// Returns a <see cref="Color"/> representation of this vector
        /// </summary>
        /// <returns></returns>
        public Color AsColor()
        {
            return Color.FromArgb(this.X, this.Y, this.Z);
        }

        /// <summary>
        /// Returns a copy of this vector rotated around the hsv color circle
        /// by one step, the step length being defined by the number of divisions
        /// </summary>
        /// <param name="circleDivisions">number of segments to divide the hsv circle into</param>
        /// <returns></returns>
        public RgbVector AsHsvRotated(int circleDivisions)
        {
            // convert to hsv color
            var colorHsv = ColorHsv.FromColor(this.AsColor());

            // rotate by circle divisions
            var rotated = colorHsv.AsRotated(360.0 / circleDivisions);

            // return converted back
            return new RgbVector(rotated.AsColor());
        }

        //public int DistanceTo(in RgbVector other)
        //{
        //    return Math.Sqrt(this.DistanceToSquared(other));
        //}

        #region operators

        public static RgbVector operator +(in RgbVector a) => a;
        public static RgbVector operator -(in RgbVector a) => new RgbVector(-a.X, -a.Y, -a.Z);

        public static RgbVector operator +(in RgbVector a, in RgbVector b) => new RgbVector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static RgbVector operator -(in RgbVector a, in RgbVector b) => a + (-b);

        public static double operator *(in RgbVector a, in RgbVector b) => DotProduct(a, b);
        public static RgbVector operator *(in RgbVector a, int b) => new RgbVector(a.X * b, a.Y * b, a.Z * b);
        public static RgbVector operator /(in RgbVector a, int b) => new RgbVector(a.X / b, a.Y / b, a.Z / b);

        #endregion

        private static readonly RgbVector _zero = new RgbVector(0, 0, 0);

        public static ref readonly RgbVector Zero => ref _zero;
    }
}
