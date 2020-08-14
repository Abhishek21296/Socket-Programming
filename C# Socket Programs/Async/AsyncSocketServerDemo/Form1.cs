using System;
using System.Windows.Forms;
using DemoSocketAsync;

namespace AsyncSocketServerDemo
{
    public partial class Form1 : Form
    {
        DemoSocketAsyncLib demoSocket;

        public Form1()
        {
            InitializeComponent();
            demoSocket = new DemoSocketAsyncLib();
            demoSocket.RaiseEvent += HandleClientConnected;
            demoSocket.Received += HandleTextReceived;
        }

        private void AcceptIncomingAsync_Click(object sender, EventArgs e)
        {
            demoSocket.StartListeningIncoming();
        }

        private void btnSendAll_Click(object sender, EventArgs e)
        {
            demoSocket.SendToAll(txtMessage.Text.Trim());
        }

        private void btnStopServer_Click(object sender, EventArgs e)
        {
            demoSocket.StopServer();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            demoSocket.StopServer();
        }

        void HandleClientConnected(object sender, ClientEventArgs ccea)
        {
            txtConsole.AppendText(string.Format("{0} - New client Connected: {1}{2}", DateTime.Now, ccea.newClient, Environment.NewLine));
        }

        void HandleTextReceived(object sender, TextReceivedEvent t)
        {
            txtConsole.AppendText(string.Format("{0} - Message Received from {3}: {1}{2}", DateTime.Now, t.TextReceived, Environment.NewLine, t.ClientWhoSentText));
        }
    }
}
