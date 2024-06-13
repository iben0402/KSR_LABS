using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kontroller
{
    internal class Kontroller
    {
        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("amqps://bcukmdub:pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl@roedeer.rmq.cloudamqp.com/bcukmdub"),
                h =>
                { h.Username("bcukmdub"); h.Password("pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl"); });
            });
            bus.Start();
            Console.WriteLine("Kontroller wystartował");
            var tsk = bus.GetSendEndpoint(new Uri("amqps://bcukmdub:pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl@roedeer.rmq.cloudamqp.com/bcukmdub/recvqueueW"));
            tsk.Wait(); var sendEp = tsk.Result;

            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey();
                switch (cki.Key)
                {
                    case ConsoleKey.S:
                        sendEp.Send < Komunikaty.Ustaw > (new Komunikaty.Ustaw() { dziala = true });
                        Console.WriteLine("Ustaw = true");
                        break;
                    case ConsoleKey.T:
                        sendEp.Send < Komunikaty.Ustaw > (new Komunikaty.Ustaw() { dziala = false });
                        Console.WriteLine("Ustaw = false");
                        break;
                }

            } while (cki.Key != ConsoleKey.Escape);

            bus.Stop();
        }
    }
}