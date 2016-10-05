using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Bapsis
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("BAPSİS");
            ConnectionFactory factory = new ConnectionFactory { HostName = "localhost" };

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("abis", "topic",true);
                    channel.QueueDeclare("bapsis", true, false, false, null);
                    channel.QueueBind("bapsis", "abis", "bapsis");


                    Console.WriteLine("Waiting for messages");
                    QueueingBasicConsumer consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume("bapsis", false, consumer); //no manuel ack false yapıldı ancak bir ack göndermedik.  bu durumda mesajlar iletilmedi olarak algılanıp, consumer her başladığında mesajları tekrar alacak
                    while (true)
                    {
                        BasicDeliverEventArgs e = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                        Console.WriteLine(Encoding.ASCII.GetString(e.Body));

                    }
                }
            }
        }
    }
}