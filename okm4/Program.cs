using System;
using System.Collections.Generic;
using System.Linq;
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

            Program p = new Program();
            ClientUdp client = new ClientUdp(p._address, p._port);
            var cts = new CancellationTokenSource();

            cts.CancelAfter(TimeSpan.FromMilliseconds(p._timeout));

           
            try
            {
                //client.EstablishAsync().TimeoutAfter(TimeSpan.FromMilliseconds(p._timeout)).Wait();
                var task  = client.EstablishAsync(cts.Token);
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
            }

            Console.ReadKey();
        }

        
    }
}
