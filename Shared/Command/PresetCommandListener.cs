using Shared.Command.Preset;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Shared.Data;
using Shared.Messaging;
using Shared.Streaming;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Threading;

namespace Shared.Command
{
    internal class PresetCommandListener : BackgroundService
    {
        private readonly IStreamPublisher _streamPublisher;
        protected readonly IMessageSubscriber _messageSubscriber;
        private readonly string TOPIC;

        Dictionary<string, Preset.Preset> _presets;

        public PresetCommandListener(
            IMessageSubscriber messageSubscriber,
            IStreamPublisher streamPublisher,
            IConfiguration configuration)
        {
            _streamPublisher = streamPublisher;
            _messageSubscriber = messageSubscriber;
            _presets = new Dictionary<string, Preset.Preset>();


            var presetconfig = configuration.GetSection("preset").Get<PresetConfiguration>();// configuration.GetValue<PresetConfiguration>("preset");

            foreach (var p in presetconfig.Presets)
            {
                Preset.Preset x = null;
                switch (p.DataType)
                {
                    case "System.UInt32":
                        x = new Preset<UInt32>()
                        {
                            InitialValue = uint.Parse(p.InitialValue),
                            MinValue = uint.Parse(p.MinValue),
                            MaxValue = uint.Parse(p.MaxValue),
                            Name = p.Name,
                            Category = presetconfig.Topic

                        };
                        break;
                    default:
                        {
                            throw new NotImplementedException(p.DataType);
                        }
                }

               

                x.Initialize();
                _presets.Add(x.Name, x);
            }
            TOPIC = $"PresetCommand.{presetconfig.Topic}"; 
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _messageSubscriber.SubscribeAsync<PresetCommand>(TOPIC, message => {
                ExecuteCommand(message);
            });
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000);

                foreach (var p in _presets.Values)
                {
                    await _streamPublisher.PublishAsync($"Preset.{p.Category}",
                        p.GetPresetMessage());
                }
            }
        }

        protected async void ExecuteCommand(PresetCommand Message)
        {
            string datatype = "";

            var preset = _presets[Message.Name];

            switch (preset.DataType)
            {
                case "System.UInt32":
                    ((Preset<UInt32>)preset).SetValue(((JsonElement)Message.SetValue).GetUInt32());
                    break;
                default:
                    {
                        throw new NotImplementedException(_presets[Message.Name].DataType);
                    }
            }
            Console.WriteLine($"Received Preset {Message.Topic} {Message.Name} {Message.SetValue}");

            await _streamPublisher.PublishAsync("Preset." + preset.Category, preset.GetPresetMessage());
        }

        public Preset<T> GetPreset<T>(string PresetName) where T : struct, IComparable<T>
        {
            return ((Preset<T>)_presets[PresetName]);
        }

    }
}
