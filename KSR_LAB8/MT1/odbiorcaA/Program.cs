﻿using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace odbiorcaA
{
    internal class Program
    {
        public static Task Handle(ConsumeContext<Komunikaty.Komunikat1> ctx)
        {
            // ZADANIE 3
            foreach (var hdr in ctx.Headers.GetAll())
            {
                Console.WriteLine("{0}: {1}", hdr.Key, hdr.Value);
            }
            return Console.Out.WriteLineAsync($"received: {ctx.Message.tekst}");

        }
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Thread.Sleep(2000);
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new
                    Uri("amqps://bcukmdub:pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl@roedeer.rmq.cloudamqp.com/bcukmdub"), h => {
                        h.Username("bcukmdub");
                        h.Password("pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl");
                    });

                sbc.ReceiveEndpoint(host, "recvqueue", ep => {
                    ep.Handler<Komunikaty.Komunikat1>(Handle);
                });
            });
            bus.Start();
            Console.WriteLine("ODBIORCA A: wystartował"); Console.ReadKey();
            bus.Stop();
        }
    }
}
