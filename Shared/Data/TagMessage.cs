using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Data
{
    internal class TagMessage
    {
        public string TagName { get; set; }
        public double TagValue { get; set; }
    }

    internal class TagGroupMessage
    {
        public List<TagMessage> Tags { get; set; }
    }
}
