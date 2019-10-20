using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quasar.Singleton
{
    class PipeServer
    {
        Mutex serverMutex;

        public PipeServer(string pipeName)
        {
            Mutex ClientReady = new Mutex();

            var server = new NamedPipeServer<string>(pipeName);


            server.ClientConnected += delegate (NamedPipeConnection<string, string> connection)
            {
                Console.WriteLine("Client {0} is now connected!", connection.Id);
                connection.PushMessage("Wolcom");
                serverMutex.ReleaseMutex();
            };

            server.ClientDisconnected += delegate (NamedPipeConnection<string, string> connection)
            {
                serverMutex.WaitOne(TimeSpan.FromSeconds(2));
                Console.WriteLine("Client {0} disconnected", connection.Id);
            };

            server.ClientMessage += delegate (NamedPipeConnection<string, string> connection, string message)
            {
                Console.WriteLine("Client {0} message received !", connection.Id);
                Console.WriteLine("Client {0} says: {1}", connection.Id, message);
                connection.PushMessage("Response from server");
            };

            server.Error += delegate (Exception exception)
            {
                Console.Error.WriteLine("ERROR: {0}", exception);
            };

            server.Start();
            serverMutex = new Mutex(true, "QuasariteClient");
            Console.WriteLine("Server Started");


        }
    }
}
