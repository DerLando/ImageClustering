using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageClusteringLibrary.Data
{
    /// <summary>
    /// Color representation in XYZ space
    /// </summary>
    public readonly struct ColorXyz
    {
        public readonly double X;
        public readonly double Y;
        public readonly double Z;

        public ColorXyz(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Create a <see cref="ColorXyz"/> from a color in rgb color space
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        public static ColorXyz FromRgb(in RgbVector rgb)
        {
            //sR, sG and sB (Standard RGB) input range = 0 ÷ 255
            //X, Y and Z output refer to a D65/2° standard illuminant.
            //http://www.easyrgb.com/en/math.php

            var var_R = ((double)rgb.X / 255);
            var var_G = ((double)rgb.Y / 255);
            var var_B = ((double)rgb.Z / 255);

            if (var_R > 0.04045) var_R = Math.Pow((var_R + 0.055) / 1.055, 2.4);
            else var_R = var_R / 12.92;
            if (var_G > 0.04045) var_G = Math.Pow((var_G + 0.055) / 1.055, 2.4);
            else var_G = var_G / 12.92;
            if (var_B > 0.04045) var_B = Math.Pow((var_B + 0.055) / 1.055, 2.4);
            else var_B = var_B / 12.92;

            var_R = var_R * 100;
            var_G = var_G * 100;
            var_B = var_B * 100;

            var X = var_R * 0.4124 + var_G * 0.3576 + var_B * 0.1805;
            var Y = var_R * 0.2126 + var_G * 0.7152 + var_B * 0.0722;
            var Z = var_R * 0.0193 + var_G * 0.1192 + var_B * 0.9505;

            return new ColorXyz(X, Y, Z);
        }

        /// <summary>
        /// Returns a copy of this color in standard rgb space
        /// </summary>
        /// <returns></returns>
        public RgbVector AsColorRgb()
        {
            //X, Y and Z input refer to a D65/2° standard illuminant.
            //sR, sG and sB (standard RGB) output range = 0 ÷ 255

            var var_X = X / 100.0;
            var var_Y = Y / 100.0;
            var var_Z = Z / 100.0;

            var var_R = var_X * 3.2406 + var_Y * -1.5372 + var_Z * -0.4986;
            var var_G = var_X * -0.9689 + var_Y * 1.8758 + var_Z * 0.0415;
            var var_B = var_X * 0.0557 + var_Y * -0.2040 + var_Z * 1.0570;

            var_R = var_R > 0.0031308 ? 1.055 * (Math.Pow(var_R, (1 / 2.4))) - 0.055 : 12.92 * var_R;
            var_G = var_G > 0.0031308 ? 1.055 * (Math.Pow(var_G, (1 / 2.4))) - 0.055 : 12.92 * var_G;
            var_B = var_B > 0.0031308 ? 1.055 * (Math.Pow(var_B, (1 / 2.4))) - 0.055 : 12.92 * var_B;

            var sR = (int)(var_R * 255);
            var sG = (int)(var_G * 255);
            var sB = (int)(var_B * 255);

            return new RgbVector(sR, sG, sB);
        }
    }
}
