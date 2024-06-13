using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace odbiorcaB
{
    internal class Program
    {
        class HandlerClass : IConsumer<Komunikaty.Komunikat1>
        {
            private int msgCount = 0;
            public Task Consume(ConsumeContext<Komunikaty.Komunikat1> ctx)
            {
                msgCount++;
                foreach (var hdr in ctx.Headers.GetAll())
                {
                    Console.WriteLine("{0}: {1}", hdr.Key, hdr.Value);
                }
                Console.WriteLine($"Recieved message count: {msgCount}");

                // ZADANIE 4
                return Console.Out.WriteLineAsync($"received: text: {ctx.Message.tekst}");

                // ZADANIE 6
                //return Console.Out.WriteLineAsync($"received: text: {ctx.Message.tekst} number: {ctx.Message.num}");


            }
        }
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Thread.Sleep(2000);
            var instance = new HandlerClass();
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc => {
                var host = sbc.Host(new
                    Uri("amqps://bcukmdub:pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl@roedeer.rmq.cloudamqp.com/bcukmdub"), h => {
                        h.Username("bcukmdub");
                        h.Password("pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl");
                    });

                sbc.ReceiveEndpoint(host, "recvqueue2", ep => {
                    ep.Instance(instance);
                });
            });
            bus.Start();
            Console.WriteLine("ODBIORCA C: wystartował"); Console.ReadKey();
            bus.Stop();
        }
    }
}
