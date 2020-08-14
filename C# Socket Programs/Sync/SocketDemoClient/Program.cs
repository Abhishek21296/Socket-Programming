using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketDemoClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket client = null;
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress ipAddr = null;
            try
            {
                Attribute:
                Console.WriteLine("Type valid server IP : ");
                string ip = Console.ReadLine();

                Console.WriteLine("Enter port number:");
                string port = Console.ReadLine();
                int portint = 0;

                if(!IPAddress.TryParse(ip, out ipAddr))
                {
                    Console.WriteLine("Invalid IP");
                    goto Attribute;
                }
                if (!int.TryParse(port.Trim(), out portint))
                {
                    Console.WriteLine("Invalid port number");
                    goto Attribute;
                }

                Console.WriteLine(string.Format("IPAddress: {0} - Port: {1}", ipAddr.ToString(), portint));

                client.Connect(ipAddr, portint);                                                               //blocking

                string input = string.Empty;

                while (true)
                {
                    input = Console.ReadLine();

                    if (input.Equals("<EXIT>"))
                    {
                        break;
                    }
                    client.Send(Encoding.ASCII.GetBytes(input));

                    byte[] buff = new byte[128];
                    int num = client.Receive(buff);

                    Console.WriteLine("Data received : " + Encoding.ASCII.GetString(buff, 0, num));

                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                if (client != null)
                {
                    if (client.Connected)
                    {
                        client.Shutdown(SocketShutdown.Both);
                    }
                    client.Close();
                    client.Dispose();
                }
            }

            Console.WriteLine("Press a key to exit.....");
            Console.ReadKey();
        }
    }
}
