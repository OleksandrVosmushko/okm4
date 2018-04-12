using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace okm4
{
    class ServerTcp
    {
        private const int BufSize = 32;
        private int _port;
        private TcpListener listener = null;
        public ServerTcp(int port)
        {
            _port = port;
        }

        internal void Init()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any,_port);
                listener.Start();
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        internal async Task Listen()
        {
            var data = new byte[BufSize];
            int bytesRcvd;
            while (true)
            {
                TcpClient client = null;
                NetworkStream networkStream = null;
                try
                {
                    client = listener.AcceptTcpClient();
                    networkStream = client.GetStream();
                    Console.WriteLine("Handling client  -");
                    int totalBytesEcho = 0;
                    while ((bytesRcvd = networkStream.Read(data, 0, data.Length)) > 0)
                    {
                        networkStream.Write(data, 0, bytesRcvd);
                        totalBytesEcho += bytesRcvd;
                    }

                    Console.WriteLine("echoed {0} bytes.", totalBytesEcho);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    networkStream?.Close();
                    client?.Close();
                }
            }
        } 
    }
}
