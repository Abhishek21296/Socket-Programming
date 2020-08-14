using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DemoSocketAsync
{
    public class DemoSocketClient
    {
        IPAddress mServerIp;
        int mServerPort;

        TcpClient mclient;

        public EventHandler<TextReceivedEvent> Received;

        public DemoSocketClient()
        {
            mclient = null;
            mServerIp = null;
            mServerPort = -1;
        }

        public IPAddress ServerIP
        {
            get
            {
                return mServerIp;
            }
        }

        public int ServerPort
        {
            get
            {
                return mServerPort;
            }
        }

        public bool SetServerIPAddress(string _IPAddressServer)
        {
            IPAddress ipaddr = null;

            if(!IPAddress.TryParse(_IPAddressServer, out ipaddr))
            {
                Console.WriteLine("Invalid server IP required");
                return false;
            }

            mServerIp = ipaddr;
            return true;
        }

        protected virtual void OnReceived(TextReceivedEvent t)
        {
            EventHandler<TextReceivedEvent> handler = Received;
            if(handler != null)
            {
                handler(this, t);
            }
        }

        public async Task SendToServer(string msg)
        {
            if(string.IsNullOrEmpty(msg))
            {
                Console.WriteLine("Emplty string supplied to send.");
                return;
            }
            if(mclient != null)
            {
                if(mclient.Connected)
                {
                    StreamWriter clientStream = new StreamWriter(mclient.GetStream());
                    clientStream.AutoFlush = true;

                    await clientStream.WriteAsync(msg);
                }
            }
        }

        public void CloseAndDisconnect()
        {
            if(mclient != null)
            {
                if(mclient.Connected)
                {
                    mclient.Close();
                }
            }
        }

        public bool SetPort(string _ServerPort)
        {
            int portNumber = 0;

            if (!int.TryParse(_ServerPort, out portNumber))
            {
                Console.WriteLine("Invalid poer number supplied, return.");
                return false;
            }

            if(portNumber <= 0 || portNumber > 65535)
            {
                Console.WriteLine("Port number must be between 0 and 65535.");
                return false;
            }

            mServerPort = portNumber;
            return true;
        }

        public async Task ConnectToServer()
        {
            if(mclient == null)
            {
                mclient = new TcpClient();
            }

            try
            {
                await mclient.ConnectAsync(mServerIp, mServerPort);
                Console.WriteLine("Connected to IP/Port: {0}/{1}", mServerIp, mServerPort);

                ReadDataAsync(mclient);
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
        }

        private async Task ReadDataAsync(TcpClient mclient)
        {
            try
            {
                StreamReader clientStream = new StreamReader(mclient.GetStream());

                char[] buff = new char[64];
                int readBytes = 0;

                while(true)
                {
                    readBytes = await clientStream.ReadAsync(buff, 0, buff.Length);
                    if(readBytes <= 0)
                    {
                        Console.WriteLine("Disconnected from server");
                        mclient.Close();
                        break;
                    }

                    Console.WriteLine("Bytes: {0}, Message: {1}", readBytes, new string(buff));
                    OnReceived(new TextReceivedEvent(mclient.Client.RemoteEndPoint.ToString(), new string(buff)));
                    Array.Clear(buff, 0, buff.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
