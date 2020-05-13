using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageClusteringLibrary.Algorithms;

namespace ImageClusteringLibrary.Data
{
    /// <summary>
    /// Abstraction around a Vector2{int}
    /// </summary>
    public readonly struct Position
    {
        public readonly Vector2<int> Vector;

        public Position(Vector2<int> vector)
        {
            Vector = vector;
        }

        public Position(int x, int y)
        {
            Vector = new Vector2<int>(x, y);
        }

        public override string ToString()
        {
            return $"Position, X:{Vector.X}, Y:{Vector.Y}";
        }

        private static readonly Position _zero = new Position(0, 0);

        /// <summary>
        /// Zero position
        /// </summary>
        public static ref readonly Position Zero => ref _zero;

        //public int X => Vector.X;
        //public int Y => Vector.Y;

        public double DistanceTo(in Position other)
        {
            return Math.Sqrt(DistanceToSquared(other));
        }

        public int DistanceToSquared(in Position other)
        {
            return (this.Vector.X - other.Vector.X) * (this.Vector.X - other.Vector.X) +
                   (this.Vector.Y - other.Vector.Y) * (this.Vector.Y - other.Vector.Y);
        }

        public static Position operator +(in Position a, in Position b) =>
            new Position(a.Vector.X + b.Vector.X, a.Vector.Y + b.Vector.Y);
        
        public static Position operator /(in Position a, in int f) => new Position(a.Vector.X / f, a.Vector.Y / f);
    }
}
