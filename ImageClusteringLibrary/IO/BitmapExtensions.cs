using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageClusteringLibrary.Data;

namespace ImageClusteringLibrary.IO
{
    public static class BitmapExtensions
    {
        public static Rectangle GetRectangle(this Bitmap bitmap)
        {
            return new Rectangle(0, 0, bitmap.Width, bitmap.Height);
        }

        /// <summary>
        /// Gets the color value of a pixel in CIELAB space
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static ColorCielab GetCielabPixel(this Bitmap bitmap, int x, int y)
        {
            return ColorCielab.FromRgb(new RgbVector(bitmap.GetPixel(x, y)));
        }

        public static ImagePixel[] ToPixels(this Bitmap bitmap)
        {
            //// lock the image bytes
            //var bmpData = bitmap.LockBits(bitmap.GetRectangle(), System.Drawing.Imaging.ImageLockMode.ReadOnly,
            //    bitmap.PixelFormat);

            // create empty array
            var width = bitmap.Width;
            var height = bitmap.Height;
            var pixels = new ImagePixel[width * height];

            // iterate over image width
            for (int i = 0; i < width; i++)
            {
                // set array offset
                var offset = height * i;
                for (int j = 0; j < height; j++)
                {
                    // create new imagepixel and insert in array
                    pixels[offset + j] = new ImagePixel(i, j, bitmap.GetPixel(i, j));
                }
            }

            //// unlock the bitmap data
            //bitmap.UnlockBits(bmpData);

            // return result
            return pixels;
        }
    }
}
