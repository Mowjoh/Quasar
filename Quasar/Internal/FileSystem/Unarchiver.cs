using Quasar.Controls;
using Quasar.Controls.Mod.ViewModels;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Quasar.FileSystem
{
    public class Unarchiver
    {
        ModListItemViewModel ModListItemViewModel;

        public Unarchiver(ModListItemViewModel mli)
        {
            ModListItemViewModel = mli;
        }

        //Archive Extraction async process
        public async Task<int> ExtractArchiveAsync(string _ArchiveSource, string ArchiveDestination, string _ArchiveType)
        {
            //Setting up Extraction UI
            ModListItemViewModel.ModStatusTextValue = "Extractin";

            //Launching Extraction Task
            await Task.Run(() => Extract(_ArchiveSource, ArchiveDestination, _ArchiveType));

            return 0;
        }

        //Archive Extraction Task
        public Task<int> Extract(string _ArchiveSource, string ArchiveDestination, string _ArchiveType)
        {
            switch (_ArchiveType)
            {
                case "rar":
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
                case "zip":
                    using (var archive = ZipArchive.Open(_ArchiveSource))
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
                case "7z":
                    using (var archive = SevenZipArchive.Open(_ArchiveSource))
                    {
                        var reader = archive.ExtractAllEntries();
                        while (reader.MoveToNextEntry())
                        {
                            if (!reader.Entry.IsDirectory)
                            {
                                SevenZipArchiveEntry entry = (SevenZipArchiveEntry)reader.Entry;
                                entry.WriteToDirectory(ArchiveDestination, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                            }
                        }
                    }
                    break;
                default:

                    break;
            }

            return Task.FromResult(1);
        }
    }
}
