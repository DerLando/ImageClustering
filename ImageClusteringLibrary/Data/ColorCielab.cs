﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageClusteringLibrary.Data
{
    /// <summary>
    /// Color representation in CIELAB color space
    /// </summary>
    public readonly struct ColorCielab
    {
        public readonly double L;
        public readonly double a;
        public readonly double b;

        // d65 reference, because we convert from rgb to CIE 1931
        private static readonly double _reference_x = 95.047;
        private static readonly double _reference_y = 100.0;
        private static readonly double _reference_z = 108.883;

        public ColorCielab(double l, double a, double b)
        {
            L = l;
            this.a = a;
            this.b = b;
        }

        /// <summary>
        /// Calculates a <see cref="ColorCielab"/> from a color in standard rgb space
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        public static ColorCielab FromRgb(in RgbVector rgb)
        {
            var colorXyz = ColorXyz.FromRgb(rgb);

            return FromXyz(colorXyz);
        }

        private static ColorCielab FromXyz(in ColorXyz xyz)
        {
            //Reference-X, Y and Z refer to specific illuminants and observers.
            //Common reference values are available below in this same page.

            var var_X = xyz.X / _reference_x;
            var var_Y = xyz.Y / _reference_y;
            var var_Z = xyz.Z / _reference_z;

            if (var_X > 0.008856) var_X = Math.Pow(var_X, (1.0 / 3.0));
            else var_X = (7.787 * var_X) + (16.0 / 116.0);
            if (var_Y > 0.008856) var_Y = Math.Pow(var_Y, (1.0 / 3.0));
            else var_Y = (7.787 * var_Y) + (16.0 / 116.0);
            if (var_Z > 0.008856) var_Z = Math.Pow(var_Z, (1.0 / 3.0));
            else var_Z = (7.787 * var_Z) + (16.0 / 116.0);

            var L = (116 * var_Y) - 16;
            var a = 500 * (var_X - var_Y);
            var b = 200 * (var_Y - var_Z);

            return new ColorCielab(L, a, b);
        }
    }
}
