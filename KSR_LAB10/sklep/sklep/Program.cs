using MassTransit;
using MassTransit.Saga;
using Automatonymous;
using System;
using System.Threading.Tasks;
using Komunikaty;
using System.Collections.Generic;

namespace sklep
{
    public class SklepData : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public int amount { get; set; }
        public string ClientId { get; set; }
    }


    public class SklepZamowienie : MassTransitStateMachine<SklepData>
    {

        public Event<StartZamowienia> OrderStart { get; private set; }
        public Event<Potwierdzenie> Confirmation { get; private set; }
        public Event<BrakPotwierdzenia> NoConfirmation { get; private set; }
        public Event<OdpowiedzWolne> RespondAvailable { get; private set; }
        public Event<OdpowiedzWolneNegatywna> RespondNotAvailable { get; private set; }

        public State WaitingforAvailability { get; private set; }
        public State WaitingForConfirmation { get; private set; }
        public State Finished { get; private set; }

        public SklepZamowienie()
        {
            InstanceState(x => x.CurrentState);

            Event(() => OrderStart, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => Confirmation, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => NoConfirmation, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => RespondAvailable, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => RespondNotAvailable, x => x.CorrelateById(context => context.Message.CorrelationId));

            Initially(
                When(OrderStart).Then(ctx =>
                {
                    ctx.Instance.amount = ctx.Data.amount;
                    ctx.Instance.ClientId = ctx.Data.ClientId;
                    Console.WriteLine("Otrzymano zamówienie: " + ctx.Data.amount);
                })
                    .ThenAsync(ctx => Console.Out.WriteLineAsync($"ilość={ctx.Data.amount}, Klient {ctx.Instance.ClientId}"))
                    .Respond(ctx => new PytanieoWolne { CorrelationId = ctx.Instance.CorrelationId, amount = ctx.Data.amount })
                    .TransitionTo(WaitingforAvailability)
            );

            During(WaitingforAvailability,
                When(RespondAvailable).Then(ctx => Console.WriteLine("Magazyn potwierdza dostępność: " + ctx.Data.amount))
                    .Respond(ctx => new PytanieoPotwierdzenie { CorrelationId = ctx.Instance.CorrelationId, amount = ctx.Instance.amount, ClientId = ctx.Instance.ClientId })
                    .TransitionTo(WaitingForConfirmation),
                When(RespondNotAvailable).Then(ctx => Console.WriteLine("Magazyn odmawia realizacji zamówienia (za mało)"))
                    .Respond(ctx => new OdrzucenieZamowienia { CorrelationId = ctx.Instance.CorrelationId, amount = ctx.Instance.amount, ClientId = ctx.Instance.ClientId })
                    .Finalize()
            );

            During(WaitingForConfirmation,
                When(Confirmation).Then(ctx => Console.WriteLine("Potwierdzenie od klienta " + ctx.Data.amount))
                    .Respond(ctx => new AkceptacjaZamowienia { CorrelationId = ctx.Instance.CorrelationId, amount = ctx.Instance.amount, ClientId = ctx.Instance.ClientId })
                    .Finalize(),
                When(NoConfirmation).Then(ctx => Console.WriteLine("Odmowa od klienta "))
                    .Respond(ctx => new OdrzucenieZamowienia { CorrelationId = ctx.Instance.CorrelationId, amount = ctx.Instance.amount, ClientId = ctx.Instance.ClientId })
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }
    }


    internal class Program
    {
        public static async Task Main()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("SKLEP: START");
            Console.WriteLine("----------------------------------------------------");

            var saga = new SklepZamowienie();
            var repo = new InMemorySagaRepository<SklepData>();

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("amqps://bcukmdub:pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl@roedeer.rmq.cloudamqp.com/bcukmdub"), h =>
                {
                    h.Username("bcukmdub");
                    h.Password("pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl");
                });

                cfg.ReceiveEndpoint("queue_sklep", ep =>
                {
                    ep.StateMachineSaga(saga, repo);
                });

                cfg.UseInMemoryScheduler();
            });

            await bus.StartAsync();
            try
            {
                Console.WriteLine("Sklep aktywny. \nEXIT: DOWOLNY PRZYCISK.");
                Console.ReadKey();
            }
            finally
            {
                await bus.StopAsync();
            }
        }
    }
}
