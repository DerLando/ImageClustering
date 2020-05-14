using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageClusteringLibrary.Data
{
    // A data that can be labeled
    public class LabelData<T>
    {
        /// <summary>
        /// The pixel itself, with color value and position
        /// </summary>
        public readonly T Data;

        /// <summary>
        /// The distance of the pixel to the SuperPixel it is assigned to
        /// </summary>
        public double Distance { get; set; } = double.MaxValue;

        /// <summary>
        /// The index of the SuperPixel this pixel is assigned to
        /// </summary>
        public int Label { get; set; } = -1;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="pixel"></param>
        public LabelData(T data)
        {
            Data = data;
        }

        /// <summary>
        /// If the pixel already has a label assigned
        /// </summary>
        public bool IsLabeled => Label != -1;

        /// <summary>
        /// Resets the pixel label
        /// </summary>
        public void Reset()
        {
            Distance = double.MaxValue;
            Label = -1;
        }
    }
}
