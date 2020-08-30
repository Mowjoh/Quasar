using FluentFTP;
using Nett;
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

        public static void ModifyTouchmARCConfig(string a, string s)
        {
            
            string path = Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml";
            TouchmARCConfig config = Toml.ReadFile<TouchmARCConfig>(path);

            config.paths.arc = a;
            config.paths.stream = s;

            Toml.WriteFile<TouchmARCConfig>(config, path);
        }

        public static void GetLocalConfig()
        {
            string source = Properties.Settings.Default.DefaultDir + "\\References\\ModLoaders\\TouchmARC\\arcropolis.toml";
            string path = Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml";
            File.Copy(source, path, true);
        }

        public static bool GetSDConfig(string Letter)
        {
            bool exists = false;
            string inputpath = Letter + "atmosphere\\contents\\01006A800016E000\\romfs\\arcropolis.toml";
            string path = Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml";
            if (File.Exists(inputpath))
            {
                exists = true;
                File.Copy(inputpath, path, true);
            }

            return exists;
        }

        public static bool GetDistantConfig(FtpClient ftp)
        {
            bool exists = false;
            string inputPath = "atmosphere\\contents\\01006A800016E000\\romfs\\arcropolis.toml";
            string path = Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml";

            if (ftp.FileExists(inputPath))
            {
                exists = true;
                ftp.DownloadFile(path, inputPath, FtpLocalExists.Overwrite);
            }

            return exists;
        }

        public static void SendSDConfig(string Letter)
        {
            string inputpath = Letter + "atmosphere\\contents\\01006A800016E000\\romfs\\arcropolis.toml";
            string path = Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml";
            
            Folderino.CheckCopyFile(path, inputpath);
        }

        public static void SendDistantConfig(FtpClient ftp)
        {
            string inputPath = "atmosphere\\contents\\01006A800016E000\\romfs\\arcropolis.toml";
            string path = Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml";

            ftp.UploadFile(path, inputPath, FtpRemoteExists.Overwrite);
        }

        public static bool LocalNewer()
        {
            bool test = false;

            string source = Properties.Settings.Default.DefaultDir + "\\References\\ModLoaders\\TouchmARC\\arcropolis.toml";
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

            return test;
        }

        public static void sendRemote(FtpClient ftp)
        {
            string basefolder = Properties.Settings.Default.DefaultDir + "\\References\\ModLoaders\\TouchmARC\\";

            string[] Paths = new string[]
            {
                "atmosphere\\contents\\01006A800016E000\\exefs\\main.npdm",
                "atmosphere\\contents\\01006A800016E000\\exefs\\subsdk9",
                "atmosphere\\contents\\01006A800016E000\\romfs\\skyline\\plugins\\libarcropolis.nro"
            };
            foreach(string s in Paths)
            {
                ftp.UploadFile(basefolder+s, s, FtpRemoteExists.Overwrite);
            }
            
        }
        public static void sendLocal(string Letter)
        {
            string basefolder = Properties.Settings.Default.DefaultDir + "\\References\\ModLoaders\\TouchmARC\\";

            string[] Paths = new string[]
            {
                "atmosphere\\contents\\01006A800016E000\\exefs\\main.npdm",
                "atmosphere\\contents\\01006A800016E000\\exefs\\subsdk9",
                "atmosphere\\contents\\01006A800016E000\\romfs\\skyline\\plugins\\libarcropolis.nro"
            };
            foreach (string s in Paths)
            {
                Folderino.CheckCopyFile(basefolder + s,Letter + s);
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
        public string stream { get; set; }
        public string umm { get; set; }
    }

    public class misc
    {
        public bool debug { get; set; }
    }
}
