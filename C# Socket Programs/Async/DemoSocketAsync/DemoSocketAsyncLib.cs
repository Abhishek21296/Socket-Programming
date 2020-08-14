using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DemoSocketAsync
{
    public class DemoSocketAsyncLib
    {
        IPAddress mIP;
        int mPort;
        TcpListener mTCPListener;

        List<TcpClient> mClients;

        public EventHandler<ClientEventArgs> RaiseEvent; //connected event

        public EventHandler<TextReceivedEvent> Received;

        public bool KeepRunning { get; set; }

        public DemoSocketAsyncLib()
        {
            mClients = new List<TcpClient>();
        }

        protected virtual void OnReceived(TextReceivedEvent e)
        {
            EventHandler<TextReceivedEvent> handler = Received;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRaiseEvent(ClientEventArgs e)
        {
            EventHandler<ClientEventArgs> handler = RaiseEvent;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        public async void StartListeningIncoming(IPAddress ipaddr = null, int port = 23000)
        {
            if (ipaddr == null)
            {
                ipaddr = IPAddress.Any;
            }
            if (port <= 0)
            {
                port = 23000;
            }

            mIP = ipaddr;
            mPort = port;

            Debug.WriteLine("IP Address: {0} - Port: {1}", mIP, mPort);

            mTCPListener = new TcpListener(mIP, mPort);
            try
            {
                mTCPListener.Start();

                KeepRunning = true;

                while (KeepRunning)
                {
                    var returnByAccept = await mTCPListener.AcceptTcpClientAsync();

                    mClients.Add(returnByAccept);

                    Debug.WriteLine("Client Connected Successfully, count {0}: {1}", mClients.Count, returnByAccept.ToString());

                    TakeCareofTCPCleint(returnByAccept);

                    ClientEventArgs aClientConnected;
                    aClientConnected = new ClientEventArgs(returnByAccept.Client.RemoteEndPoint.ToString());

                    OnRaiseEvent(aClientConnected);
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        public void StopServer()
        {
            try
            {
                if(mTCPListener != null)
                {
                    mTCPListener.Stop();
                }

                foreach(TcpClient c in mClients)
                {
                    c.Close();
                }

                mClients.Clear();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private async void TakeCareofTCPCleint(TcpClient returnByAccept)
        {
            NetworkStream stream = null;
            StreamReader reader = null;

            try
            {
                stream = returnByAccept.GetStream();
                reader = new StreamReader(stream);

                char[] buff = new char[64];

                while (KeepRunning)
                {
                    Debug.WriteLine("***Ready to read");

                    int read = await reader.ReadAsync(buff, 0, buff.Length);

                    if(read == 0)
                    {
                        Debug.WriteLine("Socket Disconnected");
                        RemoveClient(returnByAccept);
                        break;
                    }

                    string recvdText = new string(buff);
                    Debug.WriteLine("***RECEIVED:" + recvdText);

                    OnReceived(new TextReceivedEvent(returnByAccept.Client.RemoteEndPoint.ToString(), recvdText));

                    Array.Clear(buff, 0, buff.Length);
                    
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.ToString());
                RemoveClient(returnByAccept);
            }
        }

        private void RemoveClient(TcpClient client)
        {
            if(mClients.Contains(client))
            {
                mClients.Remove(client);
                Debug.WriteLine("Client Removed Successfully, count {0}", mClients.Count);
            }
        }

        public async void SendToAll(string msg)
        {
            if(string.IsNullOrEmpty(msg))
            {
                return;
            }

            try
            {
                byte[] buff = Encoding.ASCII.GetBytes(msg);
                foreach(TcpClient c in mClients)
                {
                    c.GetStream().WriteAsync(buff, 0, buff.Length);
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
    }
}
