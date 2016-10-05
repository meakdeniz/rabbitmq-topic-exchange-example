using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using System.Collections.Generic;

namespace Abis
{
    class Program
    {
        private const string EXCHANGE_NAME = "abis";

        public static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory { HostName = "localhost" }; // connecting broker
            factory.AutomaticRecoveryEnabled = true; //network conn fail olması durumu için
            factory.TopologyRecoveryEnabled = true;

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(EXCHANGE_NAME, "topic",true); // kalıcı bir topic exchange tanımladık
                    channel.QueueDeclare("bapsis", true, false, false, null); //durable true kalıcı olması için kuyruğun
                    channel.QueueDeclare("aves", true, false, false, null);
                    channel.QueueDeclare("varsis", true, false, false, null);
                    channel.QueueBind("aves", EXCHANGE_NAME, "aves"); // exchange ve queue bağladık routing key ile
                    channel.QueueBind("bapsis", EXCHANGE_NAME, "bapsis");
                    channel.QueueBind("varsis", EXCHANGE_NAME, "varsis");

                    //channel.BasicAcks += Channel_BasicAcks;
                    //channel.ConfirmSelect();

                    var props = channel.CreateBasicProperties();
                    props.DeliveryMode = 2; // mesajların kalıcı olması için
                    //channel.TxSelect();
                    for (int i = 0; i < 10; i++)
                    {
                        byte[] payload = Encoding.ASCII.GetBytes("Avese git");
                        channel.BasicPublish(EXCHANGE_NAME, "aves", props, payload); // basit bir mesaj gönderiyoruz routing key ekleyerek
                        
                        byte[] payload2 = Encoding.ASCII.GetBytes("Bapsise git");
                        channel.BasicPublish(EXCHANGE_NAME, "bapsis",props, payload2);

                        byte[] payload3 = Encoding.ASCII.GetBytes("Varsise git");
                        channel.BasicPublish(EXCHANGE_NAME, "varsis", props, payload3);

                        byte[] payload4 = Encoding.ASCII.GetBytes("unroutable message");
                        channel.BasicPublish(EXCHANGE_NAME, "#",true, props, payload4);  // unroutable mesajları yakalamak için mandatory flag is true yaptık
                        channel.BasicReturn += Channel_BasicReturn; //handling unroutable message
                        //channel.WaitForConfirmsOrDie();

                        //channel.TxCommit();
                        Console.WriteLine("Sent Message " + i);
                        Thread.Sleep(1000);// uzun işleri simüle etmek için
                    }
                }
            }
        }

        private static void Channel_BasicReturn(object sender, RabbitMQ.Client.Events.BasicReturnEventArgs e)
        {
            Console.WriteLine(" Unrouted Messages: {0}", Encoding.UTF8.GetString(e.Body));
        }

        //private static void Channel_BasicAcks(object sender, RabbitMQ.Client.Events.BasicAckEventArgs e)
        //{
        //    IModel model = (IModel)sender;
        //    throw new NotImplementedException();
        //}
    }
}
