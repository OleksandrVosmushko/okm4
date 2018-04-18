using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace okm4
{
    class ClientTcp
    {
        private string _address;
        private int _port;
        private string _name;
        private Socket sock = null;
        public ClientTcp(string address, int port)
        {
            _address = address;
            _port = port;
        }

        internal void Init()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        internal void Send(string stringData)
        {
            var data = Encoding.ASCII.GetBytes(stringData);
            try
            {
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream,ProtocolType.Tcp);
                sock.Connect(new IPEndPoint(Dns.Resolve(_address).AddressList[0],_port));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            int totalSend = 0;
            int totalRecive = 0;
            sock.Blocking = false;
            while (totalRecive<data.Length)
            {
                if (totalSend < data.Length)
                {
                    try
                    {
                        totalSend += sock.Send(data, totalSend, data.Length - totalSend, SocketFlags.None);
                    }
                    catch(SocketException se)
                    {
                        if (se.ErrorCode == 10035)
                        {
                            Console.WriteLine("Try again later");
                        }
                        else
                        {
                            Console.WriteLine(se.Message);
                        }

                    }
                }

                try
                {
                    int recived = 0;
                    if ((recived = sock.Receive(data, totalRecive, data.Length - totalRecive, SocketFlags.None)) == 0)
                    {
                        Console.WriteLine("Connection closed");
                        break;
                    }
                    totalRecive += recived;
                }
                catch (SocketException se)
                {
                    if (se.ErrorCode == 10035)
                    {
                        continue;
                    }
                    else
                    {
                        Console.WriteLine(se.Message);
                        break;
                    }
                }
                Thread.Sleep(50);
            }
            Console.WriteLine("Received {0} bytes from server: {1}", totalRecive, Encoding.ASCII.GetString(data, 0, totalRecive));
            sock.Close();
        }

    }
}
