using NetMQ;
using NetMQ.Sockets;
using System;

namespace Helpers.IPC
{
    public static class QuasarDataClient
    {
        public static void RunClient(string _QuasarURL)
        {
            Console.WriteLine("Connecting to server");
            using (var requester = new RequestSocket())
            {
                requester.Connect("tcp://localhost:18128");

                //Sending a Quasar URL Message
                requester.SendFrame(_QuasarURL);

                //Getting aknowledgment
                string str = requester.ReceiveFrameString();
            }
        }
    }
}
