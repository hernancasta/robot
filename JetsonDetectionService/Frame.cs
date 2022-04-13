using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JetsonDetectionService
{
    internal class Frame
    {
        public Frame(string data)
        {
            var detects = data.Split('|');
            Detections = new Detection[detects.Length - 1];
            for (int i = 1; i < detects.Length; i++)
            {
                Detections[i - 1] = new Detection(detects[i]);
            }
        }

        public Detection[] Detections { get; set; }
    }

    internal class Detection
    {
        public Detection(string data)
        {
            try
            {
                var fields = data.Split(';');
                if (fields.Length != 6)
                {
                    throw new InvalidOperationException($"unexpected fields count [{fields.Length}] [{data}]");
                }
                ObjectName = fields[0];
                Confidence = int.Parse(fields[1]);
                Left = int.Parse(fields[2]);
                Top = int.Parse(fields[3]);
                Right = int.Parse(fields[4]);
                Bottom = int.Parse(fields[5]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(data);
                throw;
            }
        }

        public string ObjectName { get; private set; }
        public int Confidence { get; private set; }
        public int Left { get; private set; }
        public int Right { get; private set; }
        public int Top { get; private set; }
        public int Bottom { get; private set; }

        public double X
        {
            get
            {
                return (double)(Left + Right) / 2.0;
            }
        }

        public double Y
        {
            get
            {
                return (double)(Top + Bottom) / 2.0;
            }
        }

        public int SizeX
        {
            get
            {
                return Right - Left;
            }
        }


    }
}
