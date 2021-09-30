using NetMQ;
using NetMQ.Sockets;
using Quasar.Controls.Mod.Models;
using Quasar.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.IPC
{
    public static class QuasarDataServer
    {
        public static void RunServer()
        {
            using (var responder = new ResponseSocket())
            {
                responder.Bind("tcp://*:18128");

                while (true)
                {
                    string str = responder.ReceiveFrameString();
                    responder.SendFrame("Oukay");
                    EventSystem.Publish<QuasarDownload>(new QuasarDownload() { QuasarURL = str });
                }
            }
        }
    }
}
