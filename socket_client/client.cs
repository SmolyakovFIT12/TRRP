using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using SocketExample;
using RabbitMQ.Client;
using System.Security.Cryptography;
//using socket_server;

namespace socket_client
{
    class client
    {
        static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    try
                    {
                        ConnectToServer();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }             
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message.ToString(), ex.Source.ToString());
            }
            Console.ReadKey();
        }

        public static string GetJson()
        {
                PersonsData data = new PersonsData();
                data = Get.GetPersonsData();
                string json = JsonConvert.SerializeObject(data);

                return json;   
        }
        
        static void ConnectToServer()
        {

            SocketClient client = new SocketClient(Properties.Settings.Default.IP, Properties.Settings.Default.Port);
            SendRabbitMQ mq = new SendRabbitMQ();
            mq.StartReceivingRSA();
            client.Received += ReceivedHandler;
            string message = "", message1 = "";
            while (true)
            {

                Console.WriteLine("Выберите действие:\n1. Переслать данные по сокетам\n2. Переслать данные с помощью RabbitMQ\n3. Разорвать соединение");
                message1 = Console.ReadLine();
                if(message1 == "1")
                {
                    message = GetJson();
                    
                       
                    client.Send(message);
                }

                if (message1 == "2")
                {
                    message = GetJson();
                    
                    SendRabbitMQ.Send(message);
                }

                if (message1 == "3")
                {
                    client.Disconnect();
                    break;
                }
            }
        }

        static void ReceivedHandler(string data)
        {
            Console.WriteLine(data);
        }
    }
}


