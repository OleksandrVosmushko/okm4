using System;
using System.Collections;
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
        private Socket server = null;
        private const int Backlog = 5;
        public ServerTcp(int port)
        {
            _port = port;
        }

        internal void Init()
        {
            try
            {
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        internal async Task Listen()
        {
            try
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(IPAddress.Any, _port));
                server.Listen(Backlog);

            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);                   
            }
            byte [] recive = new byte[32];
            int bytesRecived;
            while (true)
            {
                Socket client = null;
                try
                {
                        client = server.Accept();
                        IPEndPoint localEp = (IPEndPoint) ((Socket) server).LocalEndPoint;
                        Console.Write("cur port " + localEp.Port);
                        int totalEcho = 0;
                        while ((bytesRecived = client.Receive(recive, 0, recive.Length, SocketFlags.None)) > 0)
                        {
                            client.Send(recive, 0, bytesRecived, SocketFlags.None);
                            totalEcho += bytesRecived;
                        }
                        Console.WriteLine("Echoed",totalEcho);
                        client.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    client?.Close();
                }  
            }
        } 
    }
}
