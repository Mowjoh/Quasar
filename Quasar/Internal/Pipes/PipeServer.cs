using log4net;
using NamedPipeWrapper;
using Quasar.Controls.Mod.Models;
using Quasar.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Quasar.Singleton
{
    class PipeServer
    {
        

        public PipeServer(string pipeName,string Args, ILog log)
        {
            
            Mutex ClientReady = new Mutex();

            var server = new NamedPipeServer<string>(pipeName);


            server.ClientConnected += delegate (NamedPipeConnection<string, string> connection)
            {
                connection.PushMessage("Wolcom");
                log.Debug("Client Connected, sent Wolcom");
            };

            server.ClientDisconnected += delegate (NamedPipeConnection<string, string> connection)
            {
            };

            server.ClientMessage += delegate (NamedPipeConnection<string, string> connection, string message)
            {
                EventSystem.Publish<QuasarDownload>(new QuasarDownload() { QuasarURL = message });
                connection.PushMessage("Oukay");
                log.Debug(String.Format("Client message received :'{0}', sent Oukay", message));
            };

            server.Error += delegate (Exception exception)
            {
            };

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
