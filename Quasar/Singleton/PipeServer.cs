using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Singleton
{
    class PipeServer
    {
        public PipeServer(string pipeName)
        {
            var server = new NamedPipeServer<NPMessage>(pipeName);
            server.ClientConnected += OnClientConnected;
            server.ClientDisconnected += OnClientDisconnected;
            server.ClientMessage += OnClientMessage;
            server.Error += OnError;
            server.Start();

            server.Stop();
        }

        private void OnClientConnected(NamedPipeConnection<NPMessage, NPMessage> connection)
        {
            Console.WriteLine("Client {0} is now connected!", connection.Id);
            connection.PushMessage(new NPMessage
            {
                Text = "Welcome!"
            });
        }

        private void OnClientDisconnected(NamedPipeConnection<NPMessage, NPMessage> connection)
        {
            Console.WriteLine("Client {0} disconnected", connection.Id);
        }

        private void OnClientMessage(NamedPipeConnection<NPMessage, NPMessage> connection, NPMessage message)
        {
            Console.WriteLine("Client {0} says: {1}", connection.Id, message);
        }

        private void OnError(Exception exception)
        {
            Console.Error.WriteLine("ERROR: {0}", exception);
        }
    }
}
