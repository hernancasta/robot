using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Data
{
    public class PresetMessage
    {
        public double CurrentValue { get; set; }

        public double SetValue { get; set; }

        public string Category { get; set; }
        public string Name { get; set; }
        public string Uom { get; set; }
    }
}
