using GreenPipes;
using Komunikaty;
using MassTransit;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wydawca_188593
{
    internal class Wydawca
    {
        static Random rnd = new Random();
        class HandlerClass : IConsumer<Komunikaty.Ustaw>
        {
            public bool active = false;
            public Task Consume(ConsumeContext<Komunikaty.Ustaw> context)
            {
                this.active = context.Message.dziala;
                return Console.Out.WriteLineAsync("ACTIVATION: " + this.active);
            }
        }
        public static Task HandleA(ConsumeContext<Komunikaty.OdpB> ctx)
        {
            if (rnd.Next(0, 3) == 0)
            {
                throw new Exception();
            }
            return Console.Out.WriteLineAsync("A: RESPONSE FROM: " + ctx.Message.kto);
        }
        public static Task HandleB(ConsumeContext<Komunikaty.OdpB> ctx)
        {
            if (rnd.Next(0, 3) == 0)
            {
                throw new Exception();
            }
            return Console.Out.WriteLineAsync("B: RESPONSE FROM" + ctx.Message.kto);
        }
        static void Main(string[] args)
        {
            var inst = new HandlerClass();
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("amqps://bcukmdub:pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl@roedeer.rmq.cloudamqp.com/bcukmdub"),
                h =>
                { h.Username("bcukmdub"); h.Password("pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl"); });

                sbc.ReceiveEndpoint(host, "recvqueueW", ep =>
                {
                    ep.Instance(inst);
                });

                sbc.ReceiveEndpoint(host, "recvqueueOA", ep =>
                {
                    ep.Handler<Komunikaty.OdpB>(HandleA);
                    ep.UseRetry(r => r.Immediate(5));
                });
                sbc.ReceiveEndpoint(host, "recvqueueOB", ep =>
                {
                    ep.Handler<Komunikaty.OdpB>(HandleB);
                    ep.UseRetry(r => r.Immediate(5));
                });

            });
            bus.Start();
            Console.WriteLine("Wydawca wystartował");
            int index = 0;
            while (true)
            {
                if (inst.active)
                {
                    bus.Publish(new Komunikaty.Komunikaty() { tekst = "MSG with number:" + index, number = index });
                    Console.WriteLine("MESSAGE SENT. ID: " + index);
                    index++;
                }
                System.Threading.Thread.Sleep(1000);
            }
            bus.Stop();
        }
    }
}