using Quasar.NamedPipes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Deployment;
using System.Deployment.Application;
using System.Reflection;
using System.IO;
using Quasar.Internal.Tools;
using log4net;
using Quasar.Helpers.XML;
using Quasar.Data.V1;

namespace Quasar.NamedPipes
{
    public static class PipeHelper
    {
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
                    new PipeServer("Quasarite", Args[1],log);
                }
                else
                {
                    new PipeServer("Quasarite", "", log);
                }

            }

            return _serverMutex;
        }

    }
}
