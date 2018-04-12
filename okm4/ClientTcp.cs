using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace okm4
{
    class ClientTcp
    {
        private string _address;
        private int _port;
        private TcpClient client = null;
        private NetworkStream stream = null;
        public ClientTcp(string address, int port)
        {
            _address = address;
            _port = port;
        }

        internal void Init()
        {
            try
            {
                client = new TcpClient(_address, _port);
                stream = client.GetStream();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        internal void Send(string stringData)
        {
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(stringData);
                stream.Write(data, 0, data.Length);
                Console.WriteLine("Sent {0} bytes to server...", data.Length);
                int bytesRecived = 0;
                int totalBytesRecived = 0;
                while (totalBytesRecived < data.Length)
                {
                    if ((bytesRecived = stream.Read(data, totalBytesRecived, data.Length - totalBytesRecived)) == 0)
                    {
                        Console.WriteLine("Connection closed");
                        break;
                    }

                    totalBytesRecived += bytesRecived;
                }
                Console.WriteLine("Received {0} bytes from server: {1}", totalBytesRecived,  Encoding.ASCII.GetString(data, 0, totalBytesRecived));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
