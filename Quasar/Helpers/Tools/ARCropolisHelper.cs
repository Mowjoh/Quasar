using log4net;
using log4net.Appender;
using Nett;
using Quasar.Build.Models;
using DataModels.User;
using Quasar.Helpers.FileOperations;
using System;
using System.IO;

namespace Quasar.Helpers.Tools
{
    public static class ARCropolisHelper
    {
        /// <summary>
        /// Sends ARCropolis and an updated TOML
        /// </summary>
        /// <param name="Writer">The used Filewriter</param>
        /// <param name="WorkspaceName">Name of the workspace to send</param>
        public static void SendTouchmARC(FileWriter Writer, string WorkspaceName)
        {
            //Logging setup
            ILog log = LogManager.GetLogger("QuasarAppender");
            FileAppender appender = (FileAppender)log.Logger.Repository.GetAppenders()[0];
            appender.File = Properties.QuasarSettings.Default.DefaultDir + "\\Quasar.log";
            appender.Threshold = log4net.Core.Level.Debug;
            appender.ActivateOptions();

            //Sending install
            SendTouchmARCInstall(Writer, log);
            
            //Getting default config file
            GetLocalConfigFile(log);
            
            //Editing and sending the generated config
            ModifyTouchmARCConfig(WorkspaceName);
            SendLocalConfigFile(Writer, log);

            File.Delete(Properties.QuasarSettings.Default.DefaultDir + "\\Library\\arcropolis.toml");

        }

        /// <summary>
        /// Modifies the local ARCropolis TOML File to match the desired workspace name
        /// </summary>
        /// <param name="WorkspacePath"></param>
        public static void ModifyTouchmARCConfig(string WorkspacePath)
        {
            //Logging setup
            ILog log = LogManager.GetLogger("QuasarAppender");
            FileAppender appender = (FileAppender)log.Logger.Repository.GetAppenders()[0];
            appender.File = Properties.QuasarSettings.Default.DefaultDir + "\\Quasar.log";
            appender.Threshold = log4net.Core.Level.Debug;
            appender.ActivateOptions();

            string path = Properties.QuasarSettings.Default.DefaultDir + "\\Library\\arcropolis.toml";
            try
            {
                ARCropolisConfiguration config = Toml.ReadFile<ARCropolisConfiguration>(path);
                log.Debug("arc set to " + @"sd:/ultimate/" + WorkspacePath + "/arc");
                config.paths.arc = @"sd:/ultimate/" + WorkspacePath + "/arc";
                log.Debug("umm set to " + @"sd:/ultimate/" + WorkspacePath + "/umm");
                config.paths.umm = @"sd:/ultimate/" + WorkspacePath + "/umm";


                Toml.WriteFile<ARCropolisConfiguration>(config, path);
            }
            catch(Exception e)
            {
                log.Error(e.Message);
            }
            
        }

        /// <summary>
        /// Retreives the remote ARCropolis TOML File
        /// </summary>
        /// <param name="Writer"></param>
        /// <param name="log"></param>
        public static void GetRemoteConfigFile(FileWriter Writer, ILog log)
        {
            try
            {
                Writer.GetFile("atmosphere\\contents\\01006A800016E000\\romfs\\arcropolis.toml", Properties.QuasarSettings.Default.DefaultDir + "\\Library\\arcropolis.toml");
                log.Debug("got file " + Properties.QuasarSettings.Default.DefaultDir + "\\Library\\arcropolis.toml");
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }

        /// <summary>
        /// Retreives the default ARCropolis TOML File
        /// </summary>
        /// <param name="log"></param>
        public static void GetLocalConfigFile(ILog log)
        {
            string source = Properties.QuasarSettings.Default.DefaultDir + "\\Resources\\ModLoaders\\ARCropolis\\arcropolis.toml";
            string path = Properties.QuasarSettings.Default.DefaultDir + "\\Library\\arcropolis.toml";
            FileOperation.CheckCopyFile(source, path);
            log.Debug("Copied file to " + path);
        }

        /// <summary>
        /// Sends the local ARCropolis TOML File to the device
        /// </summary>
        /// <param name="Writer"></param>
        /// <param name="log"></param>
        public static void SendLocalConfigFile(FileWriter Writer, ILog log)
        {
            try
            {
                Writer.SendFile(Properties.QuasarSettings.Default.DefaultDir + "\\Library\\arcropolis.toml", "atmosphere\\contents\\01006A800016E000\\romfs\\arcropolis.toml");
                log.Debug("sent file atmosphere\\contents\\01006A800016E000\\romfs\\arcropolis.toml");
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
            
        }

        /// <summary>
        /// Sends ARCropolis' install files to the device
        /// </summary>
        /// <param name="Writer"></param>
        /// <param name="log"></param>
        public static void SendTouchmARCInstall(FileWriter Writer, ILog log)
        {
            string BasePath = Properties.QuasarSettings.Default.DefaultDir + "\\Resources\\ModLoaders\\ARCropolis";
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

        public static void CreateInfoFile(LibraryItem Item,string Decription, string CategoryName = "")
        {
            ARCadiaModInfo info;
            string output = String.Format(@"{0}\Library\Mods\{1}\info.toml",Properties.QuasarSettings.Default.DefaultDir, Item.Guid);
            if (!Item.ManualMod)
            {
                info = new ARCadiaModInfo()
                {
                    display_name = Item.Name,
                    description = Decription,
                    category = CategoryName,
                    version = Item.GBItem.UpdateCount.ToString()
                };
            }
            else
            {
                info = new ARCadiaModInfo()
                {
                    display_name = Item.Name,
                    description = Decription,
                    category = "Unknown",
                    version = "1.0"
                };
            }

            Toml.WriteFile<ARCadiaModInfo>(info, output);

        }

        public static void DeleteInfoFile()
        {
            if (File.Exists(String.Format(@"{0}\Library\info.toml", Properties.QuasarSettings.Default.DefaultDir)))
                File.Delete(String.Format(@"{0}\Library\info.toml", Properties.QuasarSettings.Default.DefaultDir));
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
        public string[] extra_paths { get; set; }
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


    public class ARCadiaModInfo
    {
        public string display_name { get; set; }
        public string description { get; set; }
        public string version { get; set; }
        public string category { get; set; }

        
    }
}
