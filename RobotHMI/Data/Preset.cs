namespace RobotHMI.Data
{
    public record Preset
    {
        public string Name { get; set; }
        public object CurrentValue { get; set; }
        public object SetpointValue { get; set; }

        public string Uom { get; set; }

        public string DataType { get; set; }

        public string Topic { get; set; }

        public void Reset()
        {
            SetpointValue = CurrentValue;
        }
    }
}
