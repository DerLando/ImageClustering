using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageClusteringLibrary.Algorithms
{
    /// <summary>
    /// Different functions to initially generate 'random' cluster centroids
    /// </summary>
    public enum ClusterRandomFunction
    {
        /// <summary>
        /// Initial cluster centroids have to be at least a given distance from
        /// a closest existing cluster centroid
        /// </summary>
        RgbDistance,

        /// <summary>
        /// The initial cluster centroid gets hue rotated n=clusterCount times
        /// for the initial cluster setup
        /// </summary>
        HueShift
    }
}
