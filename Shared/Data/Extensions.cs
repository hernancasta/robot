using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using static Shared.Data.LidarMessage;

namespace Shared.Data
{
    internal static class Extensions
    {
        public static LidarMeasurement ToCenter(this LidarMeasurement measurement, float Distance)
        {
            var Y = Math.Cos(measurement.Angle) * measurement.Distance - Distance;
            var X = Math.Sin(measurement.Angle) * measurement.Distance;

            var angle = Math.Atan2(X, Y);
            var dist = Math.Sqrt(Y * Y + X * X);
            return new LidarMeasurement(measurement.IsNewScan, (float)angle, (float)dist, measurement.Quality);

        }

        public static object GetNativeDataType(this JsonElement preset, string DataType)
        {
            switch (DataType) {
                case "System.UInt32":
                    return preset.GetUInt32();
            }
            return null;
        }
    }

}
