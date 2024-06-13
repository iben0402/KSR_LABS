using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Komunikaty;
using MassTransit;
using static System.Net.Mime.MediaTypeNames;

namespace wydawca
{
    internal class Program
    {
        public class Kom3 : Komunikat3
        {
            public string tekst { get; set; }
            public int num { get; set; }
        }

        public class Kom2 : Komunikat2
        {
            public int num { get; set; }
        }

        public class Kom1 : Komunikat1
        {
            public string tekst { get; set; }
        }

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc => {
                var host = sbc.Host(new
                    Uri("amqps://bcukmdub:pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl@roedeer.rmq.cloudamqp.com/bcukmdub"), h => {
                        h.Username("bcukmdub");
                        h.Password("pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl");
                    });
            });
            bus.Start();
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"WYSŁANO WIADOMOŚĆ O ID {i}");

                // ZADANIE 5
                //bus.Publish(new Kom2() { num = i }, ctx =>
                //{
                //    ctx.Headers.Set("header1", $"header1: {i}* 10 = {i * 10}");
                //    ctx.Headers.Set("header2", $"header2: {i}* 20 = {i * 20}");
                //});

                // ZADANIE 1,2,3,4
                //bus.Publish(new Kom1() { tekst=$"tekst: {i}"}, ctx =>
                //{
                //    ctx.Headers.Set("header1", $"HEADER1: {i}* 30 = {i * 30}");
                //    ctx.Headers.Set("header2", $"HEADER2: {i}* 40 = {i * 40}");
                //});

                // ZADANIE 6
                bus.Publish(new Kom3() { tekst = $"tekst: {i}", num = i }, ctx =>
                {
                    ctx.Headers.Set("header1", $"HEADER1: {i}* 30 = {i * 30}");
                    ctx.Headers.Set("header2", $"HEADER2: {i}* 40 = {i * 40}");
                });


            }
            Console.WriteLine("WYDAWCA: wystartował");
            Console.ReadKey();
            bus.Stop();
        }
    }
}
