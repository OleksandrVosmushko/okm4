﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace okm4
{
    class ClientUdp
    {
        private string _address;
        private int _port;
        public ClientUdp(string address, int port)
        {
            _address = address;
            _port = port;
        }

        public async Task NotifyAsync()
        {
            await Task.Run(() =>
            {
                byte[] message = Encoding.ASCII.GetBytes("Welcome to server!");
                UdpClient client = new UdpClient();
                while(true)
                {
                    try
                    {
                        client.Send(message, message.Length, _address, _port);
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            });

        }

        public async Task EstablishAsync(CancellationToken token)
        {
            await Task.Factory.StartNew((() =>
            {
                UdpClient client = new UdpClient(_port);
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] recivePacket = client.Receive(ref remoteEndPoint);
                Console.WriteLine("Received {0} bytes from {1}:{2}",
                    recivePacket.Length, remoteEndPoint,
                    Encoding.ASCII.GetString(recivePacket, 0, recivePacket.Length));
                return true;
            }),token);

       }        
    }
}
