using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    internal class RealClient : Client
    {
        private TcpClient tcpClient = new TcpClient();
        public RemotePlayer player;

        public RealClient(RemotePlayer player)
        {
            this.player = player;
            tcpClient.Connect("127.0.0.1", 1337);
        }

        public void SendDataToPlayer(string data)
        {
            byte[] receive = new byte[1337];
            Stream stream = tcpClient.GetStream();
            int k = stream.Read(receive, 0, 1337);
            player.ReceiveData(GetString(receive, k));
        }

        public void SendDataToServer(string data)
        {
            byte[] send = new ASCIIEncoding().GetBytes(data);
            Stream stream = tcpClient.GetStream();
            stream.Write(send, 0, send.Length);
        }

        private string GetString(byte[] receive, int k)
        {
            char[] chars = new char[k];
            for (int i = 0; i < k; i++)
                chars[i] = Convert.ToChar(receive[i]);
            return new string(chars);
        }

    }
}
