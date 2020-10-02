using Quasar.FileSystem;
using Quasar.Quasar_Sys;
using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Quasar.XMLResources.Library;

namespace Quasar.Controls
{
    /// <summary>
    /// Interaction logic for DefinitionsWindow.xaml
    /// </summary>
    public partial class DefinitionsWindow : Window, INotifyPropertyChanged
    {
        #region Triggers
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public bool filtered = false;
        #endregion

        #region Data
        private string _InputFolder { get; set; }
        public string InputFolder
        {
            get => _InputFolder;
            set
            {
                _InputFolder = value;
            }
        }

        private string _OutputFolder { get; set; }
        public string OutputFolder
        {
            get => _OutputFolder;
            set
            {
                _OutputFolder = value;
            }
        }

        private string _InputFile { get; set; }
        public string InputFile
        {
            get => _InputFile;
            set
            {
                _InputFile = value;
            }
        }

        private string _OutputFile { get; set; }
        public string OutputFile
        {
            get => _OutputFile;
            set
            {
                _OutputFile = value;
            }
        }

        private ModFileManager _MFM;
        public ModFileManager MFM
        {
            get => _MFM;
            set
            {
                _MFM = value;
            }
        }

        private IEnumerable<string> _Files;
        public IEnumerable<string> Files
        {
            get => _Files;
            set
            {
                _Files = value;
                OnPropertyChanged("Files");
            }
        }

        private List<TestFile> _TestFiles;
        public List<TestFile> TestFiles
        {
            get => _TestFiles;
            set
            {
                OnPropertyChanged("TestFiles");
                _TestFiles = value;
            }
        }

        public List<GameDataCategory> Categories;
        public List<GameDataItem> Items;
        #endregion

        public List<InternalModType> Types;
        public int GameBuilderIndex;

        public DefinitionsWindow()
        {
            InitializeComponent();
        }

        public DefinitionsWindow(ModFileManager _MFM, string IF, string OF, string Ifo, string Ofo, List<GameDataCategory> _GDC, List<InternalModType> _Types, int GameBuilder)
        {
            MFM = _MFM;
            InputFile = IF;
            OutputFile = OF;
            InputFolder = Ifo;
            OutputFolder = Ofo;

            //Getting mod files
            Files = Directory.EnumerateFiles(MFM.LibraryContentFolderPath, "*.*", SearchOption.AllDirectories);

            InitializeComponent();

            InputFileText.Text = InputFile;
            OutputFileText.Text = OutputFile;
            InputFolderText.Text = InputFolder;
            OutputFolderText.Text = OutputFolder;

            Categories = _GDC;
            Types = _Types;
            GameBuilderIndex = GameBuilder;

            getFiles();
            getMatches();

        }

        
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        public void getFiles()
        {
            TestFiles = new List<TestFile>();
            foreach(string f in Files)
            {
                TestFiles.Add(new TestFile() { Path = f.Replace("\\","/") });
            }

        }
        
