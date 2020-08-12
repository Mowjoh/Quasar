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
    static class PipeClient
    {
        

        public static void StartPipeClient(string _PipeName, string _Arguments)
        {
            NamedPipeClient<string> PipeClient;
            PipeClient = new NamedPipeClient<string>(_PipeName);
            bool Disconnect = false;
            
            //When the servers sends a hello message, send back arguments
            PipeClient.ServerMessage += delegate (NamedPipeConnection<string, string> connection, string message) {
                //Server sends a hello
                if(message.Equals("Wolcom"))
                {
                    startMessage(_Arguments, PipeClient);
                }

                //Server received the message
                if (message.Equals("Oukay"))
                {
                    Disconnect = true;
                }
            };

            PipeClient.Error += delegate (Exception exception)
            {
                Console.Error.WriteLine("ERROR: {0}", exception);
            };


            PipeClient.Start();

            //Waiting for operations to complete
            while (!Disconnect)
            {

            }

            PipeClient.Stop();
        }



        public static void startMessage(string _Arguments, NamedPipeClient<string> _PipeClient)
        {
            _PipeClient.PushMessage(_Arguments);
        }


    }


}
