using Quasar.Controls.Build.ViewModels;
using Quasar.Controls.Common.Models;
using Quasar.FileSystem;
using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Quasar.Controls.Build.Models
{

    enum BuildModes { Comparative, WipeRecreate }

    public abstract class Builder: ObservableObject
    {
        public abstract void CopyModLoader();
        public abstract void StartCheck();
        public abstract Task<bool> StartBuild();
        public abstract bool StartTransfer();
        public abstract void CompareDeleteDifferences();
        public abstract void DeleteAll();
        public abstract void SetData(BuildViewModel ViewModel);
        
    }

    public class SmashBuilder : Builder
    {
        #region Fields
        private BuildViewModel _ViewModel { get; set; }
        #endregion

        #region Properties
        /// <summary>
        /// List of all Library Mods
        /// </summary>
        public BuildViewModel ViewModel
        {
            get => _ViewModel;
            set
            {
                if (_ViewModel == value)
                    return;

                _ViewModel = value;
                OnPropertyChanged("ViewModel");
            }
        }
        #endregion


        FileWriter Writer { get; set; }
        int BuildMode { get; set; }
        int ModLoader { get; set; }
        string Logs { get; set; }

        public SmashBuilder(FileWriter _Writer, int _BuildMode, int _ModLoader, string _Logs)
        {
            Writer = _Writer;
            BuildMode = _BuildMode;
            ModLoader = _ModLoader;
            Logs = _Logs;

            if (Writer.VerifyOK())
            {
                CopyModLoader();
                StartCheck();
            }
        }
        public override void SetData(BuildViewModel _ViewModel)
        {
            ViewModel = _ViewModel;
        }
        public override void CopyModLoader()
        {
            
        }
        public override void StartCheck()
        {
            if (BuildMode == (int)BuildModes.Comparative)
            {
                CompareDeleteDifferences();
            }
            if (BuildMode == (int)BuildModes.WipeRecreate)
            {
                DeleteAll();
            }
        }
        public override async Task<bool> StartBuild()
        {
            if (Writer.VerifyOK())
            {
                CopyModLoader();
                StartCheck();
                StartTransfer();
            }

            return false;
        }
        public override bool StartTransfer()
        {
            GameData GD = ViewModel.GameDatas.Single(g => g.GameID == ViewModel.Games[1].ID);
            foreach (Association ass in ViewModel.ActiveWorkspace.Associations)
            {
                //References
                ContentMapping cm = ViewModel.ContentMappings.Single(l => l.ID == ass.ContentMappingID);
                InternalModType imt = ViewModel.InternalModTypes.Single(t => t.ID == cm.InternalModType);
                GameDataCategory GDC = GD.Categories.Find(c => c.ID == imt.Association);
                GameDataItem GDI = GDC.Items.Find(i => i.ID == cm.GameDataItemID);

                //Mod
                LibraryMod lm = ViewModel.Mods.Single(m => m.ID == cm.ModID);
                ModFileManager mfm = new ModFileManager(lm, ViewModel.Games[1]);
                
                foreach(ContentMappingFile file in cm.Files)
                {
                    //Looping through recognized files
                    string source = file.SourcePath;
                    string basedestination = "";

                    InternalModTypeFile imtf = imt.Files.Find(f => f.ID == file.InternalModTypeFileID);

                    BuilderFile bfi = imtf.Files[ModLoader];

                    string[] output = BuilderActions.FormatOutput(bfi.File, bfi.Path, GDI.Attributes[0].Value, file, cm);
                    string FinalDestination = basedestination + output[0] + "/" + output[1];
                    FinalDestination = FinalDestination.Replace(@"{Workspace}", ViewModel.ActiveWorkspace.Name);
                    FinalDestination = FinalDestination.Replace(@"/", @"\");
                    Writer.SendFile(source, FinalDestination);
                    Logs += ("Finished with : " + cm.Name + " of Type : " + imt.Name + " Into Slot " + (ass.Slot + 1) + "\r\n");
                }
               
            }

            return false;
        }
        public override void CompareDeleteDifferences()
        {

        }
        public override void DeleteAll()
        {

        }
        
    }

    static class BuilderActions
    {
        public static string[] FormatOutput(string file, string path, string GameDataItem, ContentMappingFile cmf, ContentMapping cm)
        {
            string OutputFile = file;
            string OutputPath = path;
            Regex FolderReplacinator = new Regex("{Folder}");
            Regex PartReplacinator = new Regex("{Part}");
            Regex GameDataReplacinator = new Regex("{GameData}");
            Regex SlotReplacinatorSingle = new Regex("{S0}");
            Regex SlotReplacinatorDouble = new Regex("{S00}");
            Regex SlotReplacinatorTriple = new Regex("{S000}");
            Regex AnyReplacinator = new Regex("{AnyFile}");

            //Processing File Output
            if (cmf.FileParts != null)
            {
                foreach (string s in cmf.FileParts)
                {
                    OutputFile = PartReplacinator.Replace(OutputFile, s, 1);
                }
            }
            if (cmf.FileFolders != null)
            {
                foreach (string s in cmf.FileFolders)
                {
                    OutputFile = FolderReplacinator.Replace(OutputFile, s, 1);
                }
            }

            OutputFile = GameDataReplacinator.Replace(OutputFile, GameDataItem, 1);

            OutputFile = SlotReplacinatorSingle.Replace(OutputFile, cm.Slot.ToString("0"), 1);
            OutputFile = SlotReplacinatorDouble.Replace(OutputFile, cm.Slot.ToString("00"), 1);
            OutputFile = SlotReplacinatorTriple.Replace(OutputFile, cm.Slot.ToString("000"), 1);

            if (cmf.AnyFile != null)
            {
                OutputFile = AnyReplacinator.Replace(OutputFile, cmf.AnyFile, 1);
            }

            //Processing Folder Output
            if (cmf.FolderParts != null)
            {
                foreach (string s in cmf.FolderParts)
                {
                    OutputPath = PartReplacinator.Replace(OutputPath, s, 1);
                }
            }

            if (cmf.FolderFolders != null)
            {
                foreach (string s in cmf.FolderFolders)
                {
                    OutputPath = FolderReplacinator.Replace(OutputPath, s, 1);
                }
            }

            OutputPath = GameDataReplacinator.Replace(OutputPath, GameDataItem, 1);

            OutputPath = SlotReplacinatorSingle.Replace(OutputPath, cm.Slot.ToString("0"), 1);
            OutputPath = SlotReplacinatorDouble.Replace(OutputPath, cm.Slot.ToString("00"), 1);
            OutputPath = SlotReplacinatorTriple.Replace(OutputPath, cm.Slot.ToString("000"), 1);

            return new string[2] { OutputPath, OutputFile };
        }

        public static void NotifyStatus(string Status, BuildViewModel ViewModel)
        {
           // Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, ViewModel.Steps = Status);
        }
    }



}
