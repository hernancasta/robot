using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Shared.Data
{
    public class GamePadMessage{
        public IEnumerable<Button> Buttons { get; set; }
        public IEnumerable<Axis> Axes { get; set; }

        public bool IsPressed(int ButtonNumber)
        {
            var temp = Buttons.Where(x => x.Index == ButtonNumber);
            if (temp.Any())
            {
                return temp.Single().Pressed;
            }
            return false;
        }

        public short GetAxis(int AxisNumber)
        {
            var temp = Axes.Where(x => x.Index == AxisNumber);
            if (temp.Any())
            {
                return temp.Single().Value;
            }
            return 0;

        }
    }

    public class Button {
        public byte Index { get; set; }
        public bool Pressed { get; set; }

        public const int A = 0;
        public const int B = 1;
        public const int X = 2;
        public const int Y = 3;
        public const int LB = 4;
        public const int RB = 5;
        public const int BACK = 6;
        public const int START = 7;

    }

    public class Axis {
        public byte Index { get; set; }
        public short Value { get; set; }

        public const int LEFT_STICK_X = 0;
        public const int LEFT_STICK_Y = 1;
        public const int RIGHT_STICK_X = 2;
        public const int RIGHT_STICK_Y = 3;
    }

}
