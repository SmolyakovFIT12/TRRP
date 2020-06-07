using RabbitMQ.Client;
//using socket_server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;
using System.Runtime.Serialization.Formatters.Binary;


namespace socket_client
{
    class SendRabbitMQ
    {
        public delegate void ReceivedEventHandler(string data);
        public event ReceivedEventHandler Received;
        public static RSACryptoServiceProvider RSAkey = new RSACryptoServiceProvider();

        public static void Send(string json)
        {
            string EncriptedJson = encryption.scramble(json, RSAkey.ExportParameters(false));
            var factory = new ConnectionFactory() { HostName = Properties.Settings.Default.HostName, UserName = Properties.Settings.Default.UserName, Password = Properties.Settings.Default.Password }; //192.168.137.20
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: Properties.Settings.Default.Queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
                    
                    var body = Encoding.ASCII.GetBytes(EncriptedJson);

                    channel.BasicPublish(exchange: "",
                                                         routingKey: Properties.Settings.Default.Queue,
                                                         basicProperties: null,
                                                         body: body);
                    Console.WriteLine("Отправил: {0}", EncriptedJson);
                }
            }
            Console.WriteLine("");
            Console.ReadLine();
        }

        public void StartReceivingRSA()
        {
            var factory = new ConnectionFactory() { HostName = Properties.Settings.Default.HostName, UserName = Properties.Settings.Default.UserName, Password = Properties.Settings.Default.Password };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: Properties.Settings.Default.RSA,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.ASCII.GetString(body);

                        Console.WriteLine("Получил RSA(public): {0}", message);
                        if (Received != null)
                        {
                            Received(message);
                        }
                        RSAkey.FromXmlString(message);

                        //string DecryptMessage = decryption.unscramble(message);
                        //Put.JsonCon(DecryptMessage);
                    };
                    channel.BasicConsume(queue: Properties.Settings.Default.RSA,
                                         autoAck: true,
                                         consumer: consumer);

                    //Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }


    }
}
