using log4net;
using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quasar.NamedPipes
{
    static class PipeClient
    {
        

        public static void StartPipeClient(string _PipeName, string _Arguments, ILog log)
        {
            NamedPipeClient<string> PipeClient;
            PipeClient = new NamedPipeClient<string>(_PipeName);
            bool Disconnect = false;

            //When the servers sends a hello message, send back arguments
            PipeClient.ServerMessage += delegate (NamedPipeConnection<string, string> connection, string message) {
                //Server sends a hello
                if (message.Equals("Wolcom"))
                {
                    startMessage(_Arguments, PipeClient);
                    log.Debug("Server sent Wolcom, Sent my args");
                }

                //Server received the message
                if (message.Equals("Oukay"))
                {
                    Disconnect = true;
                    log.Debug("Server sent Oukay, Disconnecting");
                }
            };

            PipeClient.Error += delegate (Exception exception)
            {
                log.Error("ERROR: {0}", exception);
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
