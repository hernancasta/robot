using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Data
{
    public class GamePadMessage{
        public IEnumerable<Button> Buttons { get; set; }
        public IEnumerable<Axis> Axes { get; set; }
    }

    public class Button {
        public byte Index { get; set; }
        public bool Pressed { get; set; }
    }

    public class Axis {
        public byte Index { get; set; }
        public short Value { get; set; }
    }

}
