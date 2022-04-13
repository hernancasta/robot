using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class SabertoothMessage
{

    public SabertoothMessage(string message)
    {
        string[] part = message.Split(':');
        motornumber = int.Parse(part[0].Substring(1));
        value = float.Parse(part[1].Substring(1));
        switch (part[1].Substring(0, 1))
        {
            case "B":
                type = ValueType.Battery;
                value = value / 10;
                break;
            case "C":
                type = ValueType.Current;
                value = value / 10;
                break;
            case "T":
                type = ValueType.Temperature;
                break;
            case " ":
                type = ValueType.Speed;
                switch (part[0].Substring(0, 1))
                {
                    case "A":
                        type = ValueType.SignalA;
                        break;
                    case "S":
                        type = ValueType.SignalS;
                        break;
                }
                break;
        }
    }

    internal ValueType type { get; private set; }

    public string Category { get { return type.ToString(); } }

    public float value { get; private set; }
    public int motornumber { get; private set; }

    public readonly static SabertoothMessage Error = new SabertoothMessage("E0: -99999");

}

public enum ValueType
{
    Unknown,
    Battery,
    Temperature,
    Current,
    Speed,
    SignalA,
    SignalS
}
