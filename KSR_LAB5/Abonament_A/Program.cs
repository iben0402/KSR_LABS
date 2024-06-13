using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abonament_A
{
    internal class AbonamentA
    {
        public static Task Handle(ConsumeContext<Komunikaty.Komunikaty> ctx)
        {
            if (ctx.Message.number % 2 == 0&& ctx.Message.number!=0)
            {
                ctx.RespondAsync<Komunikaty.OdpA>(new Komunikaty.OdpA() { kto = " AbonamentA" });
                Console.Out.WriteLineAsync("MESSAGE DIVIDABLE BY 2!");
            }
            return Console.Out.WriteLineAsync($" RECEIVED: { ctx.Message.tekst}");
        }
 
        public static Task HndlFault(ConsumeContext< Fault < Komunikaty.OdpA> > ctx)
        {
            return Console.Out.WriteLineAsync("MESSAGE CAUSE EXCEPTION");
        }

        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("amqps://bcukmdub:pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl@roedeer.rmq.cloudamqp.com/bcukmdub"),
                h =>
                { h.Username("bcukmdub"); h.Password("pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl"); });
                sbc.ReceiveEndpoint(host, "recvqueueA", ep =>
                {
                    ep.Handler < Komunikaty.Komunikaty > (Handle);
                });
                sbc.ReceiveEndpoint(host, "recvqueueOA_error", ep =>
                {
                    ep.Handler < Fault < Komunikaty.OdpA> > (HndlFault);
                });
             });
            bus.Start();
            Console.WriteLine("Abonament A wystartował");
            Console.ReadKey();
            bus.Stop();
        }
    }
}