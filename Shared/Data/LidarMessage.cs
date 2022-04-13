using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Data
{
    public class LidarMessage
    {
        public IEnumerable<LidarMeasurement> Measurements { get; set; }

        public record LidarMeasurement(bool IsNewScan, float Angle, float Distance, int? Quality);
    }

}
