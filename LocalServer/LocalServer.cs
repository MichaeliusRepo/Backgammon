using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LocalServer
{
    class LocalServer
    {
        static TcpListener serverSocket;
        static Socket player1;
        static Socket player2;

        static void Main(string[] args)
        {
            Initialize();
            ExchangeData();
        }

        static void Initialize()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            serverSocket = new TcpListener(ip, 1337);
            serverSocket.Start();
            player1 = serverSocket.AcceptSocket();
            Console.WriteLine("Connection 1 accepted from " + player1.RemoteEndPoint);
            player2 = serverSocket.AcceptSocket();
            Console.WriteLine("Connection 2 accepted from " + player2.RemoteEndPoint);
        }

        static void ExchangeData()
        {
            Console.WriteLine("Begin gemu!");
            while (true)
                try
                {
                    byte[] receive = new byte[1337];
                    int k = player1.Receive(receive);
                    string data = GetString(receive, k);

                    player2.Send(new ASCIIEncoding().GetBytes(data));

                    if (data.Equals("EndGame"))
                        break;

                    // Warning: code repeat below. May trigger programmers.

                    receive = new byte[1337];
                    k = player2.Receive(receive);
                    data = GetString(receive, k);

                    player1.Send(new ASCIIEncoding().GetBytes(data));

                    if (data.Equals("EndGame"))
                        break;
                }
                catch (Exception e) { Console.WriteLine(e.StackTrace); }
            player1.Close();
            player2.Close();
            serverSocket.Stop();
        }

        static string GetString(byte[] receive, int k)
        {
            char[] chars = new char[k];
            for (int i = 0; i < k; i++)
                chars[i] = Convert.ToChar(receive[i]);
            return new string(chars);
        }

    }
}
