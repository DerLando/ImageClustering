using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
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

        [TestMethod]
        public void PositionHelperGrid_ShouldContainNoZeroVectors()
        {
            // Arrange
            var rect = new Rectangle(0, 0, 500, 400);
            var pointCount = 50;
            var zero = new Vector2<int>(0, 0);

            // Act
            var grid = PositionHelper.CalculateGrid(rect, pointCount);
            var zeroes = from position in grid where position.Equals(zero) select position;

            // Assert
            // check no zero vectors
            Assert.AreEqual(0, zeroes.Count());
        }

        [TestMethod]
        public void PositionHelperGrid_ShouldRespectBounds()
        {
            // Arrange
            var rect = new Rectangle(0, 0, 1920, 1080);
            var pointCount = 50;

            // Act
            var grid = PositionHelper.CalculateGrid(rect, pointCount);
            var badPositions = new List<Vector2<int>>();
            foreach (var position in grid)
            {
                if(position.X < rect.X | position.X > rect.Width) badPositions.Add(position);
                if(position.Y < rect.Y | position.Y > rect.Height) badPositions.Add(position);
            }

            // Assert
            Assert.AreEqual(0, badPositions.Count);
        }
    }
}
