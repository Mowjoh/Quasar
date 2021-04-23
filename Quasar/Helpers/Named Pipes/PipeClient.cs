using log4net;
using NamedPipeWrapper;
using System;

namespace Quasar.NamedPipes
{
    static class PipeClient
    {
        
        /// <summary>
        /// Starts the Pipe Client
        /// </summary>
        /// <param name="_PipeName"></param>
        /// <param name="_Arguments"></param>
        /// <param name="log"></param>
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

        /// <summary>
        /// Sends a message through the pipe
        /// </summary>
        /// <param name="_Arguments"></param>
        /// <param name="_PipeClient"></param>
        public static void startMessage(string _Arguments, NamedPipeClient<string> _PipeClient)
        {
            _PipeClient.PushMessage(_Arguments);
        }


    }


}
