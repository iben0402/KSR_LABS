using MassTransit;
using System;
using System.Threading.Tasks;
using Komunikaty;

namespace magazyn
{
    public class Magazyn
    {
        public static int Available = 1000;
        public static int Reserved = 0;

        public static async Task Main()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("MAGAZYN: START");
            Console.WriteLine("----------------------------------------------------");

            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("amqps://bcukmdub:pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl@roedeer.rmq.cloudamqp.com/bcukmdub"), h =>
                {
                    h.Username("bcukmdub");
                    h.Password("pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl");
                });

                cfg.ReceiveEndpoint("queue_magazyn", ep =>
                {
                    ep.Handler<PytanieoWolne>(async ctx =>
                    {
                        Console.WriteLine("Otrzymano zamówienie na ilość: " + ctx.Message.amount);

                        if (Available >= ctx.Message.amount)
                        {
                            Available -= ctx.Message.amount;
                            Reserved += ctx.Message.amount;
                            Console.WriteLine($"Dostępne: {Available}, Zarezerwowane: {Reserved}");

                            await ctx.RespondAsync(new OdpowiedzWolne
                            {
                                CorrelationId = ctx.Message.CorrelationId,
                                amount = ctx.Message.amount
                            });
                        }
                        else
                        {
                            Console.WriteLine($"Nie wystarczająco produktów. Dostępne: {Available}, Zarezerwowane: {Reserved}");

                            await ctx.RespondAsync(new OdpowiedzWolneNegatywna
                            {
                                CorrelationId = ctx.Message.CorrelationId,
                                amount = ctx.Message.amount
                            });
                        }
                    });

                    ep.Handler<AkceptacjaZamowienia>(async ctx =>
                    {
                        Console.WriteLine("Zamówienie przyjęte: " + ctx.Message.amount);
                        Reserved -= ctx.Message.amount;
                        Console.WriteLine($"Dostępne: {Available}, Zarezerwowane: {Reserved}");
                        await Task.CompletedTask;
                    });

                    ep.Handler<OdrzucenieZamowienia>(async ctx =>
                    {
                        Console.WriteLine("Zamówienie odrzucone: " + ctx.Message.amount);
                        if (Reserved >= ctx.Message.amount)
                        {
                            Available += ctx.Message.amount;
                            Reserved -= ctx.Message.amount;
                            Console.WriteLine($"Dostępne:  {Available}, Zarezerwowane: {Reserved}");
                        }
                        await Task.CompletedTask;
                    });
                });
            });

            await busControl.StartAsync();
            try
            {
                Console.WriteLine("Magazyn aktywny. \nEXIT: DOWOLNY PRZYCISK.");

                while (true)
                {
                    await Task.Delay(8000);
                    Console.WriteLine("STAN MAGAZYNU:");
                    Console.WriteLine($"Dostępne:  {Available}, Zarezerwowane: {Reserved}");
                }
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
