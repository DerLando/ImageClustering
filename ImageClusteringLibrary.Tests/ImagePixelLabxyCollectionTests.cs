using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageClusteringLibrary.Data;
using ImageClusteringLibrary.Data.Collections;
using ImageClusteringLibrary.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImageClusteringLibrary.Tests
{
    [TestClass]
    public class ImagePixelLabxyCollectionTests
    {
        [TestMethod]
        public void GetPixel_ShouldGiveSameResultsAsOriginalImage()
        {
            // Arrange
            var bitmap = new Bitmap("D:\\Desktop\\cornwall_stock.jpg").ResizeImage(400, 300);
            var pixels = bitmap.ToCielabPixels();
            var collection = new ImagePixelLabxyCollection(bitmap.Width, bitmap.Height, pixels);
            var pos1 = Position.Zero;
            var pos2 = new Position(12, 50);
            var pos3 = new Position(399, 299);

            // Act

            // Assert
            Assert.AreEqual(bitmap.GetCielabPixel(pos1), collection.GetPixel(pos1));
            Assert.AreEqual(bitmap.GetCielabPixel(pos2), collection.GetPixel(pos2));
            Assert.AreEqual(bitmap.GetCielabPixel(pos3), collection.GetPixel(pos3));
        }
    }
}
