using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abonament_B
{
    internal class AbonamentB
    {
        public static Task Handle(ConsumeContext<Komunikaty.Komunikaty> ctx)
        {
            if (ctx.Message.number % 3 == 0 && ctx.Message.number != 0)
            {
                ctx.RespondAsync<Komunikaty.OdpB>(new Komunikaty.OdpB() { kto = " AbonamentB" });
                Console.Out.WriteLineAsync("MESSAGE DIVIDABLE BY 3!");
            }
            return Console.Out.WriteLineAsync($" RECEIVED: {ctx.Message.tekst}");
        }

        public static Task HndlFault(ConsumeContext<Fault<Komunikaty.OdpB>> ctx)
        {
            return Console.Out.WriteLineAsync("MESSAGE CAUSED EXCEPTION");
        }

        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("amqps://bcukmdub:pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl@roedeer.rmq.cloudamqp.com/bcukmdub"),
                h =>
                { h.Username("bcukmdub"); h.Password("pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl"); });
                sbc.ReceiveEndpoint(host, "recvqueueB", ep =>
                {
                    ep.Handler<Komunikaty.Komunikaty>(Handle);
                });
                sbc.ReceiveEndpoint(host, "recvqueueOB_error", ep =>
                {
                    ep.Handler<Fault<Komunikaty.OdpB>>(HndlFault);
                });
            });
            bus.Start();
            Console.WriteLine("Abonament B wystartował");
            Console.ReadKey();
            bus.Stop();
        }
    }
}