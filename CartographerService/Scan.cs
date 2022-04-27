using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.Data.LidarMessage;

namespace CartographerService
{
    internal class Scan
    {

        internal List<LidarMeasurement> Measurements { get; set; }

        internal Int32 Encoder1 { get; set; }

        internal Int32 Encoder2 { get; set; }

    }
}
