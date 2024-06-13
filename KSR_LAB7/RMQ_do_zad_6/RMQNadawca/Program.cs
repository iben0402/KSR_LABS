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
                channel.QueueDeclare(
                    queue: "message_queue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                var replyQueueName = channel.QueueDeclare().QueueName;
                var consumer = new EventingBasicConsumer(channel);

                var properties = channel.CreateBasicProperties();
                properties.ReplyTo = replyQueueName;
                var corrId = Guid.NewGuid().ToString();
                properties.CorrelationId = corrId;

                channel.BasicConsume(
                    queue: replyQueueName,
                    autoAck: true,
                    consumer: consumer
                );

                for (int i = 1; i <= 10; i++)
                {
                    string message = $"WIADOMOŚĆ {i}";
                    var body = Encoding.UTF8.GetBytes(message);

                    properties.Headers = new Dictionary<string, object>
                    {
                        { "header1", $"wartoscHeader1_{i}" },
                        { "header2", $"wartoscHeader2_{i}" }
                    };

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: "message_queue",
                        basicProperties: properties,
                        body: body
                    );
                    Console.WriteLine($"Nadawca: WIADOMOŚĆ {i}");
                    
                    //System.Threading.Thread.Sleep(1000); // ZADANIE 4
                }

                // ZADANIE 6
                Console.WriteLine("oczekiwanie na odpowiedzi\n");
                consumer.Received += (model, ea) =>
                {
                    if (ea.BasicProperties.CorrelationId == corrId)
                    {
                        var body = ea.Body.ToArray();
                        Console.WriteLine(Encoding.UTF8.GetString(body));
                    }
                };
                Console.ReadKey();
                Console.ReadKey();

            }
            Console.WriteLine("Nadawca: KONIEC WYSŁANIA WIADOMOŚCI.");
        }
    }
}