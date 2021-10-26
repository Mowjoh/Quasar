
using log4net;
using Quasar.Controls.Mod.ViewModels;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Quasar.Helpers.Compression
{
    public class Unarchiver
    {
        //View Model for UI Updates
        ModListItemViewModel ModListItemViewModel;

        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <param name="mli">Mod List Item View Model</param>
        public Unarchiver(ModListItemViewModel mli)
        {
            ModListItemViewModel = mli;
        }

        /// <summary>
        /// Archive Extraction async process
        /// </summary>
        /// <param name="_ArchiveSource"></param>
        /// <param name="ArchiveDestination"></param>
        /// <param name="_ArchiveType"></param>
        /// <returns></returns>
        public async Task<int> ExtractArchiveAsync(string _ArchiveSource, string ArchiveDestination, string _ArchiveType)
        {
            //Launching Extraction Task
            await Task.Run(() => Extract(_ArchiveSource, ArchiveDestination, _ArchiveType, ModListItemViewModel.MVM.QuasarLogger));

            return 0;
        }

        /// <summary>
        /// Archive Extraction Task
        /// </summary>
        /// <param name="_ArchiveSource">Source file path</param>
        /// <param name="ArchiveDestination">Destination folder path</param>
        /// <param name="_ArchiveType">Archive extension</param>
        /// <returns></returns>
        public Task<int> Extract(string _ArchiveSource, string ArchiveDestination, string _ArchiveType, ILog QuasarLogger)
        {
            try
            {
                switch (_ArchiveType)
                {
                    case "rar":
                        if (!Directory.Exists(ArchiveDestination))
                            Directory.CreateDirectory(ArchiveDestination);

                        using (var archive = RarArchive.Open(_ArchiveSource))
                        {
                            foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                            {
                                entry.WriteToDirectory(ArchiveDestination, new ExtractionOptions()
                                {
                                    ExtractFullPath = true,
                                    Overwrite = true
                                });
                            }
                        }
                        break;
                    default:
                        string zPath = "7za.exe"; //add to proj and set CopyToOuputDir
                        ProcessStartInfo pro = new ProcessStartInfo();
                        pro.WindowStyle = ProcessWindowStyle.Hidden;
                        pro.CreateNoWindow = true;
                        pro.FileName = zPath;
                        pro.Arguments = string.Format("x \"{0}\" -y -o\"{1}\"", _ArchiveSource, ArchiveDestination);
                        Process x = Process.Start(pro);
                        x.WaitForExit();

                        break;
                }
                
            }
            catch(Exception e)
            {
                QuasarLogger.Error(e.Message);
                QuasarLogger.Error(e.StackTrace);

            }
            

            return Task.FromResult(1);
        }
    }
}
