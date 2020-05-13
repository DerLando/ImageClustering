using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageClusteringLibrary.Data
{
    /// <summary>
    /// A two-dimensional vector of data
    /// </summary>
    /// <typeparam name="T">The type of data to build the vector of</typeparam>
    public readonly struct Vector2<T>
    {
        public readonly T X;
        public readonly T Y;

        public Vector2(T x, T y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"Vector of {typeof(T)}, X:{X}, Y:{Y}";
        }
    }
}
