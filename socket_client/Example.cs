using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketExample
{
    public class SocketClient
    {
        public Socket socket;
        private int bufferSize = 10 * 1024 * 1024;

        public delegate void ReceivedEventHandler(string data);
        public event ReceivedEventHandler Received;     

        public delegate void DisconnectedEventHandler();
        public event DisconnectedEventHandler Disconnected;

        public SocketClient(Socket _socket)
        {
            socket = _socket;
            startReceiving();
        }

        public SocketClient(string host, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            while (!socket.Connected)
            {
                try
                {
                    Console.WriteLine("Попытка подключиться к серверу");
                    socket.Connect(new IPEndPoint(IPAddress.Parse(host), 80));
                    Console.WriteLine("Соединение с сервером установлено");
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Не удалось подключиться к серверу " + e.Message);
                    Console.WriteLine("Повторная попытка через 10 секунд");
                }
                Thread.Sleep(10000);
            }
            startReceiving();
        }

        public void Send(string data)
        {

            socket.Send(Encoding.UTF8.GetBytes(data));
        }

        private void startReceiving()
        {
            try
            {
                byte[] buffer = new byte[bufferSize];
                socket.BeginReceive(buffer, 0, bufferSize, SocketFlags.None, (IAsyncResult AR) => { receiveCallback(AR, buffer, socket); }, null);
            }
            catch { }
        }

        private void receiveCallback(IAsyncResult AR, byte[] buffer, Socket receiveSocket)
        {
            try
            {
                int bytesReceived = receiveSocket.EndReceive(AR);
                if (bytesReceived > 0)
                {
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                    if (Received != null)
                    {
                        Received(data);
                    }
                    startReceiving();
                }
                else
                {
                    Disconnect();
                }
            }
            catch
            {
                if (!socket.Connected)
                {
                    Disconnect();
                }
                else
                {
                    startReceiving();
                }
            }
        }

        public void Disconnect()
        {
            socket.Shutdown(SocketShutdown.Both);
            if (Disconnected != null)
            {
                Disconnected();
            }
        }
    }
}

