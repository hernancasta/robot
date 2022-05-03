using Shared.Command.Preset;
using Shared.Data;
using Shared.Messaging;
using Shared.Streaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace ManualControlService
{
    internal class PresetCommandListener : BackgroundService
    {
        private readonly IStreamPublisher _streamPublisher;
        protected readonly IMessageSubscriber _messageSubscriber;
        private readonly string TOPIC;

        public PresetCommandListener(IMessageSubscriber messageSubscriber, IStreamPublisher streamPublisher)
        { 
            _streamPublisher = streamPublisher;
            _messageSubscriber = messageSubscriber;
            TOPIC = "PresetCommand.ManualControlService";
        }

        public uint Acceleration { get; private set; }
        public uint Deceleration { get; private set; }
        public uint Speed { get; private set; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _messageSubscriber.SubscribeAsync<PresetCommand>(TOPIC, message => {
                ExecuteCommand(message);
            });
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
                await _streamPublisher.PublishAsync("Preset.ManualControlService", 
                    new PresetMessage() { Category = "ManualControlService", Name = "Acceleration", 
                        CurrentValue = Acceleration, SetValue = Acceleration, DataType = typeof(UInt32).ToString() });

                await _streamPublisher.PublishAsync("Preset.ManualControlService",
                    new PresetMessage()
                    {
                        Category = "ManualControlService",
                        Name = "Deceleration",
                        CurrentValue = Deceleration,
                        SetValue = Deceleration,
                        DataType = typeof(UInt32).ToString()
                    });

                await _streamPublisher.PublishAsync("Preset.ManualControlService",
                    new PresetMessage()
                    {
                        Category = "ManualControlService",
                        Name = "Speed",
                        CurrentValue = Speed,
                        SetValue = Speed,
                        DataType = typeof(UInt32).ToString()
                    });

            }
            //            return Task.CompletedTask;
        }

        protected async void ExecuteCommand(PresetCommand Message)
        {
            string datatype = "";
            switch (Message.Name)
            {
                case "Acceleration":
                    Acceleration = ((JsonElement)Message.SetValue).GetUInt32();
                    datatype = typeof(UInt32).ToString();
                    break;
                case "Deceleration":
                    Deceleration = ((JsonElement)Message.SetValue).GetUInt32();
                    datatype = typeof(UInt32).ToString();
                    break;
                case "Speed":
                    Speed = ((JsonElement)Message.SetValue).GetUInt32();
                    datatype = typeof(UInt32).ToString();
                    break;
            }
            Console.WriteLine($"Received Preset {Message.Topic} {Message.Name} {Message.SetValue}");

            await _streamPublisher.PublishAsync("Preset."+Message.TopicName , new PresetMessage() { 
                Category = Message.Topic, 
                Name=Message.Name, 
                CurrentValue = Message.SetValue, 
                SetValue=Message.SetValue, DataType=datatype });
        }


    }
}
