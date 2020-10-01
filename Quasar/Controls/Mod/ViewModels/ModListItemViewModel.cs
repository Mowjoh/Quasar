using Quasar.Controls.Common.Models;
using Quasar.Controls.ModManagement.ViewModels;
using Quasar.FileSystem;
using Quasar.Internal;
using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Quasar.Controls.Mod.ViewModels
{
    public class ModListItemViewModel : ObservableObject
    {
        #region Fields

        #region Data
        private ObservableCollection<Game> _Games { get; set; }
        private Game _Game { get; set; }
        private LibraryMod _LibraryMod { get; set; }
        private ObservableCollection<LibraryMod> _Mods { get; set; }
        
        private ObservableCollection<Object> _Authors { get; set; }
        private ObservableCollection<Object> _Roles { get; set; }

        private string _ActionRequested { get; set; }

        #endregion

        #region Working References
        private String _ModStatusValue { get; set; }
        private String _ModStatusTextValue { get; set; }
        private string _ContentStatText { get; set; }
        private int _ProgressBarValue { get; set; }
        private int _ContentStatValue { get; set; }
        private bool _Downloading { get; set; }
        private bool _CreatorMode { get; set; }
        private bool _AdvancedMode { get; set; }
        private bool _Smol { get; set; }
        private Rect _Rekt { get; set; }
        private Rect _Rekta { get; set; }
        private Uri _ImageSource { get; set; }
        #endregion

        #region Commands
        private ICommand _MinimizeCommand { get; set; }
        private ICommand _FileViewCommand { get; set; }
        private ICommand _DeleteModCommand { get; set; }
        private ICommand _AddModCommand { get; set; }
        private ICommand _ShowContentsCommand { get; set; }
        #endregion

        #endregion

        #region Properties

        #region Data
        public ObservableCollection<Game> Games
        {
            get => _Games;
            set
            {
                if (_Games == value)
                    return;

                _Games = value;
                OnPropertyChanged("Games");
            }
        }
        public Game Game
        {
            get
            {
                return _Game;
            }
            set
            {
                if (_Game == value)
                    return;

                _Game = value;
                OnPropertyChanged("Game");
            }
        }
        public LibraryMod LibraryMod
        {
            get
            {
                return _LibraryMod;
            }
            set
            {
                if (_LibraryMod == value)
                    return;

                _LibraryMod = value;
                OnPropertyChanged("LibraryMod");
            }
        }
        public ObservableCollection<LibraryMod> Mods
        {
            get => _Mods;
            set
            {
                if (_Mods == value)
                    return;

                _Mods = value;
                OnPropertyChanged("Mods");
            }
        }
        


        public ObservableCollection<Object> Authors
        {
            get => _Authors;
            set
            {
                if (_Authors == value)
                    return;

                _Authors = value;
                OnPropertyChanged("Authors");
            }
        }
        public ObservableCollection<Object> Roles
        {
            get => _Roles;
            set
            {
                if (_Roles == value)
                    return;

                _Roles = value;
                OnPropertyChanged("Roles");
            }
        }

        public string ActionRequested
        {
            get => _ActionRequested;
            set
            {
                if (_ActionRequested == value)
                    return;


                _ActionRequested = value;
                OnPropertyChanged("ActionRequested");
            }
        }
        #endregion

        #region Working References
        public String ModStatusValue
        {
            get
            {
                return _ModStatusValue;
            }
            set
            {
                if (_ModStatusValue == value)
                    return;

                _ModStatusValue = value;
                OnPropertyChanged("ModStatusValue");
            }
        }
        public String ModStatusTextValue
        {
            get
            {
                return _ModStatusTextValue;
            }
            set
            {
                if (_ModStatusTextValue == value)
                    return;

                _ModStatusTextValue = value;
                OnPropertyChanged("ModStatusTextValue");
            }
        }
        public string ContentStatText
        {
            get
            {
                return _ContentStatText;
            }
            set
            {
                if (_ContentStatText == value)
                    return;

                _ContentStatText = value;
                OnPropertyChanged("ContentStatText");
            }
        }
        public int ContentStatValue
        {
            get
            {
                return _ContentStatValue;
            }
            set
            {
                if (_ContentStatValue == value)
                    return;

                _ContentStatValue = value;
                OnPropertyChanged("ContentStatValue");
            }
        }
        public int ProgressBarValue
        {
            get
            {
                return _ProgressBarValue;
            }
            set
            {
                if (_ProgressBarValue == value)
                    return;

                _ProgressBarValue = value;
                OnPropertyChanged("ProgressBarValue");
            }
        }
        public bool Downloading
        {
            get
            {
                return _Downloading;
            }
            set
            {
                if (_Downloading == value)
                    return;

                _Downloading = value;
                OnPropertyChanged("Downloading");
            }
        }
        public bool CreatorMode
        {
            get
            {
                return _CreatorMode;
            }
            set
            {
                if (_CreatorMode == value)
                    return;

                _CreatorMode = value;
                OnPropertyChanged("CreatorMode");
            }
        }
        public bool AdvancedMode
        {
            get
            {
                return _AdvancedMode;
            }
            set
            {
                if (_AdvancedMode == value)
                    return;

                _AdvancedMode = value;
                OnPropertyChanged("AdvancedMode");
            }
        }
        public bool Smol
        {
            get
            {
                return _Smol;
            }
            set
            {
                if (_Smol == value)
                    return;

                _Smol = value;

                Rekt = value ? new Rect(0, 0, 50, 30) : new Rect(0, 0, 50, 160);
                Rekta = value ? new Rect(0, 0, 50, 30) : new Rect(0, 0, 50, 160);

                LoadImage(_Smol);
                OnPropertyChanged("Smol");
            }
        }
        public Rect Rekt
        {
            get => _Rekt;
            set
            {
                if (_Rekt == value)
                    return;

                _Rekt = value;
                OnPropertyChanged("Rekt");
            }
        }
        public Rect Rekta
        {
            get => _Rekta;
            set
            {
                if (_Rekta == value)
                    return;

                _Rekta = value;
                OnPropertyChanged("Rekta");
            }
        }
        public Uri ImageSource
        {
            get => _ImageSource;
            set
            {
                if (_ImageSource == value)
                    return;

                _ImageSource = value;
                OnPropertyChanged("ImageSource");
            }
        }



        #endregion

        #region Commands
        public ICommand MinimizeCommand
        {
            get
            {
                if (_MinimizeCommand == null)
                {
                    _MinimizeCommand = new RelayCommand(param => Test());
                }
                return _MinimizeCommand;
            }
        }
        public ICommand FileViewCommand
        {
            get
            {
                if (_FileViewCommand == null)
                {
                    _FileViewCommand = new RelayCommand(param => ShowFileView());
                }
                return _FileViewCommand;
            }
        }
        public ICommand DeleteModCommand
        {
            get
            {
                if (_DeleteModCommand == null)
                {
                    _DeleteModCommand = new RelayCommand(param => DeleteMod());
                }
                return _DeleteModCommand;
            }
        }

        public ICommand AddModCommand
        {
            get
            {
                if (_AddModCommand == null)
                {
                    _AddModCommand = new RelayCommand(param => AddMod());
                }
                return _AddModCommand;
            }
        }
        public ICommand ShowContentsCommand
        {
            get
            {
                if (_ShowContentsCommand == null)
                {
                    _ShowContentsCommand = new RelayCommand(param => ShowContents());
                }
                return _ShowContentsCommand;
            }
        }
        #endregion

        #endregion

        public ModsViewModel MVM { get; set; }

        public ModListItemViewModel()
        {
            Downloading = true;
            Smol = true;
        }

        public ModListItemViewModel(LibraryMod Mod, Game Gamu, ModsViewModel model, bool _Downloading = false)
        {
            Game = Gamu;
            LibraryMod = Mod;
            Downloading = _Downloading;
            Smol = true;
            MVM = model;

            CreatorMode = Properties.Settings.Default.EnableCreator;
            
            GetAuthors();
        }

        public ModListItemViewModel(string QuasarURL, ObservableCollection<Game> _Games, ObservableCollection<LibraryMod> _Mods, ModsViewModel model)
        {
            Downloading = true;
            Smol = true;
            Games = _Games;
            Mods = _Mods;
            MVM = model;
        }

        #region Actions
        

        public void Extract()
        {

        }

        public void LoadImage(bool _Smol)
        {
            if (!_Smol)
            {
                string imageSource = Properties.Settings.Default.DefaultDir + @"\Library\Screenshots\";
                string imagename = LibraryMod.GameID + "_" + LibraryMod.TypeID + "_" + LibraryMod.ID;
                string[] files = System.IO.Directory.GetFiles(imageSource, imagename + ".*");

                if (files.Length > 0)
                {
                    ImageSource = new Uri(files[0], UriKind.RelativeOrAbsolute);
                }
                else
                {
                    ImageSource = new Uri(Properties.Settings.Default.DefaultDir + @"\References\images\NoScreenshot.png");
                }
            }
            else
            {
                ImageSource = new Uri(Properties.Settings.Default.DefaultDir + @"\References\images\NoScreenshot.png");
            }
            
        }
        public void LoadStats()
        {
            if(MVM != null)
            {
                List<ContentMapping> CM = MVM.ContentMappings.Where(cm => cm.ModID == LibraryMod.ID).ToList();
                bool allFound = true;
                bool noneFound = true;
                foreach (ContentMapping mapping in CM)
                {
                    if (!MVM.ActiveWorkspace.Associations.Any(a => a.ContentMappingID == mapping.ID))
                    {
                        allFound = false;
                    }
                    else
                    {
                        noneFound = false;
                    }
                }
                ContentStatValue = noneFound ? 0 : allFound ? 2 : 1;
            }
            
        }

        public void GetAuthors()
        {
            Authors = new ObservableCollection<object>();
            Roles = new ObservableCollection<object>();

            int count = _LibraryMod.Authors.Length > 3 ? 3 : _LibraryMod.Authors.Length;
            for (int i = 0; i < count; i++)
            {
                if (_LibraryMod.Authors[i][2] == "0")
                {
                    Authors.Add(new Label() { Content = _LibraryMod.Authors[i][0], Foreground = (SolidColorBrush)App.Current.Resources["QuasarTextColor"], Height = 28 });
                }
                else
                {
                    Run run = new Run();
                    run.Text = _LibraryMod.Authors[i][0];

                    Hyperlink hl = new Hyperlink(run);
                    hl.NavigateUri = new Uri(@"https://gamebanana.com/members/" + _LibraryMod.Authors[i][2]);
                    hl.Click += new RoutedEventHandler(link_click);

                    TextBlock textBlock = new TextBlock() { Margin = new Thickness(5, 0, 0, 0), Foreground = (SolidColorBrush)App.Current.Resources["QuasarTextColor"], Height = 28, TextAlignment = TextAlignment.Left, LineStackingStrategy = LineStackingStrategy.BlockLineHeight, LineHeight = 22 };
                    textBlock.Inlines.Add(hl);

                    Authors.Add(textBlock);
                }

                Roles.Add(new Label() { Content = _LibraryMod.Authors[i][1] != ""? _LibraryMod.Authors[i][1] : "No role provided" , Foreground = (SolidColorBrush)App.Current.Resources["QuasarTextColor"], Height = 28 });
            }
        }

        public void link_click(Object sender, RoutedEventArgs e)
        {
            
            Hyperlink Link = (Hyperlink)sender;
            Process.Start(Link.NavigateUri.ToString());
        }

        public void ShowFileView()
        {
            new FileView(new ModFileManager(LibraryMod, Game).LibraryContentFolderPath, LibraryMod.Name).Show();
        }

        public void Test()
        {
            ModFileManager mfm = new ModFileManager(LibraryMod, MVM.Games[1]);
            new DefinitionsWindow(mfm, "", "", "", "", MVM.GameDatas[0].Categories.ToList(), MVM.InternalModTypes.ToList(), 0).Show();
        }

        public void DeleteMod()
        {
            ActionRequested = "Delete";
            EventSystem.Publish<ModListItemViewModel>(this);
        }

        public void AddMod()
        {
            if(ContentStatValue == 0)
            {
                ActionRequested = "Add";
            }
            else
            {
                ActionRequested = "Remove";
            }
            
            EventSystem.Publish<ModListItemViewModel>(this);
        }

        public void ShowContents()
        {
            ActionRequested = "ShowContents";
            EventSystem.Publish<ModListItemViewModel>(this);
        }

        private async void CheckUpdates(object sender, RoutedEventArgs e)
        {
            /*
            ModListItem element = (ModListItem)ManagementModListView.SelectedItem;
            if (element != null)
            {
                //Getting local Mod
                LibraryMod mod = Mods.Find(mm => mm.ID == element.LocalMod.ID && mm.TypeID == element.LocalMod.TypeID);
                Game game = Games.Find(g => g.ID == mod.GameID);
                GameModType mt = game.GameModTypes.Find(g => g.ID == mod.TypeID);
                //Parsing mod info from API
                APIMod newAPIMod = await APIRequest.GetAPIMod(mt.APIName, element.LocalMod.ID.ToString());

                //Create Mod from API information
                LibraryMod newmod = GetLibraryMod(newAPIMod, game);

                if (mod.Updates < newmod.Updates)
                {
                    string[] newDL = await APIRequest.GetDownloadFileName(mt.APIName, element.LocalMod.ID.ToString());
                    string quasarURL = APIRequest.GetQuasarDownloadURL(newDL[0], newDL[1], mt.APIName, element.LocalMod.ID.ToString());
                    LaunchDownload(quasarURL);
                }
            }*/
        }
        #endregion
    }
}
