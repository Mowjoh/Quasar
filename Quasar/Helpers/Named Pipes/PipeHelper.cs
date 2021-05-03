using System;
using System.Threading;
using log4net;

namespace Quasar.NamedPipes
{
    public static class PipeHelper
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
                    PipeClient.StartPipeClient("Quasarite", Args[1], log);
                }
                mt.Close();
                Environment.Exit(0);
            }
            else
            {
                log.Info("Started as a server");
                //Server
                _serverMutex = new Mutex(true, "Quasarite");
                if (Args.Length == 2)
                {
                    //Starting with arguments
                    new PipeServer("Quasarite", Args[1],log);
                }
                else
                {
                    //Starting without arguments
                    new PipeServer("Quasarite", "", log);
                }

            }

            return _serverMutex;
        }

    }
}
