using FluentFTP;
using log4net;
using log4net.Appender;
using Nett;
using Quasar.Controls.Build.Models;
using Quasar.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Quasar.Internal.Tools
{
    public static class TouchmARC
    {
        public static void CheckTouchmARC(FileWriter Writer, string WorkspaceName, bool DefaultConfig = false)
        {
            ILog log = LogManager.GetLogger("QuasarAppender");
            FileAppender appender = (FileAppender)log.Logger.Repository.GetAppenders()[0];
            appender.File = Properties.Settings.Default.DefaultDir + "\\Quasar.log";
            appender.Threshold = log4net.Core.Level.Debug;
            appender.ActivateOptions();

            if (!Writer.CheckFileExists("atmosphere\\contents\\01006A800016E000\\romfs\\skyline\\plugins\\libarcropolis.nro"))
            {
                log.Debug("Remote ARCropolis does not exist");
                sendTouchmARC(Writer, log);
                GetLocalConfig(log);
                ModifyTouchmARCConfig(WorkspaceName, DefaultConfig, log);
                sendRemoteFile(Writer, log);
            }
            else
            {
                log.Debug("Remote ARCropolis exists");
                getRemoteFile(Writer, log);
                if (!File.Exists(Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml"))
                {
                    log.Debug("Remote TOML does not exist");
                    sendTouchmARC(Writer, log);
                    GetLocalConfig(log);
                    ModifyTouchmARCConfig(WorkspaceName, DefaultConfig, log);
                    sendRemoteFile(Writer, log);
                }
                else
                {
                    log.Debug("Remote TOML exists");
                    if (LocalNewer(log))
                    {
                        log.Debug("Local TOML is newer");
                        GetLocalConfig(log);
                        sendTouchmARC(Writer, log);
                    }

                    ModifyTouchmARCConfig(WorkspaceName, DefaultConfig, log);
                    sendRemoteFile(Writer, log);

                }
            }
            
        }

        public static void ModifyTouchmARCConfig(string WorkspacePath, bool DefaultConfig, ILog log)
        {
            
            string path = Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml";
            TouchmARCConfig config = Toml.ReadFile<TouchmARCConfig>(path);
            if (DefaultConfig)
            {
                log.Debug("default arc sent");
                config.paths.arc = @"rom:/arc/" + WorkspacePath;
            }
            else
            {
                log.Debug("arc set to " + @"sd:/Quasar/" + WorkspacePath+"/arc");
                config.paths.arc = @"sd:/Quasar/" + WorkspacePath + "/arc";
            }
            

            Toml.WriteFile<TouchmARCConfig>(config, path);
        }

        public static void GetLocalConfig(ILog log)
        {
            string source = Properties.Settings.Default.DefaultDir + "\\References\\ModLoaders\\ARCropolis\\arcropolis.toml";
            string path = Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml";
            Folderino.CheckCopyFile(source, path);
            log.Debug("Copied file to " + path);
        }
        public static bool LocalNewer(ILog log)
        {
            bool test = false;

            string source = Properties.Settings.Default.DefaultDir + "\\References\\ModLoaders\\ARCropolis\\arcropolis.toml";
            string path = Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml";

            TouchmARCConfig RemoteConfig = Toml.ReadFile<TouchmARCConfig>(path);
            TouchmARCConfig LocalConfig = Toml.ReadFile<TouchmARCConfig>(source);
            string remotever = RemoteConfig.infos.version.Replace(".", "");
            string localver = LocalConfig.infos.version.Replace(".", "");
            int r = int.Parse(remotever);
            int l = int.Parse(localver);
            if (l > r)
            {
                test = true;
            }
            log.Debug("test is " + test + " local = "+localver+" remote = "+remotever);

            return test;
        }

        public static void getRemoteFile(FileWriter Writer, ILog log)
        {
            
            try
            {
                Writer.GetFile("atmosphere\\contents\\01006A800016E000\\romfs\\arcropolis.toml", Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml");
                log.Debug("got file " + Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml");
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }
        public static void sendRemoteFile(FileWriter Writer, ILog log)
        {
            try
            {
                Writer.SendFile(Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml", "atmosphere\\contents\\01006A800016E000\\romfs\\arcropolis.toml", true);
                log.Debug("sent file atmosphere\\contents\\01006A800016E000\\romfs\\arcropolis.toml");
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
            
        }
        public static void sendTouchmARC(FileWriter Writer, ILog log)
        {
            string[] Paths = new string[]
            {
                "atmosphere\\contents\\01006A800016E000\\exefs\\main.npdm",
                "atmosphere\\contents\\01006A800016E000\\exefs\\subsdk9",
                "atmosphere\\contents\\01006A800016E000\\romfs\\skyline\\plugins\\libarcropolis.nro"
            };

            foreach (string s in Paths)
            {
                try
                {
                    string source = Properties.Settings.Default.DefaultDir + "\\References\\ModLoaders\\ARCropolis\\" + s;
                    Writer.SendFile(source, s, true);
                    log.Debug("sent file " + s);
                }
                catch(Exception e)
                {
                    log.Error(e.Message);
                }
                
            }
        }
    }


    public class TouchmARCConfig
    {
        public infos infos { get; set; }
        public paths paths { get; set; }
        public misc misc { get; set; }
    
    }

    public class infos
    {
        public string version { get; set; }
    }

    public class paths
    {
        public string arc { get; set; }
        public string umm { get; set; }
    }

    public class misc
    {
        public bool debug { get; set; }
    }
}
