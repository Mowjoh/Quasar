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
        

        public PipeServer(string pipeName)
        {
            
            Mutex ClientReady = new Mutex();

            var server = new NamedPipeServer<string>(pipeName);


            server.ClientConnected += delegate (NamedPipeConnection<string, string> connection)
            {
                Console.WriteLine("Client {0} is now connected!", connection.Id);
                connection.PushMessage("Wolcom");
                
            };

            server.ClientDisconnected += delegate (NamedPipeConnection<string, string> connection)
            {
                
                Console.WriteLine("Client {0} disconnected", connection.Id);
            };

            server.ClientMessage += delegate (NamedPipeConnection<string, string> connection, string message)
            {
                Console.WriteLine("Client {0} message received !", connection.Id);
                Console.WriteLine("Client {0} says: {1}", connection.Id, message);
                connection.PushMessage("Oukay");
            };

            server.Error += delegate (Exception exception)
            {
                Console.Error.WriteLine("ERROR: {0}", exception);
            };

            server.Start();
            Console.WriteLine("Server Started");
        }
    }
}