        public void getMatches()
        {
            Regex FileRegex = new Regex(Searchie.PrepareRegex(InputFileText.Text));
            Regex FolderRegex = new Regex(Searchie.PrepareRegex(InputFolderText.Text));
            Regex OutputFileRegex = new Regex(Searchie.PrepareRegex(OutputFileText.Text));
            Regex OutputFolderRegex = new Regex(Searchie.PrepareRegex(OutputFolderText.Text));

            InputFileRegexT.Text = FileRegex.ToString();
            InputFolderRegexT.Text = FolderRegex.ToString();
            foreach(InternalModType IMT in Types)
            {
                foreach(InternalModTypeFile IMTF in IMT.Files)
                {
                    if(GameBuilderIndex != -1)
                    {
                        FileRegex = new Regex(Searchie.PrepareRegex(IMTF.SourceFile));
                        FolderRegex = new Regex(Searchie.PrepareRegex(IMTF.SourcePath));
                        OutputFileRegex = new Regex(Searchie.PrepareRegex(IMTF.Files[GameBuilderIndex].File));
                        OutputFolderRegex = new Regex(Searchie.PrepareRegex(IMTF.Files[GameBuilderIndex].Path));

                        if (TestFiles != null)
                        {
                            foreach (TestFile tf in TestFiles)
                            {
                                if(tf.ValidFile == false)
                                {
                                    Match fileMatch = FileRegex.Match(tf.Path);
                                    Match folderMatch = FolderRegex.Match(tf.Path);

                                    if (folderMatch.Success)
                                    {
                                        tf.FolderMatch = folderMatch.Value;
                                        getMatchValues(tf, folderMatch.Groups, false);

                                    }
                                    else
                                    {
                                        tf.FolderMatch = "";
                                        tf.FolderFolders = "";
                                        tf.FolderGameData = "";
                                        tf.FolderParts = "";
                                    }

                                    if (fileMatch.Success)
                                    {
                                        tf.FileMatch = fileMatch.Value;
                                        if (folderMatch.Success)
                                        {
                                            getMatchValues(tf, fileMatch.Groups, true);

                                            //If there is a detected Folder Gamedata but not for the file
                                            if ((tf.FileGameData == "" || tf.FileGameData == null) && tf.FolderGameData != null)
                                            {
                                                if (ValidateGameData(tf.FolderGameData,IMT.Association))
                                                {
                                                    tf.ValidFile = true;
                                                    formOutputs(tf, IMTF.Files[GameBuilderIndex].Path, IMTF.Files[GameBuilderIndex].Path);
                                                    tf.InternalModType = IMT.Name;
                                                    tf.InternalModTypeFileID = IMTF.ID;
                                                }
                                            }
                                            //If there is a detected File Gamedata
                                            if (tf.FileGameData != "" && tf.FileGameData != null)
                                            {
                                                if (ValidateGameData(tf.FileGameData, IMT.Association))
                                                {
                                                    tf.ValidFile = true;
                                                    formOutputs(tf, IMTF.Files[GameBuilderIndex].Path, IMTF.Files[GameBuilderIndex].Path);
                                                    tf.InternalModType = IMT.Name;
                                                    tf.InternalModTypeFileID = IMTF.ID;
                                                }
                                            }


                                        }
                                        else
                                        {
                                            getMatchValues(tf, fileMatch.Groups, true);
                                            //If there is a detected File Gamedata
                                            if (tf.FileGameData != "" && tf.FileGameData != null)
                                            {
                                                if (ValidateGameData(tf.FileGameData, IMT.Association))
                                                {
                                                    tf.ValidFile = true;
                                                    formOutputs(tf, IMTF.Files[GameBuilderIndex].Path, IMTF.Files[GameBuilderIndex].Path);
                                                    tf.InternalModType = IMT.Name;
                                                    tf.InternalModTypeFileID = IMTF.ID;
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        OnPropertyChanged("TestFiles");
                    }
                }
            }
            
        }

        public void formOutputs(TestFile tf, string file, string path)
        {
            string OutputFile = file;
            string OutputPath = path;
            Regex FolderReplacinator = new Regex("{Folder}");
            Regex PartReplacinator = new Regex("{Part}");
            Regex GameDataReplacinator = new Regex("{GameData}");
            Regex SlotReplacinator = new Regex("{S00}");
            Regex AnyReplacinator = new Regex("{AnyFile}");

            if (tf.ValidFile)
            {
                if(tf.FileParts != null)
                {
                    foreach (string s in tf.FileParts.Split(';'))
                    {
                        if (s != "")
                        {
                            OutputFile = PartReplacinator.Replace(OutputFile, s, 1);
                        }
                    }
                }
                if (tf.FileFolders != null)
                {
                    foreach (string s in tf.FileFolders.Split(';'))
                    {
                        if (s != "")
                        {
                            OutputFile = FolderReplacinator.Replace(OutputFile, s, 1);
                        }
                    }
                }
                
                if (tf.FileGameData != null)
                {
                    foreach (string s in tf.FileGameData.Split(';'))
                    {
                        if (s != "")
                        {
                            OutputFile = GameDataReplacinator.Replace(OutputFile, s, 1);
                        }
                    }
                }
                if(tf.Slot != null)
                {
                    OutputFile = SlotReplacinator.Replace(OutputFile, tf.Slot, 1);
                }
                

                if (tf.FolderParts != null)
                {
                    foreach (string s in tf.FolderParts.Split(';'))
                    {
                        if (s != "")
                        {
                            OutputPath = PartReplacinator.Replace(OutputPath, s, 1);
                        }
                    }
                }
                
                if (tf.FolderFolders != null)
                {
                    foreach (string s in tf.FolderFolders.Split(';'))
                    {
                        if (s != "")
                        {
                            OutputPath = FolderReplacinator.Replace(OutputPath, s, 1);
                        }
                    }
                }
                
                if (tf.FolderGameData != null)
                {
                    foreach (string s in tf.FolderGameData.Split(';'))
                    {
                        if (s != "")
                        {
                            OutputPath = GameDataReplacinator.Replace(OutputPath, s, 1);
                        }
                    }
                }
                if (tf.Slot != null)
                {
                    OutputPath = SlotReplacinator.Replace(OutputPath, tf.Slot, 1);
                }

                if(tf.AnyFile != null)
                {
                    OutputFile = AnyReplacinator.Replace(OutputFile, tf.AnyFile, 1);
                }
                
            }
            tf.OutputFileMatch = OutputFile;
            tf.OutputFolderMatch = OutputPath;
        }
        public void getMatchValues(TestFile TF, GroupCollection Groups, bool file)
        {
            foreach(Group g in Groups)
            {
                switch (g.Name)
                {
                    default:
                        break;
                    case "Slot":
                        TF.Slot = g.Value;
                        break;
                    case "GameData":
                        if (file)
                        {
                            TF.FileGameData = g.Value.ToLower();
                        }
                        else
                        {
                            TF.FolderGameData = g.Value.ToLower();
                        }
                        
                        break;
                    case "AnyFile":
                        if (file)
                        {
                            TF.AnyFile = g.Value;
                        }
                        else
                        {
                            TF.AnyFile = g.Value;
                        }

                        break;
                    case "Folder":
                        if (file)
                        {
                            foreach(Capture c in g.Captures)
                            {
                                TF.FileFolders += c.Value + ";";
                            }
                            
                        }
                        else
                        {
                            foreach (Capture c in g.Captures)
                            {
                                TF.FolderFolders += c.Value + ";";
                            }
                            
                        }
                        break;
                    case "Part":
                        if (file)
                        {
                            foreach (Capture c in g.Captures)
                            {
                                TF.FileParts += c.Value + ";";
                            }
                            
                        }
                        else
                        {
                            foreach (Capture c in g.Captures)
                            {
                                TF.FolderParts += c.Value + ";";
                            }
                            
                        }
                        break;
                }
            }
        }

        public bool ValidateGameData(string Gamedatavalues, int ID)
        {
            GameDataCategory GDC = Categories.Find(f=> f.ID == ID);
            GameDataItem item = GDC.Items.Find(i => i.Attributes[0].Value == Gamedatavalues);
            
            return item != null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (InputFileText.Text != "" && OutputFileText.Text != "" && InputFolderText.Text != "" && OutputFolderText.Text != "")
            {
                getMatches();
            }
            CollectionViewSource cvs = (CollectionViewSource)this.Resources["CSV"];
            cvs.View.Refresh();
        }
    }
    public class TestFile
    {
        public string Path { get; set; }
        public bool ValidFile { get; set; }
        public string FileMatch { get; set; }
        public string FolderMatch { get; set; }

        public string OutputFileMatch { get; set; }
        public string OutputFolderMatch { get; set; }

        public string FileParts { get; set; }
        public string FileFolders { get; set; }

        public string FileGameData { get; set; }

        public string FolderParts { get; set; }
        public string FolderFolders { get; set; }

        public string FolderGameData { get; set; }
        public string Slot { get; set; }

        public string AnyFile { get; set; }

        public string InternalModType { get; set; }
        public int InternalModTypeFileID { get; set; }
    }

}
