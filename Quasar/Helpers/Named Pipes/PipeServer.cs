using log4net;
using NamedPipeWrapper;
using Quasar.Controls.Mod.Models;
using Quasar.Helpers;
using System;
using System.Threading;

namespace Quasar.NamedPipes
{
    class PipeServer
    {

        /// <summary>
        /// Pipe Server constructor
        /// </summary>
        /// <param name="pipeName"></param>
        /// <param name="Args"></param>
        /// <param name="log"></param>
        public PipeServer(string pipeName,string Args, ILog log)
        {
            
            Mutex ClientReady = new Mutex();

            var server = new NamedPipeServer<string>(pipeName);

            //Setting up connection event
            server.ClientConnected += delegate (NamedPipeConnection<string, string> connection)
            {
                connection.PushMessage("Wolcom");
                log.Debug("Client Connected, sent Wolcom");
            };

            //Setting up disconnection event
            server.ClientDisconnected += delegate (NamedPipeConnection<string, string> connection)
            {
            };

            //Setting up message event
            server.ClientMessage += delegate (NamedPipeConnection<string, string> connection, string message)
            {
                //Sending download event
                EventSystem.Publish<QuasarDownload>(new QuasarDownload() { QuasarURL = message });
                connection.PushMessage("Oukay");
                log.Debug(String.Format("Client message received :'{0}', sent Oukay", message));
            };

            //Setting up error event
            server.Error += delegate (Exception exception)
            {
            };

            //Starting server
            server.Start();
            if(Args != "")
            {
                EventSystem.Publish<QuasarDownload>(new QuasarDownload() { QuasarURL = Args });
            }
            log.Debug("Server Started");
            Console.WriteLine("Server Started");
        }
    }
}
