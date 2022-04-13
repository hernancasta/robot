using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPLidar
{
    public static class LidarInfoHelper
    {
        public static Measurement[] Sectorize(this Scan data, int sectors, float startangle)
        {
            return data.Measurements.Sectorize(sectors, startangle);
        }

        public static Measurement[] Sectorize(this List<Measurement> data1, int sectors, float startangle)
        {
            var data = data1.ToArray();
            Measurement[] result = new Measurement[360 / sectors]; //4 12
            float step = 360.0f / (float)sectors;
            for (int i = 0; i < sectors; i++)
            {
                float min = startangle + i * step;
                float max = startangle + (i + 1) * step;
                float med = (min + max) / 2.0f;
                if (min < 0)
                {
                    min += 360;
                }
                if (max < 0)
                {
                    max += 360;
                }
                if (min > 360)
                {
                    min -= 360;
                }
                if (max > 360)
                {
                    max -= 360;
                }
                if (med < 0) med += 360;
                if (med > 360) med -= 360;

                var d = (from x in data
                         where (
                                  (
                                      (
                                       (max > min) && (x.Angle >= min && x.Angle < max)
                                      )
                                        ||
                                      (
                                       (max < min) && (x.Angle >= min || x.Angle < max)
                                      )
                                  ) &&
                                     x.Distance > 0
                                )
                         select x.Distance).Min();
                result[i] = new Measurement()
                {
                    Angle = med,
                    Distance = d
                };
            }
            return result;
        }
    }
}
