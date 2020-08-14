using System;

namespace DemoSocketAsync
{
    //define events here
    //clienteventargs = client connected event
    //textreceivedevent = text received event
    public class ClientEventArgs : EventArgs
    {
        public string newClient { get; set; }

        public ClientEventArgs(string _newclient)
        {
            newClient = _newclient;
        }
    }

    public class TextReceivedEvent : EventArgs
    {
        public string ClientWhoSentText { get; set; }
        public string TextReceived { get; set; }

        public TextReceivedEvent(string _clientwhosenttext, string _textreceived)
        {
            ClientWhoSentText = _clientwhosenttext;
            TextReceived = _textreceived;
        }
    }
}