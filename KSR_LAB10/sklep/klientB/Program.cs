using Komunikaty;
using MassTransit;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace klientB
{
    public class KlientB
    {
        private static IBusControl bus;
        private const string ClientId = "B";

        private static async void StartNewOrder()
        {
            Console.WriteLine("Podaj rozmiar zamówienia: ");
            int amount;
            while (!int.TryParse(Console.ReadLine(), out amount))
            {
                Console.WriteLine("Niepoprawna wartość (nie jest to liczba), spróbuj ponownie");
            }

            Console.WriteLine("Wysyłanie zamówienia");
            await bus.Publish(new StartZamowienia { amount = amount, CorrelationId = NewId.NextGuid(), ClientId = ClientId });
        }

        private static void CleanQueue(string queueName)
        {
            var factory = new ConnectionFactory()
            {
                UserName = "bcukmdub",
                Password = "pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl",
                HostName = "roedeer.rmq.cloudamqp.com",
                VirtualHost = "bcukmdub",
                Port = 5672
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDelete(queueName, false, false);
            }
        }

        public static async Task Main()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("KLIENT B: START");
            Console.WriteLine("----------------------------------------------------");

            CleanQueue("klient_b_queue");

            bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("amqps://bcukmdub:pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl@roedeer.rmq.cloudamqp.com/bcukmdub"), h =>
                {
                    h.Username("bcukmdub");
                    h.Password("pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl");
                });

                cfg.ReceiveEndpoint("klient_b_queue", ep =>
                {
                    ep.Handler<AkceptacjaZamowienia>(async ctx =>
                    {
                        if (ctx.Message.ClientId == ClientId)
                        {
                            Console.WriteLine("Zamówienie na ilość " + ctx.Message.amount + "zostało zaakceptowane");
                            StartNewOrder();
                        }
                    });

                    ep.Handler<OdrzucenieZamowienia>(async ctx =>
                    {
                        if (ctx.Message.ClientId == ClientId)
                        {
                            Console.WriteLine("Zamówienie na ilość " + ctx.Message.amount + " zostało odrzucone");
                            StartNewOrder();
                        }
                    });

                    ep.Handler<PytanieoPotwierdzenie>(async ctx =>
                    {
                        if (ctx.Message.ClientId == ClientId)
                        {
                            Console.WriteLine("Potwierdzasz zamówienie na " + ctx.Message.amount + " produktów");
                            Console.WriteLine("Potwierdź, by potwierdzić wpisz t, by odrzucić wpisz n: ");
                            var key = Console.ReadKey().KeyChar;

                            if (key == 't')
                            {
                                await ctx.RespondAsync(new Potwierdzenie
                                {
                                    CorrelationId = ctx.Message.CorrelationId,
                                    amount = ctx.Message.amount
                                });
                                StartNewOrder();

                            }
                            else
                            {
                                await ctx.RespondAsync(new BrakPotwierdzenia
                                {
                                    CorrelationId = ctx.Message.CorrelationId,
                                    amount = ctx.Message.amount
                                });
                                StartNewOrder();

                            }
                        }
                    });
                });
            });

            await bus.StartAsync();
            try
            {
                StartNewOrder();
                await Task.Delay(-1);
            }
            finally
            {
                await bus.StopAsync();
            }
        }
    }
}
