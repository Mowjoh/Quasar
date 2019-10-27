using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quasar.Singleton
{
    class PipeClient
    {
        NamedPipeClient<string> client;

        public PipeClient(string pipeName, string Args)
        {
            client = new NamedPipeClient<string>(pipeName);
            bool Disconnect = false;
            
            //When the servers sends a hello message, send back arguments
            client.ServerMessage += delegate (NamedPipeConnection<string, string> connection, string message) {
                //Server sends a hello
                if(message.Equals("Wolcom"))
                {
                    startMessage(Args);
                }

                //Server received the message
                if (message.Equals("Oukay"))
                {
                    Disconnect = true;
                }
            };

            client.Error += delegate (Exception exception)
            {
                Console.Error.WriteLine("ERROR: {0}", exception);
            };


            client.Start();

            //Waiting for operations to complete
            while (!Disconnect)
            {

            }

            client.Stop();
        }


        public void startMessage(string Args)
        {
            client.PushMessage(Args);
        }


    }


}
