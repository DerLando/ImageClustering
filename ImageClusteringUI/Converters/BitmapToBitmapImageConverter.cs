using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageClusteringUI.Converters
{
    public static class BitmapToBitmapImageConverter
    {
        /// <summary>
        /// Converts a bitmap to a bitmapImage
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapImage Convert(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }
    }
}
