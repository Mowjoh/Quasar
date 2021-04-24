using log4net;
using Quasar.Common.Models;
using Quasar.Data.V2;
using Quasar.Helpers.Json;
using Quasar.Helpers;
using Quasar.Helpers.Tools;
using Quasar.Workspaces.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Quasar.MainUI.ViewModels;


namespace Quasar.Workspaces.ViewModels
{
    public class WorkspaceViewModel : ObservableObject
    {
        #region Fields
        ObservableCollection<Workspace> _Workspaces { get; set; }

        private Workspace _ActiveWorkspace {get; set;}

        private MainUIViewModel _MUVM { get; set; }
        private string _WorkspaceName { get; set; }
        private ICommand _RenameWorkspaceCommand { get; set; }
        private ICommand _AddWorkspaceCommand { get; set; }
        private ICommand _DeleteWorkspaceCommand { get; set; }
        private ICommand _EmptyWorkspaceCommand { get; set; }
        private ICommand _DuplicateWorkspaceCommand { get; set; }
        private ICommand _ShareWorkspaceCommand { get; set; }
        private ICommand _ImportWorkspaceCommand { get; set; }
        #endregion

        #region Commands
        public ICommand RenameWorkspaceCommand
        {
            get
            {
                if(_RenameWorkspaceCommand == null)
                {
                    _RenameWorkspaceCommand = new RelayCommand(param => SaveWorkspaceName());
                }
                return _RenameWorkspaceCommand;
            }
        }
        public ICommand AddWorkspaceCommand
        {
            get
            {
                if (_AddWorkspaceCommand == null)
                {
                    _AddWorkspaceCommand = new RelayCommand(param => AddWorkspace());
                }
                return _AddWorkspaceCommand;
            }
        }
        public ICommand DeleteWorkspaceCommand
        {
            get
            {
                if (_DeleteWorkspaceCommand == null)
                {
                    _DeleteWorkspaceCommand = new RelayCommand(param => AskDeleteWorkspace());
                }
                return _DeleteWorkspaceCommand;
            }
        }
        public ICommand EmptyWorkspaceCommand
        {
            get
            {
                if (_EmptyWorkspaceCommand == null)
                {
                    _EmptyWorkspaceCommand = new RelayCommand(param => AskEmptyWorkspace());
                }
                return _EmptyWorkspaceCommand;
            }
        }
        public ICommand DuplicateWorkspaceCommand
        {
            get
            {
                if (_DuplicateWorkspaceCommand == null)
                {
                    _DuplicateWorkspaceCommand = new RelayCommand(param => DuplicateWorkspace());
                }
                return _DuplicateWorkspaceCommand;
            }
        }
        public ICommand ShareWorkspaceCommand
        {
            get
            {
                if (_ShareWorkspaceCommand == null)
                {
                    _ShareWorkspaceCommand = new RelayCommand(param => ShareWorkspace());
                }
                return _ShareWorkspaceCommand;
            }
        }
        public ICommand ImportWorkspaceCommand
        {
            get
            {
                if (_ImportWorkspaceCommand == null)
                {
                    _ImportWorkspaceCommand = new RelayCommand(param => ImportWorkspace());
                }
                return _ImportWorkspaceCommand;
            }
        }
        #endregion

        #region Parameters
        /// <summary>
        /// List of workspaces
        /// </summary>
        public ObservableCollection<Workspace> Workspaces
        {
            get => _Workspaces;
            set
            {
                if (_Workspaces == value)
                    return;

                _Workspaces = value;
                OnPropertyChanged("Workspaces");
            }
        }

        /// <summary>
        /// The Workspace Selected in the list
        /// </summary>
        public Workspace ActiveWorkspace
        {
            get => _ActiveWorkspace;
            set
            {
                if (_ActiveWorkspace == value)
                    return;

                _ActiveWorkspace = value;
                OnPropertyChanged("ActiveWorkspace");

                if(value != null)
                {
                    WorkspaceName = _ActiveWorkspace.Name;
                    EventSystem.Publish<Workspace>(value);
                }
                    

            }
        }

        /// <summary>
        /// Workspace Name for the Textbox
        /// </summary>
        public string WorkspaceName
        {
            get => _WorkspaceName;
            set
            {
                if (_WorkspaceName == value)
                    return;

                _WorkspaceName = value;
                OnPropertyChanged("WorkspaceName");
            }
        }

        public MainUIViewModel MUVM
        {
            get => _MUVM;
            set
            {
                if (_MUVM == value)
                    return;

                _MUVM = value;
                OnPropertyChanged("MUVM");
            }
        }
        #endregion

