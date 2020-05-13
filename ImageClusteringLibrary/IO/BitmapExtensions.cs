using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
        public static ColorCielab GetCielabColor(this Bitmap bitmap, int x, int y)
        {
            return ColorCielab.FromRgb(new RgbVector(bitmap.GetPixel(x, y)));
        }

        /// <summary>
        /// Gets the color value of a pixel in CIELAB space
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static ColorCielab GetCielabColor(this Bitmap bitmap, in Position position)
        {
            return ColorCielab.FromRgb(new RgbVector(bitmap.GetPixel(position.Vector.X, position.Vector.Y)));
        }

        /// <summary>
        /// Gets the pixel in CIELAB space
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static PixelLabxy GetCielabPixel(this Bitmap bitmap, in Position position)
        {
            return new PixelLabxy(bitmap.GetCielabColor(position), position);
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

        /// <summary>
        /// Converts a bitmap to an array of position vectors, for all pixel positions
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Position[] ToPositionVectors(this Bitmap bitmap)
        {
            var width = bitmap.Width;
            var height = bitmap.Height;
            var positions = new Position[width * height];

            // iterate over image width
            for (int i = 0; i < width; i++)
            {
                // set array offset
                var offset = height * i;
                for (int j = 0; j < height; j++)
                {
                    // create new imagepixel and insert in array
                    positions[offset + j] = new Position(i, j);
                }
            }

            // return result
            return positions;
        }

        /// <summary>
        /// Converts a bitmap to an array of cielab pixels
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static PixelLabxy[] ToCielabPixels(this Bitmap bitmap)
        {
            var width = bitmap.Width;
            var height = bitmap.Height;
            var pixels = new PixelLabxy[width * height];

            // iterate over image width
            for (int i = 0; i < width; i++)
            {
                // set array offset
                var offset = height * i;
                for (int j = 0; j < height; j++)
                {
                    // create new imagepixel and insert in array
                    pixels[offset + j] = new PixelLabxy(bitmap.GetCielabColor(i, j), new Position(i, j));
                }
            }

            // return result
            return pixels;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(this Image image, int width, int height)
        {
            // https://stackoverflow.com/a/24199315
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
