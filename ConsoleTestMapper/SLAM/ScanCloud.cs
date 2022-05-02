using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MappingService.SLAM
{
    internal class ScanCloud
    {
        /// <summary>
        /// Pose at the moment of scanning
        /// </summary>
        public Vector3 Pose { get; init; } = Vector3.Zero;

        /// <summary>
        /// Scan points
        /// </summary>
        public List<Vector2> Points { get; init; } = new List<Vector2>();
    }
}
