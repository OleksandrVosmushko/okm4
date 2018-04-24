using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

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
               
                try
                {
                    sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sock.Connect(new IPEndPoint(Dns.Resolve(_address).AddressList[0], _port));
                    Console.WriteLine("input name:");
                    sock.Blocking = false;
                    string name = Console.ReadLine();
                    Send("name " + name);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        internal async Task Recive()
        {
            await Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    int totalRecive = 0;
                    var responceBytes = new byte[32];

                    try
                    {
                        int recived = 0;

                        recived = sock.Receive(responceBytes);

                        //if ((recived = sock.Receive(data, totalRecive, data.Length - totalRecive, SocketFlags.None)) == 0)
                        //{
                        //    Console.WriteLine("Connection closed");
                        //    break;
                        //}

                        totalRecive += recived;
                        if (recived > 0)
                        {
                            var responeString = Encoding.ASCII.GetString(responceBytes, 0, responceBytes.Length);
                            // Console.WriteLine("Received {0} bytes from server: {1}", totalRecive, Encoding.ASCII.GetString(responceBytes, 0, responceBytes.Length));
                            Console.WriteLine("Received {0} bytes from server: {1}", recived, responeString);
                            continue;
                        }
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

                    Thread.Sleep(500); 
                }
            });
        }

        internal void Send(string stringData)
        {
            string splice = "";
            if (stringData.Length >= 4) 
                splice = stringData.Substring(0, 4);
            if (splice.ToLower() == "quit")
            {
                sock.Close();
                Environment.Exit(0);
            }
            
            var data = Encoding.ASCII.GetBytes(stringData);
            int totalSend = 0;
            int totalRecive = 0;
            
            var responceBytes = new byte[32];
            while (totalRecive < data.Length)
            {
                if (totalSend < data.Length)
                {
                    try
                    {
                        totalSend += sock.Send(data, totalSend, data.Length - totalSend, SocketFlags.None);
                    }
                    catch (SocketException se)
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
                break;

                try
                {
                    int recived = 0;
                 
                    recived = sock.Receive(responceBytes);
                    
                    //if ((recived = sock.Receive(data, totalRecive, data.Length - totalRecive, SocketFlags.None)) == 0)
                    //{
                    //    Console.WriteLine("Connection closed");
                    //    break;
                    //}
                   
                    totalRecive += recived;
                    if (recived > 0 )
                          break;
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
                Thread.Sleep(500);
            }


           // var responeString = Encoding.ASCII.GetString(responceBytes, 0, responceBytes.Length);
           //// Console.WriteLine("Received {0} bytes from server: {1}", totalRecive, Encoding.ASCII.GetString(responceBytes, 0, responceBytes.Length));
           // Console.WriteLine("Received {0} bytes from server: {1}", totalRecive, responeString);
            //sock.Close();
        }

    }
}