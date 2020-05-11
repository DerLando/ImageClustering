using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageClusteringLibrary.Data
{
    public class PixelCluster
    {
        // TODO: Instead of individual pixels, store super-pixels
        public List<ImagePixel> Pixels = new List<ImagePixel>();
        public RgbVector ColorCentroid;

        public void AddPixel(ImagePixel pixel)
        {
            Pixels.Add(pixel);
        }

        /// <summary>
        /// Updates the centroid to the mean of all pixel rgb vectors stored inside
        /// </summary>
        public bool UpdateCentroid()
        {
            if (Pixels.Count == 0) return false;

            var oldCentroid = ColorCentroid;
            ColorCentroid = RgbVector.Mean((from pixel in Pixels select pixel.ColorRgb).ToArray());

            return oldCentroid.Equals(ColorCentroid);
        }
    }
}
