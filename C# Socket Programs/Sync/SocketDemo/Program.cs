using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // ipv4, stream, tcp protocol
            IPAddress ipAddr = IPAddress.Any;       //listen on any available ip
            IPEndPoint ipEnd = new IPEndPoint(ipAddr, 23000);

            try
            {
                listener.Bind(ipEnd);

                listener.Listen(5);

                Console.WriteLine("About to accept incoming connection");

                Socket client = listener.Accept();  //synchronous blocking operation

                Console.WriteLine("Client connected. " + client.ToString() + " - IP End Point : " + client.RemoteEndPoint.ToString());

                byte[] buff = new byte[128];

                int number = 0;

                while (true)
                {
                    number = client.Receive(buff);

                    Console.WriteLine("number of received bytes : " + number);
                    string received = Encoding.ASCII.GetString(buff, 0, number);

                    Console.WriteLine("Data sent by client is : " + received);

                    //client.Send(Encoding.ASCII.GetBytes(new char[] { 'a', 'f' }));
                    client.Send(buff);

                    Array.Clear(buff, 0, buff.Length);
                    number = 0;

                    if (received == "x")
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
