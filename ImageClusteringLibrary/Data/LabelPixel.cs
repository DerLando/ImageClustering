using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageClusteringLibrary.Data
{
    // A Pixel position that can be labeled
    public class LabelPixel
    {
        /// <summary>
        /// The pixel itself, with color value and position
        /// </summary>
        public readonly PixelLabxy Pixel;

        /// <summary>
        /// The distance of the pixel to the SuperPixel it is assigned to
        /// </summary>
        public double Distance { get; set; } = double.MaxValue;

        /// <summary>
        /// The index of the SuperPixel this pixel is assigned to
        /// </summary>
        public int SuperPixelIndex { get; set; } = -1;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="pixel"></param>
        public LabelPixel(PixelLabxy pixel)
        {
            Pixel = pixel;
        }

        /// <summary>
        /// If the pixel already has a label assigned
        /// </summary>
        public bool IsLabeled => SuperPixelIndex != -1;

        /// <summary>
        /// Resets the pixel label
        /// </summary>
        public void Reset()
        {
            Distance = double.MaxValue;
            SuperPixelIndex = -1;
        }
    }
}
