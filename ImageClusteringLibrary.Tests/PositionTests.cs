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
            var center = new Position(5, 5);

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
            var zero = new Position(0, 0);

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
            var badPositions = new List<Position>();
            foreach (var position in grid)
            {
                if(position.Vector.X < rect.X | position.Vector.X > rect.Width) badPositions.Add(position);
                if(position.Vector.Y < rect.Y | position.Vector.Y > rect.Height) badPositions.Add(position);
            }

            // Assert
            Assert.AreEqual(0, badPositions.Count);
        }

        [TestMethod]
        public void PositionMeanParallel_ShouldGiveSameResultAsSingleThreaded()
        {
            // Arrange
            var positions = (from i in Enumerable.Range(0, 10000) select new Position(i, i)).ToArray();

            // Act
            var expected = positions.Mean();
            var actual = positions.MeanParallel();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
