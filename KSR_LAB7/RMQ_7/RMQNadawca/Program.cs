using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace RMQNadawca
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            var factory = new ConnectionFactory()
            {
                UserName = "bcukmdub",
                Password = "pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl",
                HostName = "roedeer.rmq.cloudamqp.com",
                VirtualHost = "bcukmdub",
                Port = 5672
            };

            Console.WriteLine("Nadawca: URUCHOMIONO");
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(
                    exchange: "topic_exchange",
                    type: "topic",
                    durable: false,
                    autoDelete: false,
                    arguments: null
                );


                var routingKeys = new[] { "abc.def", "abc.xyz" };
                var replyQueueName = channel.QueueDeclare().QueueName;
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var response = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Nadawca: OTRZYMANA ODPOWIEDZ: {response}");
                };

                channel.BasicConsume(
                    queue: replyQueueName,
                    autoAck: true,
                    consumer: consumer
                );



                for (int i = 1; i <= 10; i++)
                {
                    var routingKey = routingKeys[i % 2];
                    string message = $"WIADOMOŚĆ {i} NA KANALE {routingKey}";
                    var body = Encoding.UTF8.GetBytes(message);
                    var properties = channel.CreateBasicProperties();
                    properties.ReplyTo = replyQueueName;
                    properties.Headers = new Dictionary<string, object>
                    {
                        { "header1", $"wartoscHeader1_{i}" },
                        { "header2", $"wartoscHeader2_{i}" }
                    };


                    channel.BasicPublish(
                        exchange: "topic_exchange",
                        routingKey: routingKey,
                        basicProperties: properties,
                        body: body
                    );
                    Console.WriteLine($"Nadawca: WIADOMOŚĆ {i}");
                    System.Threading.Thread.Sleep(1000);
                }
            }
            Console.WriteLine("Nadawca: KONIEC WYSŁANIA WIADOMOŚCI.");
            Console.ReadLine();
        }
    }
}