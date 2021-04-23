using log4net;
using log4net.Appender;
using Nett;
using Quasar.Build.Models;
using Quasar.Helpers.FileOperations;
using System;
using System.IO;

namespace Quasar.Helpers.Tools
{
    public static class ARCropolisHelper
    {
        public static void SendTouchmARC(FileWriter Writer, string WorkspaceName, bool DefaultConfig = false,bool NativeARCFolder = false, bool ARC = false, bool UMM = false)
        {
            //Logging setup
            ILog log = LogManager.GetLogger("QuasarAppender");
            FileAppender appender = (FileAppender)log.Logger.Repository.GetAppenders()[0];
            appender.File = Properties.Settings.Default.DefaultDir + "\\Quasar.log";
            appender.Threshold = log4net.Core.Level.Debug;
            appender.ActivateOptions();

            //Sending install
            SendTouchmARCInstall(Writer, log);
            
            //Getting default config file
            GetLocalConfigFile(log);
            
            //Editing and sending the generated config
            ModifyTouchmARCConfig(WorkspaceName);
            SendLocalConfigFile(Writer, log);

        }

        public static void ModifyTouchmARCConfig(string WorkspacePath)
        {
            //Logging setup
            ILog log = LogManager.GetLogger("QuasarAppender");
            FileAppender appender = (FileAppender)log.Logger.Repository.GetAppenders()[0];
            appender.File = Properties.Settings.Default.DefaultDir + "\\Quasar.log";
            appender.Threshold = log4net.Core.Level.Debug;
            appender.ActivateOptions();

            string path = Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml";
            ARCropolisConfiguration config = Toml.ReadFile<ARCropolisConfiguration>(path);

            log.Debug("arc set to " + @"sd:/ultimate/" + WorkspacePath + "/arc");
            config.paths.arc = @"sd:/ultimate/" + WorkspacePath + "/arc";
            log.Debug("umm set to " + @"sd:/ultimate/" + WorkspacePath + "/umm");
            config.paths.umm = @"sd:/ultimate/" + WorkspacePath + "/umm";


            Toml.WriteFile<ARCropolisConfiguration>(config, path);
        }

        public static void GetRemoteConfigFile(FileWriter Writer, ILog log)
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
        public static void GetLocalConfigFile(ILog log)
        {
            string source = Properties.Settings.Default.DefaultDir + "\\Resources\\ModLoaders\\ARCropolis\\arcropolis.toml";
            string path = Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml";
            FileOperation.CheckCopyFile(source, path);
            log.Debug("Copied file to " + path);
        }
        public static void SendLocalConfigFile(FileWriter Writer, ILog log)
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
        public static void SendTouchmARCInstall(FileWriter Writer, ILog log)
        {
            string BasePath = Properties.Settings.Default.DefaultDir + "\\Resources\\ModLoaders\\ARCropolis";
            foreach (string FilePath in Directory.GetFiles(BasePath, "*",SearchOption.AllDirectories))
            {
                string destination = FilePath.Replace(BasePath + "\\", "");
                try
                {
                    if(destination != "arcropolis.toml")
                    {
                        Writer.SendFile(FilePath, destination);
                        log.Debug("sent file " + destination);
                    }
                }
                catch (Exception e)
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
        public string extra_paths { get; set; }
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
        public string region { get; set; }
    }
}
