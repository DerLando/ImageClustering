using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageClusteringLibrary.Data
{
    /// <summary>
    /// A pixel representation in the 5 dimensional labxy space.
    /// lab corresponds to the pixels color in the CIELAB space.
    /// xy corresponds to the pixels 2d position inside its image.
    /// </summary>
    public readonly struct PixelLabxy
    {
        /// <summary>
        /// Pixel color in CIELAB color space
        /// </summary>
        public readonly ColorCielab Lab;

        /// <summary>
        /// Position of pixel inside image
        /// </summary>
        public readonly Position Position;

        public PixelLabxy(ColorCielab lab, Position position)
        {
            Lab = lab;
            Position = position;
        }

        /// <summary>
        /// Calculates the distance to another pixel in labxy space
        /// </summary>
        /// <param name="other"></param>
        /// <param name="m"></param>
        /// <param name="S"></param>
        /// <returns></returns>
        [Pure]
        public double DistanceTo(in PixelLabxy other, int m, double S)
        {
            var d_lab = Math.Sqrt((this.Lab.L - other.Lab.L) * (this.Lab.L - other.Lab.L) +
                                  (this.Lab.a - other.Lab.a) * (this.Lab.a - other.Lab.a) +
                                  (this.Lab.b - other.Lab.b) * (this.Lab.b - other.Lab.b));
            var d_xy = this.Position.DistanceTo(other.Position);

            return d_lab + (m / S) * d_xy;
        }

        /// <summary>
        /// Calculates the mean of a collection of pixels in labxy space
        /// </summary>
        /// <param name="pixels"></param>
        /// <returns></returns>
        public static PixelLabxy Mean(PixelLabxy[] pixels)
        {
            var zero = new PixelLabxy(new ColorCielab(0, 0, 0), Position.Zero);
            foreach (var pixelLabxy in pixels)
            {
                zero += pixelLabxy;
            }

            return zero / pixels.Length;
        }

        public static PixelLabxy operator +(in PixelLabxy a, in PixelLabxy b) =>
            new PixelLabxy(a.Lab + b.Lab, a.Position + b.Position);
        public static PixelLabxy operator /(in PixelLabxy a, in double f) => new PixelLabxy(a.Lab / f, a.Position / (int)f);

    }
}
