using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace okm4
{
    class ServerTcp
    {
        private object locker = new object();
        private const int BufSize = 32;
        private int _port;
        private Socket server = null;
        private const int Backlog = 5;
        private Dictionary<string, string> clientsNames = new Dictionary<string, string>();
        private bool toResponce = true;
        List<Socket> socketClients = new List<Socket>();
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
            //byte [] recive = new byte[32];
            //int bytesRecived;
            while (true)
            {
                Socket client = null;
                try
                {
                    client = server.Accept();
                    socketClients.Add(client);
                    HandleClient(client);
                    // client 
                    //IPEndPoint localEp = (IPEndPoint) ((Socket) server).LocalEndPoint;
                    //Console.Write("cur port " + localEp.Port);
                    //int totalEcho = 0;
                    //while ((bytesRecived = client.Receive(recive, 0, recive.Length, SocketFlags.None)) > 0)
                    //{
                    //    var s = Encoding.ASCII.GetString(recive, 0, recive.Length);
                    //    var a = (IPEndPoint)client.RemoteEndPoint;


                    //    var responceBytes = HandleInput(s, a.Address);
                    //    if (toResponce)
                    //        client.Send(responceBytes, 0, responceBytes.Length, SocketFlags.None);

                    //    totalEcho += bytesRecived;
                    //}
                    //Console.WriteLine("Echoed",totalEcho);
                    //client.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    client?.Close();
                }  
            }
        }

        private async Task HandleClient(Socket client)
        {
            await  Task.Factory.StartNew(()=>{
                try
                {
                    byte[] recive = new byte[32];
                    int bytesRecived;
                    IPEndPoint localEp = (IPEndPoint)((Socket)server).LocalEndPoint;
                    Console.Write("cur port " + localEp.Port);
                    int totalEcho = 0;
                    while ((bytesRecived = client.Receive(recive, 0, recive.Length, SocketFlags.None)) > 0)
                    {
                        var s = Encoding.ASCII.GetString(recive, 0, bytesRecived);
                        var a = (IPEndPoint)client.RemoteEndPoint;
                        var responceString = HandleInput(s, a);
                        var responceBytes = Encoding.ASCII.GetBytes(responceString);
                        if (toResponce && responceString != " ")
                            client.Send(responceBytes, 0, responceBytes.Length, SocketFlags.None);
                        totalEcho += bytesRecived;
                    }
                    Console.WriteLine("Echoed", totalEcho);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    client?.Close();
                }
            });
            
          
        }

        string HandleInput(string input, IPEndPoint address)
        {
            string responce = String.Copy(input);
            var inputs = input.Split();
            var command = inputs[0].ToLower();
            inputs = inputs.Skip(1).ToArray();

            if (command == "name")
            {
                clientsNames[address.ToString()] = String.Join("",inputs);
                responce = "Your name: " + String.Join("", inputs);
            }
            else if (command == "stat")
            {
                responce = "Clients: ";
                foreach (var names in clientsNames)
                {
                    responce += names.Value + " ";
                }
            } 
            else if (command == "clos")
            {
                Environment.Exit(0);
            }
            else if (command == "pare")
            {
                toResponce = !toResponce;
            }
            else if (command == "kill")
            {
                foreach (var i in socketClients.ToList())
                {
                    if (clientsNames[((IPEndPoint) i.RemoteEndPoint).ToString()] == String.Join("", inputs))
                    {

                        clientsNames.Remove(((IPEndPoint) i.RemoteEndPoint).ToString());
                        i.Close();
                        socketClients.Remove(i);
                    }
                }
            }
            else if (command == "mesg")
            {
                lock (locker)
                {
                    foreach (var i in clientsNames)
                    {
                        if (i.Value == inputs[0])
                        {
                            foreach (var sock in socketClients)
                            {
                                if (((IPEndPoint)sock.RemoteEndPoint).ToString() == i.Key)
                                {

                                    var message = inputs.Skip(1).ToArray();

                                    var responceBytes = Encoding.ASCII.GetBytes(String.Join(" ", message));

                                    sock.Send(responceBytes, 0, responceBytes.Length, SocketFlags.None);
                                    responce = "";
                                }
                            }
                        }
                    }
                }
            }
            else if (command == "bcst")
            {
                foreach (var sock in socketClients)
                {
                    var message = inputs.ToArray();
                    var responceBytes = Encoding.ASCII.GetBytes(String.Join(" ", message));
                    sock.Send(responceBytes, 0, responceBytes.Length, SocketFlags.None);
                }
                responce = " ";
            }

            return responce;
        }
    }
}
