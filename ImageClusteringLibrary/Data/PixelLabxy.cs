using System;
using System.Collections.Generic;
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
        public readonly Vector2<int> Position;

        public PixelLabxy(ColorCielab lab, Vector2<int> position)
        {
            Lab = lab;
            Position = position;
        }
    }
}
