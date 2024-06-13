using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
namespace Odbiorca2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            System.Threading.Thread.Sleep(1000);
            var factory = new ConnectionFactory()
            {
                UserName = "bcukmdub",
                Password = "pjd1byEZtSwPDLt7CyfVwbdhfHGV0THl",
                HostName = "roedeer.rmq.cloudamqp.com",
                VirtualHost = "bcukmdub",
                Port = 5672
            };

            Console.WriteLine("Odbiorca2: URUCHOMIONO");

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(
                    queue: queueName,
                    exchange: "topic_exchange",
                    routingKey: "*.xyz"
                );

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    string header1 = "unknown";
                    string header2 = "unknown";

                    if (ea.BasicProperties.Headers != null)
                    {
                        if (ea.BasicProperties.Headers.ContainsKey("header1"))
                        {
                            var rawHeader1 = ea.BasicProperties.Headers["header1"];
                            header1 = Encoding.UTF8.GetString((byte[])rawHeader1);
                        }
                        if (ea.BasicProperties.Headers.ContainsKey("header2"))
                        {
                            var rawHeader2 = ea.BasicProperties.Headers["header2"];
                            header2 = Encoding.UTF8.GetString((byte[])rawHeader2);
                        }
                    }


                    Console.WriteLine($"Odbiorca2: OTRZYMOANO WIADOMOŚĆ: {message}");
                    Console.WriteLine($"Nagłówki: header1 = {header1}, header2 = {header2}");

                    // ZADANIE 6
                    if (ea.BasicProperties.ReplyTo != null)
                    {
                        var replyProperties = channel.CreateBasicProperties();
                        replyProperties.CorrelationId = ea.BasicProperties.CorrelationId;
                        var responseMessage = $"ODBIORCA 2 ODPOWIADA NA WIADOMOSC: '{message}'";
                        var responseBody = Encoding.UTF8.GetBytes(responseMessage);
                        channel.BasicPublish(
                            exchange: "",
                            routingKey: ea.BasicProperties.ReplyTo,
                            basicProperties: replyProperties,
                            body: responseBody
                        );
                        Console.WriteLine("ODPOWIEDŹ WYSŁANA");
                    }


                    channel.BasicAck(ea.DeliveryTag, false);
                };
                channel.BasicConsume(
                    queue: queueName,
                    autoAck: false,
                    consumer: consumer
                );

                Console.WriteLine("Naciśnij Enter, aby zakończyć...");
                Console.ReadLine();
            }
        }
    }
}
