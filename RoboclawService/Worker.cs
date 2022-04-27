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
            List<object> tags = new List<object>();
            List<object> groupedtags = new List<object>();

            tags.Add(new Tag<double>("Battery", _roboclaw.GetMainVoltage )) ;
            tags.Add(new Tag<double>("Temperature1", _roboclaw.GetTemperature));
            tags.Add(new Tag<double>("Temperature2", _roboclaw.GetTemperature));

            groupedtags.Add(new TagGroup<short>("Current1", "Current2", _roboclaw.GetCurrents,"Currents", 100));
            groupedtags.Add(new TagGroup<int>  ("Speed1",   "Speed2",   _roboclaw.GetISpeeds, "Speeds"));
            groupedtags.Add(new TagGroup<int>  ("Encoder1", "Encoder2", _roboclaw.GetEncoders, "Encoders"));

            int counterSnapshot = 0;

            List<string> activealarms = new List<string>();
            List<string> inactivealarms = new List<string>();
            Dictionary<string, bool> changedAlarms = new Dictionary<string, bool>();

            while (!stoppingToken.IsCancellationRequested) {

                foreach (Tag tag in tags) {
                    if (tag.DoReading() || counterSnapshot == 0) {
                        var measure = tag.GetReading();
                        var tagmessage = new TagMessage { TagName = measure.TagName, TagValue = Convert.ToDouble(measure.TagValue) / measure.TagScale };
                        await _streamPublisher.PublishAsync($"TAG.{measure.TagName}",
                            tagmessage);
                    }
                }
                foreach(TagGroup tag in groupedtags)
                {
                    if (tag.DoReading() || counterSnapshot == 0)
                    {
                        var tagmessages = tag.GetReadings();
                        await _streamPublisher.PublishAsync($"TAGGROUP.{((TagGroup)tag).TagGroupName}",
                            new TagGroupMessage { Tags = tagmessages.ToTagMessageList() });
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
