using System;
using ImageClusteringLibrary.Algorithms;
using ImageClusteringLibrary.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImageClusteringLibrary.Tests
{
    [TestClass]
    public class SuperPixelTests
    {
        [TestMethod]
        public void TestSuperPixelBoundary()
        {
            // Arrange
            // create an 11x11 square
            var superPixel = new SuperPixel(new Vector2<int>(5, 5));
            var grid = PositionHelper.GetNeighboringPositions(superPixel.Centroid, 11);
            foreach (var position in grid)
            {
                superPixel.AddPositions(position);
            }

            // Act
            var boundary = superPixel.GetBoundaryPositions();

            // Assert
            // should have exactly 40 boundary pixels
            Assert.AreEqual(boundary.Length, 40);

        }
    }
}
