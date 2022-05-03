using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Data
{
    public class PresetMessage
    {
        public object CurrentValue { get; set; } // Value used in service

        public object SetValue { get; set; } // Value desired setted

        public string Category { get; set; }
        public string Name { get; set; }
        public string Uom { get; set; }

        public string DataType { get; set; }

    }
}
