using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;


namespace socket_server
{
    class RabbitMQ
    {
        public delegate void ReceivedEventHandler(string data);
        public event ReceivedEventHandler Received;
        public RSACryptoServiceProvider rsaCryp;

        public void SendRSA(RSACryptoServiceProvider rsaCrypto)
        {
            rsaCryp = rsaCrypto;
            var factory = new ConnectionFactory() { HostName = Properties.Settings.Default.HostName, UserName = Properties.Settings.Default.UserName, Password = Properties.Settings.Default.Password};
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {

                    channel.QueueDeclare(queue: Properties.Settings.Default.RSA,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                    var body = Encoding.ASCII.GetBytes(rsaCrypto.ToXmlString(false));

                    channel.BasicPublish(exchange: "",
                                                         routingKey: Properties.Settings.Default.RSA,
                                                         basicProperties: null,
                                                         body: body);
                    Console.WriteLine("Отправил: {0}", Convert.ToString(rsaCrypto.ToXmlString(false)));
                }
            }
            Console.WriteLine("");
            Console.ReadLine();

        }

        public void StartReceiving()
        {
            var factory = new ConnectionFactory() { HostName = Properties.Settings.Default.HostName, UserName = Properties.Settings.Default.UserName, Password = Properties.Settings.Default.Password };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: Properties.Settings.Default.Queue,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.ASCII.GetString(body);
                      
                        Console.WriteLine("Получил: {0}", message);
                        if (Received != null)
                        {
                            Received(message);
                        }
                        string DecryptMessage = decryption.unscramble(message, rsaCryp.ExportParameters(true));
                         Put.JsonCon(DecryptMessage);

                        Console.WriteLine("Расшифрованные: {0}", DecryptMessage);
                        Console.WriteLine("Данные перемещены");
                    };
                    channel.BasicConsume(queue: Properties.Settings.Default.Queue,
                                         autoAck: true,
                                         consumer: consumer);

                    //Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
