using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageClusteringLibrary.Algorithms;
using ImageClusteringLibrary.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImageClusteringLibrary.Tests
{
    [TestClass]
    public class PositionTests
    {
        [TestMethod]
        public void PositionHelperNeighbors_ShouldWork()
        {
            // Arrange
            var center = new Vector2<int>(5, 5);

            // Act
            var grid = PositionHelper.GetNeighboringPositions(center, 11);

            // Assert
            Assert.AreEqual(grid.Length, 121);

        }
    }
}
