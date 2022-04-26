using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Data
{
    internal class MapMessage
    {
        public int Size { get; set; }
        public double CellSize { get; set; }
        public byte[] Data { get; set; }
    }
}
