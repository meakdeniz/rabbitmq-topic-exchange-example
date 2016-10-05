using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;


namespace Varsis
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("VARSİS");
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("abis", "topic", true);
                channel.QueueDeclare("varsis", true, false, false, null);
                channel.QueueBind("varsis", "abis", "varsis");

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                Console.WriteLine(" Waiting for messages.");
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" Received {0}", message);

                    int dots = message.Split('.').Length - 1;
                    Thread.Sleep(dots * 1000);

                    Console.WriteLine(" Done");

                    channel.BasicAck(ea.DeliveryTag, false);
                };
                channel.BasicConsume("varsis", false, consumer);
                
                Console.ReadLine();
            }

        }
    }
}