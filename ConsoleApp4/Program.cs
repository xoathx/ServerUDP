using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApp4
{
    class Program
    {

        
        static void Main(string[] args)
        {

            const string ip = "127.0.0.1";
            const int port = 8888;
            IPEndPoint udpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpSocket.Bind(udpEndPoint);

            Console.WriteLine("Waiting for connection...");
            HashSet<int> clientPorts = new HashSet<int>();
            UdpClient udpClient = new UdpClient();
            while (true)
            {
                var buffer = new byte[256];
                var size = 0;
                var data = new StringBuilder();

                EndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, 0);

                
                do
                {
                    try
                    {
                        size = udpSocket.ReceiveFrom(buffer, ref senderEndPoint);
                        Message message = JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(buffer));
                        clientPorts.Add(message.ClientPort);
                        data.Append(message.MessageText);
                    }
                    catch {}
                    
                }
                while (udpSocket.Available > 0);


                Console.WriteLine(data);
                foreach (int clientPort in clientPorts)
                {
                    try
                    {
                        EndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), clientPort);
                        
                        udpSocket.SendTo(Encoding.UTF8.GetBytes(data.ToString()), endPoint);
                        
                    }
                    catch(Exception execption)
                    {
                        Console.WriteLine(execption.Message);
                    }
                }
            }

            udpSocket.Shutdown(SocketShutdown.Both);
            udpSocket.Close();


        }

    }
}
