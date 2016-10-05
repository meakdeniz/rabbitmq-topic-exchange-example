using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Aves
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("AVESİS");
            ConnectionFactory factory = new ConnectionFactory { HostName = "localhost" };

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("abis", "topic",true);
                    channel.QueueDeclare("aves", true, false, false, null);
                    channel.QueueBind("aves", "abis", "aves");


                    Console.WriteLine("Waiting for messages");
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" Received {0}", message);
                        channel.BasicAck(ea.DeliveryTag, false);

                    };
                    channel.BasicConsume("aves", false, consumer);
                    Console.ReadLine();
                }
            }
        }
    }
}
