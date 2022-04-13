using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Data
{
    public class ObjectDetectionFrameMessage
    {
        public IEnumerable<ObjectDetectionMessage> Detections { get; set; }
    }

    public record ObjectDetectionMessage (string Name, int Confidence, int Left, int Right, int Top, int Bottom);
}
