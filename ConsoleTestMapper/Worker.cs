using Microsoft.Extensions.Hosting;
using Shared.Command;
using Shared.Command.Movement;
using Shared.Data;
using Shared.Streaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTestMapper
{
    internal class Worker : BackgroundService
    {
        private readonly ICommandHandler<MotorCommand> _motor;
        private readonly ICommandHandler<SetEncoderCommand> _encoder;
        private readonly IStreamSubscriber _streamSubscriber;
        

        public Worker(ICommandHandler<MotorCommand> motor, IStreamSubscriber streamSubscriber, 
            ICommandHandler<SetEncoderCommand> encoder) {
            _motor = motor;
            _streamSubscriber= streamSubscriber;
            _encoder = encoder;
        }

        double C1=double.MinValue, C2= double.MinValue;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _streamSubscriber.SubscribeAsync<TagGroupMessage>("TAGGROUP.Encoders", (tag) =>
            {
                var CC1 = tag.Tags[0].TagValue;
                var CC2 = tag.Tags[1].TagValue;
                if (CC1!=C1 || CC2 != C2) { 
                    Console.WriteLine($"{tag.Tags[0].TagName}={tag.Tags[0].TagValue} {tag.Tags[1].TagName}={tag.Tags[1].TagValue} ");
                }
                C1 = CC1;
                C2 = CC2;
            });

            await _encoder.HandleAsync(
                new SetEncoderCommand { Count1 = 0, Count2 = 0 }
                ) ;

            await _motor.HandleAsync(
                new MotorCommand { 
                    MovementType=MovementType.Position, 
                    Acceleration=1000, 
                    Motor1Speed=2000, 
                    Motor2Speed=2000, 
                    Position1=6356, 
                    Position2=6356 
                });


        }
    }
}
