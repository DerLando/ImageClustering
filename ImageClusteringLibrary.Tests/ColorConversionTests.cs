using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageClusteringLibrary.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImageClusteringLibrary.Tests
{
    [TestClass]
    public class ColorConversionTests
    {
        [TestMethod]
        public void Colors_ShouldRoundtripFromRgbToCielab()
        {
            // Arrange
            var color = Color.Red;

            // Act
            var colorRgb = new RgbVector(color);
            var colorXyz = ColorXyz.FromRgb(colorRgb);
            var colorCielab = ColorCielab.FromRgb(colorRgb);
            var colorXyzBack = colorCielab.AsColorXyz();
            var colorRgbBack = colorXyzBack.AsColorRgb();

            // Assert
            Assert.Fail();
        }
    }
}
