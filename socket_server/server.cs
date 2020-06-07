using System;
using SocketExample;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;

namespace socket_server
{
    class Server
    {
        static RSACryptoServiceProvider rsaCrypto = new RSACryptoServiceProvider();

        static void Main(string[] args)
        {
            try
            {
                SocketServer socketServer = new SocketServer();
                socketServer.StartListening(80);
                socketServer.ClientConnected += ClientConnectedHandler;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

            try
            {
                RabbitMQ mqReceiver = new RabbitMQ();
                mqReceiver.SendRSA(rsaCrypto);
                mqReceiver.StartReceiving();
                mqReceiver.Received += ReceivedHandler;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            Console.Read();
        }

        static void ClientConnectedHandler(SocketClient client)
        {
            client.Received += ReceivedHandler;
            client.Disconnected += () => {
                Console.WriteLine("Socket Сервер: клиент отключился");
            };
        }

        static void ReceivedHandler(string data)
        {
            try
            {
                Put.JsonCon(data);
            } catch (Exception e)
            {
                Console.WriteLine("Error on data transfering: {0}", e.Message);
            }
            
            Console.WriteLine("Данные перемещены");
            Console.WriteLine("");
        }        
    }
}
