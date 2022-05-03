using Shared.Data;
using Shared.Streaming;
using SkiaSharp;
using System.Collections.Concurrent;

namespace RobotHMI.Data
{
    public class RobotClientService : BackgroundService
    {
        private readonly ILogger<RobotClientService> _logger;
        private readonly IStreamSubscriber _streamSubscriber;

//        private SkiaSharp.SKBitmap _skBitmap;

        public event Action OnChange;
        public event Action OnTagChange;
        public event Action OnAlarmChange;
        private void NotifyStateChanged() => OnChange?.Invoke();
        private void NotifyTagsChanged() => OnTagChange?.Invoke();
        private void NotifyAlarmsChanged() => OnAlarmChange?.Invoke();

        private ConcurrentDictionary<string,TagMessage> Signals = new ConcurrentDictionary<string,TagMessage>();
        internal ConcurrentDictionary<string, Alarm> Alarms = new ConcurrentDictionary<string, Alarm>();

        public LidarMessage? LidarScan { get; protected set; }

        public RobotClientService(ILogger<RobotClientService> logger, IStreamSubscriber streamSubscriber)
        {
            _logger = logger;
            _streamSubscriber = streamSubscriber;
        }

        internal int AlarmsCount()
        {
            return Alarms.Where(x => x.Value.Active).Count();
        }

        private SKColor unknown = new SKColor(0, 0, 0);
        private SKColor empty = new SKColor(80, 80, 80);
        private SKColor occupied = new SKColor(255, 0, 0);


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _streamSubscriber.SubscribeAsync<LidarMessage>("lidar", (data) => {
                LidarScan = data;
                NotifyStateChanged();
            });

            await _streamSubscriber.SubscribeAsync<AlarmMessage>("ALARM.*", (alarm) => {
                var alarmNew = new Alarm(alarm.Name, DateTime.Now, alarm.Active);
                var alarm1 = Alarms.GetOrAdd(alarm.Name, alarmNew);

                //if (!Alarms.ContainsKey(alarm.Name))
                //{
                //    Alarms.Add(alarm.Name, alarm1);
                //    NotifyAlarmsChanged();
                //};
                if (Alarms[alarm.Name].Active != alarm.Active) { 
                    if (!Alarms[alarm.Name].Active && alarm.Active)
                    {
                        alarm1 = new Alarm(alarm.Name, DateTime.Now, alarm.Active);
                    } else
                    {
                        alarm1 = new Alarm(alarm.Name, Alarms[alarm.Name].ActivationDate, alarm.Active);
                    }
                    Alarms[alarm.Name] = alarm1;
                    NotifyAlarmsChanged();
                }
            });

            await _streamSubscriber.SubscribeAsync<TagMessage>("TAG.*", (tag) => {
                //if (!Signals.ContainsKey(tag.TagName))
                //{
                //    Signals.Add(tag.TagName, tag);
                //}
                //Signals[tag.TagName] = tag;
                Signals.AddOrUpdate(tag.TagName, tag, (key, value) => { return tag; });
                NotifyTagsChanged();
            });

            await _streamSubscriber.SubscribeAsync<TagGroupMessage>("TAGGROUP.*", (tag) =>
            {
                foreach (var t in tag.Tags)
                {
                    Signals.AddOrUpdate(t.TagName, t, (key, value) => { return t; });
                }
            });

                /*
                await _streamSubscriber.SubscribeAsync<MapMessage>(("MAP"), (map) => {

                    if (_skBitmap == null)
                    {
                        _skBitmap = new SkiaSharp.SKBitmap(map.Size, map.Size);
                    }

                    for (int i = 0; i < map.Data.Length; i++)
                    {
                        var color = unknown;
                        if (map.Data[i] == 1)
                        {
                            color = empty;
                        }
                        if (map.Data[i] > 1) { color = occupied; }
                            var x = i % map.Size;
                            var y = i / map.Size;
                        _skBitmap.SetPixel(x ,y, color);
                    }

                    using (MemoryStream memStream = new MemoryStream())
                    using (SKManagedWStream wstream = new SKManagedWStream(memStream))
                    {
                        _skBitmap.Encode(wstream, SKEncodedImageFormat.Png, 100);
                        byte[] data = memStream.ToArray();

                        using (FileStream fs = new FileStream(@"C:\Hernan\Source\Robot2022\Robot2022\RobotHMI\bin\Debug\net6.0\hernan.png", FileMode.Create)) {

                            fs.Write(data);//, folder, filename);
                        }

                    }

                });
                */
            }


        internal string RobotStatus() { 
            if (Alarms.TryGetValue("EStopPressed",out Alarm alarm))
            {
                if (alarm.Active)
                {
                    return "blocked";
                }
            }
            return "idle";
        }

        internal void ClearAlarms()
        {
            Alarms.Clear();
        }

        internal double GetBattery()
        {
            if (Signals.ContainsKey("Battery"))
            {
                return Signals["Battery"].TagValue;
            }
            return -999;
        }

        internal int GetBatteryLevelIndex()
        {
            if (!Signals.ContainsKey("Battery")) return 0;
            var bat = Signals["Battery"].TagValue;
            if (bat > 25.24) return 4;
            if (bat > 24.8) return 3;
            if (bat > 24.2) return 2;
            if (bat > 23.32) return 1;
            return 0;
        }

        internal double GetTemperature()
        {
            if (Signals.ContainsKey("Temperature1") 
                )
            {
                return Signals["Temperature1"].TagValue;
            }
            return 0;
        }

        internal double GetTag(string tagname)
        {
            if (Signals.ContainsKey(tagname))
            {
                return Signals[tagname].TagValue;
            }
            return -999;
        }

        internal bool HasTag(string tagname)
        {
            return Signals.ContainsKey(tagname);
        }
    }
}
