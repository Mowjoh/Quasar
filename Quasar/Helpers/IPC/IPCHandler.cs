using log4net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.IPC
{
    public static class IPCHandler
    {
        /// <summary>
        /// Sets up Quasar as a client or as a Server
        /// </summary>
        /// <param name="_serverMutex"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static Mutex CheckExecuteInstance(Mutex _serverMutex, ILog log)
        {
            string[] Args = Environment.GetCommandLineArgs();
            //Checking if Quasar is running alright
            if (Mutex.TryOpenExisting("Quasarite", out Mutex mt))
            {
                log.Info("Started as a client");
                //Client
                if (Args.Length == 2)
                {
                    //Sending arguments
                    QuasarDataClient.RunClient(Args[1]);
                }
                mt.Close();
                Environment.Exit(0);
            }
            else
            {
                log.Info("Started as a server");
                //Server
                _serverMutex = new Mutex(true, "Quasarite");
                Task.Run(() => QuasarDataServer.RunServer());

            }

            return _serverMutex;
        }
    }
}
