using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageClusteringLibrary.Data
{
    /// <summary>
    /// Super pixel POC
    /// </summary>
    public readonly struct SuperPixelData
    {
        /// <summary>
        /// Positions of all pixels related to this super pixel
        /// </summary>
        public readonly Position[] Positions;

        /// <summary>
        /// Centroid of the super pixel, with color information attached
        /// </summary>
        public readonly PixelLabxy Centroid;

        public SuperPixelData(Position[] positions, PixelLabxy centroid)
        {
            Positions = positions;
            Centroid = centroid;
        }
    }
}
