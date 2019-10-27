using NamedPipeWrapper;
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
        

        public PipeServer(string pipeName,ObservableCollection<string> _DLS,string Args)
        {
            
            Mutex ClientReady = new Mutex();

            var server = new NamedPipeServer<string>(pipeName);


            server.ClientConnected += delegate (NamedPipeConnection<string, string> connection)
            {
                connection.PushMessage("Wolcom");
                
            };

            server.ClientDisconnected += delegate (NamedPipeConnection<string, string> connection)
            {
            };

            server.ClientMessage += delegate (NamedPipeConnection<string, string> connection, string message)
            {
                _DLS.Clear();
                _DLS.Add(message);
                connection.PushMessage("Oukay");
            };

            server.Error += delegate (Exception exception)
            {
            };

            server.Start();
            if(Args != "")
            {
                _DLS.Clear();
                _DLS.Add(Args);
            }

            Console.WriteLine("Server Started");
        }
    }
}
