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

namespace Quasar.File
{
    public class Unarchiver
    {
        ProgressBar progressBar;
        Label statusLabel;

        public Unarchiver(ProgressBar _progressBar, Label _statusLabel)
        {
            progressBar = _progressBar;
            statusLabel = _statusLabel;
        }

        public async Task<int> ExtractArchiveAsync(string _ArchiveSource, string ArchiveDestination, string _ArchiveType)
        {
            await progressBar.Dispatcher.BeginInvoke(new Action(() =>
            {
                progressBar.Value = 0;
                statusLabel.Visibility = Visibility.Hidden;
                progressBar.Visibility = Visibility.Visible;
                progressBar.IsIndeterminate = true;
            }), DispatcherPriority.Background);


            await Task.Run(() => Extract(_ArchiveSource, ArchiveDestination, _ArchiveType));

            await statusLabel.Dispatcher.BeginInvoke(new Action(() =>
            {
                statusLabel.Visibility = Visibility.Visible;
                progressBar.Visibility = Visibility.Hidden;
            }), DispatcherPriority.Background);

            return 0;
        }

        public async Task<int> Extract(string _ArchiveSource, string ArchiveDestination, string _ArchiveType)
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

                    break;
            }

            return 0;
        }
    }
}
