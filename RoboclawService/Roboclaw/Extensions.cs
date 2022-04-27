using Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboclawService.Roboclaw
{
    public static class Extensions
    {
        public static UInt16 CalculateCRC16(this byte[] packet)
        {
            UInt16 crc = 0;
            for (int index = 0; index < packet.Length; index++)
            {
                crc = (UInt16)(crc ^ ((UInt16)packet[index] << 8));
                for (byte bit = 0; bit < 8; bit++)
                {
                    if ((crc & 0x8000) != 0)
                    {
                        crc = (UInt16)((crc << 1) ^ 0x1021);
                    }
                    else
                    {
                        crc <<= 1;
                    }
                }
            }
            return crc;

        }

        public static byte[] GetBytes(this object obj)
        {
            if (obj.GetType() == typeof(UInt32))
            {
                return BitConverter.GetBytes((UInt32)obj);
            }
            else if (obj.GetType() == typeof(Int32))
            {
                return BitConverter.GetBytes((Int32)obj);
            }
            else if (obj.GetType() == typeof(UInt16))
            {
                return BitConverter.GetBytes((UInt16)obj);
            }
            else if (obj.GetType() == typeof(Int16))
            {
                return BitConverter.GetBytes((Int16)obj);
            }
            else if (obj.GetType() == typeof(byte))
                return new byte[] { (byte)obj };
            else
            {
                throw new InvalidOperationException($"{obj.GetType().Name} data type not supported");
            }

        }

        public static IEnumerable<string> ToAlarms(this UInt32 status)
        {
            List<string> result = new List<string>();

            if ((status & 0x000001)!= 0) result.Add("E-Stop");
            if ((status & 0x000002) != 0) result.Add("Temperature Error");
            if ((status & 0x000004) != 0) result.Add("Temperature 2 Error");
            if ((status & 0x000008) != 0) result.Add("Main Voltage High Error");
            if ((status & 0x000010) != 0) result.Add("Logic Voltage High Error");
            if ((status & 0x000020) != 0) result.Add("Logic Voltage Low Error");
            if ((status & 0x000040) != 0) result.Add("M1 Driver Fault Error");
            if ((status & 0x000080) != 0) result.Add("M2 Driver Fault Error");
            if ((status & 0x000100) != 0) result.Add("M1 Speed Error");
            if ((status & 0x000200) != 0) result.Add("M2 Speed Error");
            if ((status & 0x000400) != 0) result.Add("M1 Position Error");
            if ((status & 0x000800) != 0) result.Add("M2 Position Error");
            if ((status & 0x001000) != 0) result.Add("M1 Current Error");
            if ((status & 0x002000) != 0) result.Add("M2 Current Error");
            if ((status & 0x010000) != 0) result.Add("M1 Over Current Warning");
            if ((status & 0x020000) != 0) result.Add("M2 Over Current Warning");
            if ((status & 0x040000) != 0) result.Add("Main Voltage High Warning");
            if ((status & 0x080000) != 0) result.Add("Main Voltage Low Warning");
            if ((status & 0x100000) != 0) result.Add("Temperature Warning");
            if ((status & 0x200000) != 0) result.Add("Temperature 2 Warning");
            if ((status & 0x400000) != 0) result.Add("S4 Signal Triggered");
            if ((status & 0x800000) != 0) result.Add("S5 Signal Triggered");
            if ((status & 0x01000000) != 0) result.Add("Speed Error Limit Warning");
            if ((status & 0x02000000) != 0) result.Add("Position Error Limit Warning");

            return result;
        }

        public static UInt32 ReverseBytes(this UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }

        public static UInt16 ReverseBytes(this UInt16 value)
        {
            return (ushort)(((value & 0x00FF) << 16) | 
                (value & 0xFF00) >> 8);
        }

        public static Int32 ReverseBytes(this Int32 value)
        {
            return (int)((value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24);
        }

        public static Int16 ReverseBytes(this Int16 value)
        {
            return (Int16)(((value & 0x00FF) << 16) |
                (value & 0xFF00) >> 8);
        }

        internal static List<TagMessage> ToTagMessageList(this IEnumerable<Reading> list)
        {
            List<TagMessage> result = new List<TagMessage>();

            foreach(var item in list)
            {
                result.Add(new TagMessage { TagName = item.TagName, TagValue = Convert.ToDouble(item.TagValue) / item.TagScale });
            }

            return result;
        }

    }
}