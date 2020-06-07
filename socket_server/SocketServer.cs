using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SocketExample;
using System.Threading;

namespace socket_server
{
    class SocketServer
    {
        public Socket ListenerSocket;

        public delegate void ClientConnectedHandler(SocketClient socketInstance);
        public event ClientConnectedHandler ClientConnected;

        public SocketServer()
        {
            ListenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void StartListening(int Port)
        {
            try
            {

                ListenerSocket.Bind(new IPEndPoint(IPAddress.Parse(Properties.Settings.Default.IP), Properties.Settings.Default.Port));

                ListenerSocket.Listen(100);
                Console.WriteLine("Socket Сервер: сервер запущен на {1}:{0}", Properties.Settings.Default.Port, Properties.Settings.Default.IP);              
                ListenerSocket.BeginAccept(AcceptCallback, ListenerSocket);
            }
            catch (Exception ex)
            {
                throw new Exception("Socket Сервер: не удалось запустить сервер " + ex);
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            Console.WriteLine("Socket Сервер: клиент подключился");
            try
            {
                Socket socket = ListenerSocket.EndAccept(ar);
                SocketClient client = new SocketClient(socket);
                ClientConnected(client);
                ListenerSocket.BeginAccept(AcceptCallback, ListenerSocket);               
            }
            catch (Exception ex)
            {
                throw new Exception("Socket Сервер: ошибка при подключении клиента " + ex);
            }
        }
    }
}
