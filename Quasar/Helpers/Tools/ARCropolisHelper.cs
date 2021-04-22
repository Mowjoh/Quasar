using FluentFTP;
using log4net;
using log4net.Appender;
using Nett;
using Quasar.Build.Models;
using Quasar.FileSystem;
using Quasar.Helpers.FileOperations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Quasar.Helpers.Tools
{
    public static class ARCropolisHelper
    {
        public static void SendTouchmARC(FileWriter Writer, string WorkspaceName, bool DefaultConfig = false,bool NativeARCFolder = false, bool ARC = false, bool UMM = false)
        {
            ILog log = LogManager.GetLogger("QuasarAppender");
            FileAppender appender = (FileAppender)log.Logger.Repository.GetAppenders()[0];
            appender.File = Properties.Settings.Default.DefaultDir + "\\Quasar.log";
            appender.Threshold = log4net.Core.Level.Debug;
            appender.ActivateOptions();

            string basepath = "";
            if (typeof(SDWriter).Equals(Writer.GetType()))
            {
                SDWriter wrote = (SDWriter)Writer;
                basepath = wrote.LetterPath;
            }

            sendTouchmARC(Writer, log);
            GetLocalConfig(log);
            ModifyTouchmARCConfig(WorkspaceName, DefaultConfig, NativeARCFolder, ARC, UMM);
            sendRemoteFile(Writer, log);

        }

        public static void ModifyTouchmARCConfig(string WorkspacePath, bool DefaultConfig, bool NativeARCFolder = false, bool ARC = false, bool UMM = false)
        {
            ILog log = LogManager.GetLogger("QuasarAppender");
            FileAppender appender = (FileAppender)log.Logger.Repository.GetAppenders()[0];
            appender.File = Properties.Settings.Default.DefaultDir + "\\Quasar.log";
            appender.Threshold = log4net.Core.Level.Debug;
            appender.ActivateOptions();

            string path = Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml";
            ARCropolisConfiguration config = Toml.ReadFile<ARCropolisConfiguration>(path);
            if (DefaultConfig)
            {
                log.Debug("default arc sent");
                if (NativeARCFolder)
                {

                }
                else
                {
                    config.paths.arc = @"rom:/arc/" + WorkspacePath;
                }
                
            }
            else
            {
                log.Debug("arc set to " + @"sd:/Quasar/" + WorkspacePath+"/arc");
                if (NativeARCFolder)
                {
                    if (ARC)
                    {
                        config.paths.arc = @"rom:/arcropolis/workspaces/" + WorkspacePath + "/arc";
                    }
                    if (UMM)
                    {
                        config.paths.umm = @"rom:/arcropolis/workspaces/" + WorkspacePath + "/umm";
                    }
                }
                else
                {
                    if (ARC)
                    {
                        config.paths.arc = @"sd:/Quasar/" + WorkspacePath + "/arc";
                    }
                    if (UMM)
                    {
                        config.paths.umm = @"sd:/Quasar/" + WorkspacePath + "/Mods";
                    }
                    
                }
               
            }
            

            Toml.WriteFile<ARCropolisConfiguration>(config, path);
        }

        public static void GetLocalConfig(ILog log)
        {
            string source = Properties.Settings.Default.DefaultDir + "\\Resources\\ModLoaders\\ARCropolis\\arcropolis.toml";
            string path = Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml";
            FileOperation.CheckCopyFile(source, path);
            log.Debug("Copied file to " + path);
        }
        public static bool LocalNewer(ILog log)
        {
            bool test = false;

            string source = Properties.Settings.Default.DefaultDir + "\\Resources\\ModLoaders\\ARCropolis\\arcropolis.toml";
            string path = Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml";

            ARCropolisConfiguration RemoteConfig = Toml.ReadFile<ARCropolisConfiguration>(path);
            ARCropolisConfiguration LocalConfig = Toml.ReadFile<ARCropolisConfiguration>(source);
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
                Writer.SendFile(Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml", "atmosphere\\contents\\01006A800016E000\\romfs\\arcropolis.toml");
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
                    string source = Properties.Settings.Default.DefaultDir + "\\Resources\\ModLoaders\\ARCropolis\\" + s;
                    Writer.SendFile(source, s);
                    log.Debug("sent file " + s);
                }
                catch(Exception e)
                {
                    log.Error(e.Message);
                }
                
            }
        }
    }


    public class ARCropolisConfiguration
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
    public class updater
    {
        public string server_ip { get; set; }
        public bool beta_updates { get; set; }
    }
    public class logger
    {
        public string logger_level { get; set; }
    }
    public class misc
    {
        public bool debug { get; set; }
    }
}
