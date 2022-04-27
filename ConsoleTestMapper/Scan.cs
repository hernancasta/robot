using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.Data.LidarMessage;

namespace CartographerService
{
    public class Scans
    {
        public Scan[] scans { get; set; }

    }

    public class Scan
    {

        public List<LidarMeasurement> Measurements { get; set; }

        public Int32 Encoder1 { get; set; }

        public Int32 Encoder2 { get; set; }

    }
}
