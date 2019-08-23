using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Singleton
{
    class PipeClient
    {

        public PipeClient(string pipeName)
        {
            var client = new NamedPipeClient<NPMessage>(pipeName);
            client.ServerMessage += OnServerMessage;
            client.Error += OnError;
            client.Start();

            client.Stop();
        }

        private void OnServerMessage(NamedPipeConnection<NPMessage, NPMessage> connection, NPMessage message)
        {
            Console.WriteLine("Server says: {0}", message);
        }

        private void OnError(Exception exception)
        {
            Console.Error.WriteLine("ERROR: {0}", exception);
        }
    }
}
