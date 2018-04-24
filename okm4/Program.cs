using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace okm4
{
    static class ext
    {
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(
                s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cancellationToken);
            return await task;
        }
    }
    class Program
    {
        private int _timeout = 5000;
        private string _address = "";
        private int _port = 8001;
        private bool toStartUdpBroadcast = false;

        
        static void Main(string[] args)
        {
            //CancellationTokenSource sorce = new CancellationTokenSource();
            //sorce.CancelAfter(TimeSpan.FromSeconds(1));
            //try
            //{
            //    var t = Task<int>.Factory.StartNew(() =>
            //    {
            //        Thread.Sleep(10000);
            //        return 1;
            //    });
            //    var withCancellation = t.WithCancellation(sorce.Token);
            //    t.Wait();
            //    Console.WriteLine(t.Result);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);

            //}

            Logger.Log("123");

            ClientTcp clientTcp;
            ServerTcp serverTcp;

            Program p = new Program();
            ClientUdp client = new ClientUdp(p._address, p._port);
            try
            {
                //client.EstablishAsync().TimeoutAfter(TimeSpan.FromMilliseconds(p._timeout)).Wait();
                var task = client.EstablishAsync(p._timeout);
                task.Wait();
                p._address = client.ServerAddress;
            }
            catch (AggregateException e) 
            {
                e.Handle(x =>
                {
                    if (x is SocketException)
                    {
                        Console.WriteLine(((SocketException)x).ErrorCode);
                        Console.WriteLine(x);

                        if (((SocketException)x).ErrorCode != 10048)
                            p.toStartUdpBroadcast = true;
                    }
                    return true;
                });
               

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
                clientTcp.Recive();
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