        public ILog log { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public WorkspaceViewModel(MainUIViewModel _MUVM, ILog _log)
        {
            MUVM = _MUVM;
            Workspaces = MUVM.Workspaces;
            ActiveWorkspace = MUVM.ActiveWorkspace;
            log = _log;

            EventSystem.Subscribe<ModalEvent>(ModalEvent);

        }

        #region Modal
        public void ModalEvent(ModalEvent meu)
        {
            switch (meu.EventName)
            {
                case "DeleteWorkspace":
                    if(meu.Action == "OK")
                    {
                        DeleteWorkspace();
                    }
                    break;
                case "EmptyWorkspace":
                    if (meu.Action == "OK")
                    {
                        EmptyWorkspace();
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Actions
        public void AskDeleteWorkspace()
        {
            if (ActiveWorkspace != null)
            {
                if (ActiveWorkspace.ID != 0)
                {
                    ModalEvent meuh = new ModalEvent()
                    {
                        Action = "Show",
                        EventName = "DeleteWorkspace",
                        Type = ModalType.OkCancel,
                        Title = "Workspace Deletion",
                        Content = "Are you sure you want to delete this workspace?",
                        OkButtonText = "Yes I'm sure",
                        CancelButtonText = "No"
                    };
                    EventSystem.Publish(meuh);
                }
            }
        }
        public void AskEmptyWorkspace()
        {
            if (ActiveWorkspace != null)
            {
                ModalEvent meuh = new ModalEvent()
                {
                    Action = "Show",
                    EventName = "EmptyWorkspace",
                    Type = ModalType.OkCancel,
                    Title = "Workspace Reset",
                    Content = "Are you sure you want to empty this workspace?",
                    OkButtonText = "Yes I'm sure",
                    CancelButtonText = "No"
                };
                EventSystem.Publish(meuh);
            }
        }

        /// <summary>
        /// Saves the workspace Name
        /// </summary>
        public void SaveWorkspaceName()
        {
            if (ActiveWorkspace != null)
            {
                ActiveWorkspace.Name = WorkspaceName;
                JSonHelper.SaveWorkspaces((Workspaces));
            }
        }

        /// <summary>
        /// Adds a Workspace
        /// </summary>
        public void AddWorkspace()
        {
            Workspace newWorkspace = new Workspace() { Name = "New Workspace", ID = IDHelper.getNewWorkspaceID(), Associations = new ObservableCollection<Association>(), BuildDate = "" };
            Workspaces.Add(newWorkspace);
            JSonHelper.SaveWorkspaces(Workspaces);
        }

        /// <summary>
        /// Deletes the selected workspace
        /// </summary>
        public void DeleteWorkspace()
        {
            Workspaces.Remove(ActiveWorkspace);
            ActiveWorkspace = Workspaces[0];
            JSonHelper.SaveWorkspaces(Workspaces);
        }

        /// <summary>
        /// Empties the selected workspace
        /// </summary>
        public void EmptyWorkspace()
        {
            ActiveWorkspace.Associations = new ObservableCollection<Association>();
            JSonHelper.SaveWorkspaces(Workspaces);
        }

        /// <summary>
        /// Duplicates a workspace into a new one
        /// </summary>
        public void DuplicateWorkspace()
        {
            Workspace Clone = new Workspace() { Name = String.Format("{0} - Copy", ActiveWorkspace.Name), ID = IDHelper.getNewWorkspaceID(), Associations = ActiveWorkspace.Associations, BuildDate = "" };
            Workspaces.Add(Clone);
            JSonHelper.SaveWorkspaces(Workspaces);
        }

        /// <summary>
        /// Shares a workspace for other users to import
        /// </summary>
        public void ShareWorkspace()
        {
            if (!ActiveWorkspace.Shared)
            {
                ActiveWorkspace.Shared = true;
                ActiveWorkspace.UniqueShareID = IDHelper.getWorkspaceUniqueID();
                JSonHelper.SaveWorkspaces(MUVM.Workspaces);
            }
            
            ObservableCollection<ContentItem> WorkspaceContentItems = new ObservableCollection<ContentItem>();
            ObservableCollection<LibraryItem> WorkspaceLibraryItems = new ObservableCollection<LibraryItem>();

            foreach(Association ass in ActiveWorkspace.Associations)
            {
                if(!WorkspaceContentItems.Any(c => c.ID == ass.ContentItemID))
                {
                    ContentItem cit = MUVM.ContentItems.Single(ci => ci.ID == ass.ContentItemID);
                    WorkspaceContentItems.Add(cit);
                    if (!WorkspaceLibraryItems.Any(c => c.ID == cit.LibraryItemID))
                    {
                        WorkspaceLibraryItems.Add(MUVM.Library.Single(li => li.ID == cit.LibraryItemID));
                    }
                }
            }

            ShareableWorkspace SharedWorkspace = new ShareableWorkspace()
            {
                Workspace = ActiveWorkspace,
                ContentItems = WorkspaceContentItems,
                LibraryItems = WorkspaceLibraryItems
            };
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            JSonHelper.SaveSharedWorkspaces(SharedWorkspace, desktopPath+ @"\SharedWorkspace.json");
            System.Windows.MessageBox.Show("A new file has been saved on your Desktop, please share it with a friend for import !");
        }

        /// <summary>
        /// Imports a shared workspace
        /// </summary>
        public void ImportWorkspace()
        {
            string pathselected = null;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = @"F:\FakeSwitch\";
                openFileDialog.Filter = "Quasar Workspace File (*.json)|*.json";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    pathselected = openFileDialog.FileName;

                }
            }
            if(pathselected != null)
            {
                try
                {
                    //Reading file
                    ShareableWorkspace SW = JSonHelper.GetSharedWorkspace(true, pathselected);
                    if(SW != null)
                    {
                        ObservableCollection<LibraryItem> DownloadList = new ObservableCollection<LibraryItem>();
                        ObservableCollection<LibraryItem> UpdateList = new ObservableCollection<LibraryItem>();
                        foreach(LibraryItem SharedLibraryItem in SW.LibraryItems)
                        {
                            LibraryItem Search = MUVM.Library.SingleOrDefault(s => s.ID == SharedLibraryItem.ID);
                            if(Search == null)
                            {
                                DownloadList.Add(SharedLibraryItem);
                            }
                            else
                            {
                                UpdateList.Add(SharedLibraryItem);
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    System.Windows.MessageBox.Show("Sorry, Quasar could not process this file");
                }
            }
        }

        #endregion

        #region User Actions

        #endregion

    }
}
