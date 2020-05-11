using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageClusteringLibrary.Data
{
    /// <summary>
    /// A color representation using the HSV model
    /// </summary>
    public readonly struct ColorHsv
    {
        /// <summary>
        /// Hue of the color, in the range between 0 and 360
        /// </summary>
        public readonly double Hue;

        /// <summary>
        /// Saturation of the color, in the range of 0 to 1
        /// </summary>
        public readonly double Saturation;

        /// <summary>
        /// Value of the color, in the range of 0 to 1
        /// </summary>
        public readonly double Value;

        private ColorHsv(double hue, double saturation, double value)
        {
            Hue = hue;
            Saturation = saturation;
            Value = value;
        }

        /// <summary>
        /// Returns a copy of this color, rotated by the specified degrees
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public ColorHsv AsRotated(double rotation)
        {
            var hue = (this.Hue + rotation) % 360;

            return new ColorHsv(hue, this.Saturation, this.Value);
        }

        /// <summary>
        /// Constructs a <see cref="ColorHsv"/> from a <see cref="Color"/>
        /// </summary>
        /// <param name="color">The color to convert from</param>
        /// <returns></returns>
        public static ColorHsv FromColor(Color color)
        {
            // https://stackoverflow.com/a/1626175
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            var hue = color.GetHue();
            var saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            var value = max / 255d;

            return new ColorHsv(hue, saturation, value);
        }

        /// <summary>
        /// Returns a <see cref="Color"/> representation of this
        /// in RGB space
        /// </summary>
        /// <returns></returns>
        public Color AsColor()
        {
            // https://stackoverflow.com/a/1626175
            int hi = Convert.ToInt32(Math.Floor(this.Hue / 60)) % 6;
            double f = this.Hue / 60 - Math.Floor(this.Hue / 60);

            var value = this.Value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - this.Saturation));
            int q = Convert.ToInt32(value * (1 - f * this.Saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * this.Saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
    }
}
