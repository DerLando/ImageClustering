using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageClusteringLibrary.Data.Collections
{
    public class ImagePixelLabxyCollection
    {
        private readonly int _width;
        private readonly int _height;

        /// <summary>
        /// Internal pixels array
        /// Pixels are ordered as columns (vertical scanlines)
        /// </summary>
        private readonly PixelLabxy[] _pixels;

        public int Count => _pixels.Length;

        public ImagePixelLabxyCollection(int width, int height, PixelLabxy[] pixels)
        {
            _width = width;
            _height = height;
            _pixels = pixels;
        }

        /// <summary>
        /// Get the pixel at the specified position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public PixelLabxy GetPixel(in Position position)
        {
            return _pixels[_height * position.Vector.X + position.Vector.Y];
        }

        public IReadOnlyCollection<PixelLabxy> GetPixels() => Array.AsReadOnly(_pixels);
    }
}
