using System;
using DemoSocketAsync;

namespace AsyncSocketClientDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            DemoSocketClient client = new DemoSocketClient();
            client.Received += HandleReceived;

            Console.WriteLine("***Welcome to Socket Client Starter example by Abhishek Verma***");
            Attribute:
            Console.WriteLine("Please enter a valid IP and press enter:");
            string strIP = Console.ReadLine();

            Console.WriteLine("Please enter a valid port number and press enter:");
            string srtPort = Console.ReadLine();

            if(!client.SetServerIPAddress(strIP) || !client.SetPort(srtPort))
            {
                Console.WriteLine("Invalid Inputs. press any key to try again...");
                Console.ReadKey();
                goto Attribute;
            }

            client.ConnectToServer();

            string inp = null;

            do
            {
                inp = Console.ReadLine();
                if (inp.Trim() != "<EXIT>")
                {
                    client.SendToServer(inp);
                } 
                else if(inp.Equals("<EXIT>"))
                {
                    client.CloseAndDisconnect();
                }
            } while (inp != "<EXIT>");
        }

        private static void HandleReceived(object sender, TextReceivedEvent t)
        {
            Console.WriteLine(string.Format("{0} - Received: {1}{2}", DateTime.Now, t.TextReceived, Environment.NewLine));
        }
    }
}
