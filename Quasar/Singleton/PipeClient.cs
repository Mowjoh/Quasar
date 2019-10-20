using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quasar.Singleton
{
    class PipeClient
    {
        NamedPipeClient<string> client;

        public PipeClient(string pipeName, string Args)
        {
            client = new NamedPipeClient<string>(pipeName);

            client.ServerMessage += delegate (NamedPipeConnection<string, string> connection, string message) {
                if(message.Equals("Wolcom"))
                    {
                        
                    }
            };

            client.Error += delegate (Exception exception)
            {
                Console.Error.WriteLine("ERROR: {0}", exception);
            };


            client.Start();

            startMessage(Args);

            client.Stop();
        }


        public void startMessage(string Args)
        {
            using (Mutex mutex = new System.Threading.Mutex(false, "QuasariteClient"))
            {
                mutex.WaitOne(TimeSpan.FromSeconds(2));
                client.PushMessage("Args");
                mutex.ReleaseMutex();
            }
        }


    }


}
