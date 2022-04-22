using RoboclawService.Roboclaw;
using Shared.Data;
using Shared.Streaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboclawService
{
    internal class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IStreamPublisher _streamPublisher;
        private readonly Roboclaw.Roboclaw _roboclaw;
        
        public Worker(ILogger<Worker> logger, Roboclaw.Roboclaw roboclaw ,IStreamPublisher streamPublisher)
        {
            _logger = logger;
            _streamPublisher = streamPublisher;
            _roboclaw = roboclaw;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<Tag> tags = new List<Tag>();

            tags.Add(new Tag<double>("Battery", _roboclaw.GetMainVoltage )) ;
            tags.Add(new Tag<double>("Temperature1", _roboclaw.GetTemperature));
            tags.Add(new Tag<double>("Temperature2", _roboclaw.GetTemperature));

            tags.Add(new DoubleTag<short>("Current1", "Current2", _roboclaw.GetCurrents, 100));
            tags.Add(new DoubleTag<int>  ("Speed1",   "Speed2",   _roboclaw.GetISpeeds));
            tags.Add(new DoubleTag<int>  ("Encoder1", "Encoder2", _roboclaw.GetEncoders));

            int counterSnapshot = 0;

            List<string> activealarms = new List<string>();
            List<string> inactivealarms = new List<string>();
            Dictionary<string, bool> changedAlarms = new Dictionary<string, bool>();

            while (!stoppingToken.IsCancellationRequested) {
                foreach (var tag in tags) {
                    if (tag.DoReading() || counterSnapshot == 0) {
                        foreach(var measure in tag.Getreadings())
                        {
                            await _streamPublisher.PublishAsync($"TAG.{measure.TagName}",
                                new TagMessage { TagName = measure.TagName, TagValue = Convert.ToDouble(measure.TagValue) / measure.TagScale});
                        }
                    }
                }

                uint status = 0;
                if (_roboclaw.GetStatus(ref status)) {
                    changedAlarms.Clear();
                    var lastscan = status.ToAlarms();


                    foreach (var alarm in lastscan)
                    {
                        if (!activealarms.Contains(alarm))
                        {
                            changedAlarms.Add(alarm, true);
                        }
                        if (inactivealarms.Contains(alarm))
                        {
                            inactivealarms.Remove(alarm);
                        }
                    }

                    foreach (var alarm in activealarms.ToArray())
                    {
                        if (!lastscan.Contains(alarm))
                        {
                            inactivealarms.Add(alarm);
                            activealarms.Remove(alarm);
                            changedAlarms.Add(alarm, false);
                        }
                    }

                    foreach (var alarm in changedAlarms) { 
                        await _streamPublisher.PublishAsync($"ALARM.{alarm}",
                        new AlarmMessage { Name = alarm.Key, Active =  alarm.Value});
                    }
                }

                await Task.Delay(100);
                counterSnapshot++;
                if (counterSnapshot == 20) counterSnapshot = 0;

            }
        }
    }
}
