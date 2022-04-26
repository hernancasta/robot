using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Shared.Data.LidarMessage;

namespace MappingService.SLAM
{
    public class ScanSegment
    {
        /// <summary>
        /// Scan rays
        /// </summary>
        public List<LidarMeasurement> Rays { get; init; } = new List<LidarMeasurement>();

        /// <summary>
        /// Origin of rays
        /// </summary>
        public Vector3 Pose { get; init; }

        /// <summary>
        /// Is it last segment of the full 360 degrees scan ?
        /// </summary>
        public bool IsLast { get; init; }
    }
}
