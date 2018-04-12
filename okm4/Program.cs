using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace okm4
{

    class Program
    {
        private int _timeout = 5000;
        private string _address = "";
        private int _port = 8001;
        private bool toStartUdpBroadcast = false;
   
        static void Main(string[] args)
        {  
            ClientTcp clientTcp;
            ServerTcp serverTcp;

            Program p = new Program();
            ClientUdp client = new ClientUdp(p._address, p._port);
            try
            {
                //client.EstablishAsync().TimeoutAfter(TimeSpan.FromMilliseconds(p._timeout)).Wait();
                var task  = client.EstablishAsync(p._timeout);
                task.Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                p.toStartUdpBroadcast = true;
            }

            if (p.toStartUdpBroadcast)
            {
                Console.WriteLine("become server");
                client.NotifyAsync();
                serverTcp = new ServerTcp(p._port);
                serverTcp.Init();
                serverTcp.Listen();
            }
            else
            {
                clientTcp = new ClientTcp(p._address, p._port);
                clientTcp.Init();
                while (true)
                {
                    string message = Console.ReadLine();
                    int res;
                    int.TryParse(message, out res);
                    if (res == -1)
                    {
                        return;
                    }
                    clientTcp.Send(message);
                }
            }

            Console.ReadKey();
        }

        
    }
}
