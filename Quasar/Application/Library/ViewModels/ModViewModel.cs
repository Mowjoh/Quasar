using log4net;
using Quasar.Common.Models;
using Quasar.Controls.ModManagement.ViewModels;
using DataModels.Common;
using DataModels.User;
using DataModels.Resource;
using Quasar.FileSystem;
using Quasar.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Quasar.MainUI.ViewModels;
using Workshop.FileManagement;

namespace Quasar.Controls.Mod.ViewModels
{
    public class ModViewModel : ObservableObject
    {

        #region Data


        #region Private
        private ObservableCollection<Game> _Games { get; set; }
        private Game _Game { get; set; }
        private LibraryItem _LibraryMod { get; set; }
        private GamebananaRootCategory _AssociatedRootCategory { get; set; }
        private GamebananaSubCategory _AssociatedSubCategory { get; set; }
        private ObservableCollection<LibraryItem> _Mods { get; set; }

        private ObservableCollection<Object> _Authors { get; set; }
        private ObservableCollection<Object> _Roles { get; set; }

        private string _ActionRequested { get; set; }

        public LibraryViewModel MVM { get; set; }
        #endregion

        #region Public
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
        public LibraryItem LibraryItem
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
                OnPropertyChanged("LibraryItem");
                OnPropertyChanged("APISubCategoryName");
                OnPropertyChanged("Standby");
                OnPropertyChanged("StandbyEditing");
                if (value != null && value.GBItem != null)
                {
                    AssociatedRootCategory = MVM.MUVM.API.Games[0].RootCategories.Single(c => c.Guid == value.GBItem.RootCategoryGuid);
                    AssociatedSubCategory = AssociatedRootCategory.SubCategories.Single(c => c.Guid == value.GBItem.SubCategoryGuid);

                    OnPropertyChanged("AssociatedRootCategory");
                    OnPropertyChanged("AssociatedSubCategory");
                }
                
            }
        }
        public GamebananaRootCategory AssociatedRootCategory
        {
            get => _AssociatedRootCategory;
            set
            {
                if (_AssociatedRootCategory == value)
                    return;

                _AssociatedRootCategory = value;
                OnPropertyChanged("AssociatedRootCategory");
            }
        }
        public GamebananaSubCategory AssociatedSubCategory
        {
            get => _AssociatedSubCategory;
            set
            {
                if (_AssociatedSubCategory == value)
                    return;

                _AssociatedSubCategory = value;
                OnPropertyChanged("AssociatedSubCategory");
            }
        }
        public ObservableCollection<LibraryItem> Mods
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
        public string APICategoryName
        {
            get
            {
                if (LibraryItem != null)
                {
                    if (LibraryItem.ManualMod)
                    {
                        return "Manual";
                    }
                    else
                    {
                        return AssociatedRootCategory.Name;
                    }

                }
                else
                {
                    return "";
                }
            }
        }
        public string APISubCategoryName
        {
            get
            {
                if (LibraryItem != null)
                {
                    if (LibraryItem.ManualMod)
                    {
                        return "Manual";
                    }
                    else
                    {
                        return AssociatedSubCategory.Name;
                    }

                }
                else
                {
                    return "";
                }
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

        #endregion

        #region View

        #region Private
        private String _ModStatusValue { get; set; }
        private String _ModStatusTextValue { get; set; }
        private string _ContentStatText { get; set; }
        private int _ProgressBarValue { get; set; }
        private int _ContentStatValue { get; set; }
        private bool _Downloading { get; set; }
        private bool _DownloadFailed { get; set; } = false;
        private bool _CreatorMode { get; set; }
        private bool _AdvancedMode { get; set; }
        private bool _Smol { get; set; }
        private Rect _Rekt { get; set; }
        private Rect _Rekta { get; set; }
        private Uri _ImageSource { get; set; }
        private string _HumanTime { get; set; }
        #endregion

        #region Public
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
                OnPropertyChanged("Standby");
            }
        }
        public bool Standby => !Downloading;
        public bool StandbyEditing => !Downloading && LibraryItem.Editing;
        public bool StandbyNotEditing => !Downloading && !LibraryItem.Editing;
        public bool DownloadFailed
        {
            get
            {
                return _DownloadFailed;
            }
            set
            {
                if (_DownloadFailed == value)
                    return;

                _DownloadFailed = value;
                OnPropertyChanged("DownloadFailed");
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
        public bool NoContent => ContentStatValue != 3;
        public bool ManualMod => LibraryItem.GBItem == null;
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

                Rekt = value ? new Rect(0, 0, 50, 44) : new Rect(0, 0, 50, 217);
                Rekta = value ? new Rect(0, 0, 50, 44) : new Rect(0, 0, 50, 217);

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
        public string HumanTime
        {
            get
            {
                return _HumanTime;
            }
            set
            {
                if (_HumanTime == value)
                    return;

                _HumanTime = value;
                OnPropertyChanged("HumanTime");
            }
        }
        #endregion

        #endregion

        #region Commands

        #region Private
        private ICommand _UpdateModCommand { get; set; }
        private ICommand _AssignCommand { get; set; }
        private ICommand _DeleteModCommand { get; set; }
        private ICommand _AddModCommand { get; set; }
        private ICommand _ShowContentsCommand { get; set; }
        private ICommand _RetryDownloadCommand { get; set; }
        private ICommand _BigSmolCommand { get; set; }
        private ICommand _RenameCommand { get; set; }
        #endregion

        #region Public
        public ICommand UpdateModCommand
        {
            get
            {
                if (_UpdateModCommand == null)
                {
                    _UpdateModCommand = new RelayCommand(param => UpdateMod());
                }
                return _UpdateModCommand;
            }
        }
        public ICommand AssignCommand
        {
            get
            {
                if (_AssignCommand == null)
                {
                    _AssignCommand = new RelayCommand(param => ShowAssignmentTab());
                }
                return _AssignCommand;
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
                    _AddModCommand = new RelayCommand(param => AddorRemoveFromWorkspace());
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
                    _ShowContentsCommand = new RelayCommand(param => ShowModContents());
                }
                return _ShowContentsCommand;
            }
        }
        public ICommand RetryDownloadCommand
        {
            get
            {
                if (_RetryDownloadCommand == null)
                {
                    _RetryDownloadCommand = new RelayCommand(param => RetryDownload());
                }
                return _RetryDownloadCommand;
            }
        }
        public ICommand BigSmolCommand
        {
            get
            {
                if (_BigSmolCommand == null)
                {
                    _BigSmolCommand = new RelayCommand(param => BigSmol());
                }
                return _BigSmolCommand;
            }
        }

        public ICommand RenameCommand
        {
            get
            {
                if (_RenameCommand == null)
                {
                    _RenameCommand = new RelayCommand(param => RenameMod());
                }
                return _RenameCommand;
            }
        }

        #endregion

        #endregion

        ILog QuasarLogger { get; set; }

        public ModViewModel()
        {
            Downloading = true;
            Smol = true;

            EventSystem.Subscribe<LibraryItem>(Refresh);
        }

        public ModViewModel(LibraryItem Mod, Game Gamu, LibraryViewModel model, ILog _QuasarLogger, bool _Downloading = false)
        {
            QuasarLogger = _QuasarLogger;

            Game = Gamu;
            Downloading = _Downloading;
            Smol = true;
            MVM = model;
            LibraryItem = Mod;

            GetHumanTime();
            GetAuthors();

            EventSystem.Subscribe<LibraryItem>(Refresh);
        }

        public ModViewModel(string QuasarURL, ObservableCollection<Game> _Games, ObservableCollection<LibraryItem> _Mods, LibraryViewModel model, ILog _QuasarLogger)
        {
            QuasarLogger = _QuasarLogger;

            Downloading = true;
            Smol = true;
            Games = _Games;
            Mods = _Mods;
            MVM = model;


            EventSystem.Subscribe<LibraryItem>(Refresh);
        }

        #region Actions
        
        /// <summary>
        /// Refreshes this mods UI with a new Library Item
        /// </summary>
        /// <param name="li"></param>
        public void Refresh(LibraryItem li)
        {
            if(li.Guid == LibraryItem.Guid)
            {
                GetHumanTime();
                OnPropertyChanged("LibraryItem");
            }

        }

        /// <summary>
        /// Triggers an Image source change
        /// </summary>
        /// <param name="_Smol"></param>
        public void LoadImage(bool _Smol)
        {
            if (!_Smol)
            {
                string imageSource = Properties.Settings.Default.DefaultDir + @"\Library\Screenshots\";
                string[] files = System.IO.Directory.GetFiles(imageSource, LibraryItem.Guid + ".*");

                if (files.Length > 0)
                {
                    ImageSource = new Uri(files[0], UriKind.RelativeOrAbsolute);
                }
                else
                {
                    ImageSource = new Uri(Properties.Settings.Default.DefaultDir + @"\Resources\images\NoScreenshot.png");
                }
            }
            else
            {
                ImageSource = new Uri(Properties.Settings.Default.DefaultDir + @"\Resources\images\NoScreenshot.png");
            }
            
        }

        /// <summary>
        /// Calculates workspace presence
        /// </summary>
        public void LoadStats()
        {
            OnPropertyChanged("LibraryItem");
            OnPropertyChanged("StandbyNotEditing");
            OnPropertyChanged("StandbyEditing");

        }

        /// <summary>
        /// Refreshes the Authors UI panel
        /// </summary>
        public void GetAuthors()
        {
            Authors = new ObservableCollection<object>();
            Roles = new ObservableCollection<object>();
            if(_LibraryMod.GBItem != null)
            {
                int count = _LibraryMod.GBItem.Authors.Count > 3 ? 3 : _LibraryMod.GBItem.Authors.Count;
                for (int i = 0; i < count; i++)
                {
                    if (_LibraryMod.GBItem.Authors[i].GamebananaAuthorID == 0)
                    {
                        Authors.Add(new Label()
                        {
                            Content = _LibraryMod.GBItem.Authors[i].Name,
                            Foreground = (SolidColorBrush)App.Current.Resources["QuasarTextColor"],
                            FontFamily = (FontFamily)App.Current.Resources["PoppinsRegular"],
                            Height = 28
                        });
                    }
                    else
                    {
                        Run run = new Run();
                        run.Text = _LibraryMod.GBItem.Authors[i].Name;

                        Hyperlink hl = new Hyperlink(run);
                        hl.NavigateUri = new Uri(@"https://gamebanana.com/members/" + _LibraryMod.GBItem.Authors[i].GamebananaAuthorID);
                        hl.Click += new RoutedEventHandler(link_click);

                        TextBlock textBlock = new TextBlock()
                        {
                            Margin = new Thickness(5, 0, 0, 0),
                            Foreground = (SolidColorBrush)App.Current.Resources["CutieTextColor"],
                            FontFamily = (FontFamily)App.Current.Resources["PoppinsRegular"],
                            Height = 28,
                            TextAlignment = TextAlignment.Left,
                            LineStackingStrategy = LineStackingStrategy.BlockLineHeight,
                            LineHeight = 22
                        };
                        textBlock.Inlines.Add(hl);

                        Authors.Add(textBlock);
                    }

                    Roles.Add(new Label() { Content = _LibraryMod.GBItem.Authors[i].Role != "" ? _LibraryMod.GBItem.Authors[i].Role : "No role provided", FontFamily = (FontFamily)App.Current.Resources["PoppinsRegular"], Foreground = (SolidColorBrush)App.Current.Resources["QuasarTextColor"], Height = 28 });
                }
            }
            else
            {
                Authors.Add(new Label()
                {
                    Content = Properties.Resources.ModItem_Label_ManualAuthor,
                    Foreground = (SolidColorBrush)App.Current.Resources["QuasarTextColor"],
                    FontFamily = (FontFamily)App.Current.Resources["PoppinsRegular"],
                    Height = 28
                });

                Roles.Add(new Label()
                {
                    Content = Properties.Resources.ModItem_Label_ManualAuthorRole,
                    FontFamily = (FontFamily)App.Current.Resources["PoppinsRegular"],
                    Foreground = (SolidColorBrush)App.Current.Resources["QuasarTextColor"],
                    Height = 28
                });
            }
            

        }

        public void GetHumanTime()
        {
            TimeSpan t = DateTime.Now - LibraryItem.Time;

            HumanTime = String.Format(Properties.Resources.ModListItem_Time_Recently);

            if (t.TotalDays > 10)
                HumanTime = Properties.Resources.ModListItem_Time_Long;
            if (t.TotalDays >= 2)
                HumanTime = String.Format(Properties.Resources.ModListItem_Time_Days, t.Days);
            if (t.TotalDays == 1)
                HumanTime = String.Format(Properties.Resources.ModListItem_Time_OneDay);
            if (t.TotalHours >= 2)
                HumanTime = String.Format(Properties.Resources.ModListItem_Time_Hours, t.Hours);
            if (t.TotalHours == 1)
                HumanTime = String.Format(Properties.Resources.ModListItem_Time_OneHour);

            
        }
        #endregion

        #region User Actions

        /// <summary>
        /// Redirects the user to the author's gamebanana page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void link_click(Object sender, RoutedEventArgs e)
        {

            Hyperlink Link = (Hyperlink)sender;
            Process.Start(Link.NavigateUri.ToString());
        }

        /// <summary>
        /// Triggers the file view window
        /// </summary>
        public void ShowAssignmentTab()
        {
            ActionRequested = "ShowAssignments";
            EventSystem.Publish<ModViewModel>(this);
        }

        /// <summary>
        /// Triggers this mod's update process
        /// </summary>
        public void UpdateMod()
        {
            ActionRequested = "Update";
            EventSystem.Publish<ModViewModel>(this);
        }

        /// <summary>
        /// Triggers this mod's deletion
        /// </summary>
        public void DeleteMod()
        {
            ActionRequested = "Delete";
            EventSystem.Publish<ModViewModel>(this);

        }

        /// <summary>
        /// If the mod is present in the workspace, it will remove every association
        /// If not, it slots everything automatically
        /// </summary>
        public void AddorRemoveFromWorkspace()
        {
            if (LibraryItem.Included == false)
            {
                ActionRequested = "Add";
            }
            else
            {
                ActionRequested = "Remove";
            }

            EventSystem.Publish<ModViewModel>(this);
        }

        /// <summary>
        /// Launches a download retry
        /// </summary>
        public void RetryDownload()
        {
            ActionRequested = "RetryDownload";
            EventSystem.Publish<ModViewModel>(this);
        }

        /// <summary>
        /// Shows this mod's contents in the Contents tab
        /// </summary>
        public void ShowModContents()
        {
            ActionRequested = "ShowContents";
            EventSystem.Publish<ModViewModel>(this);
        }

        /// <summary>
        /// Triggers the big or small view
        /// </summary>
        public void BigSmol()
        {
            if (!Smol)
            {
                Smol = true;
            }
            else
            {
                Smol = false;
            }

        }

        public void RenameMod()
        {
            if (LibraryItem.Editing)
            {
                LibraryItem.Editing = false;
                OnPropertyChanged("LibraryItem");
                OnPropertyChanged("StandbyNotEditing");
                OnPropertyChanged("StandbyEditing");
                ActionRequested = "SaveLibrary";
                EventSystem.Publish<ModViewModel>(this);
            }
            else
            {
                LibraryItem.Editing = true;
                OnPropertyChanged("LibraryItem");
                OnPropertyChanged("StandbyNotEditing");
                OnPropertyChanged("StandbyEditing");
            }
            
        }
        #endregion
    }
}
