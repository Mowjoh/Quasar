﻿using Quasar.Controls.Common.Models;
using Quasar.Internal;
using Quasar.Internal.Tools;
using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Quasar.Controls.Settings.Workspaces.ViewModels
{
    public class WorkspaceViewModel : ObservableObject
    {
        #region Fields
        ObservableCollection<Workspace> _Workspaces { get; set; }

        private Workspace _ActiveWorkspace {get; set;}

        private string _WorkspaceName { get; set; }
        private ICommand _RenameWorkspaceCommand { get; set; }
        private ICommand _AddWorkspaceCommand { get; set; }
        private ICommand _DeleteWorkspaceCommand { get; set; }
        private ICommand _EmptyWorkspaceCommand { get; set; }
        private ICommand _DuplicateWorkspaceCommand { get; set; }
        private ICommand _ActivateWorkspaceCommand { get; set; }
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
                    _DeleteWorkspaceCommand = new RelayCommand(param => DeleteWorkspace());
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
                    _EmptyWorkspaceCommand = new RelayCommand(param => EmptyWorkspace());
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
        public ICommand ActivateWorkspaceCommand
        {
            get
            {
                if (_ActivateWorkspaceCommand == null)
                {
                    _ActivateWorkspaceCommand = new RelayCommand(param => ActivateWorkspace());
                }
                return _ActivateWorkspaceCommand;
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

        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public WorkspaceViewModel(ObservableCollection<Workspace> _Workspaces, Workspace _ActiveWorkspace)
        {
            Workspaces = _Workspaces;
            ActiveWorkspace = _ActiveWorkspace;
        }


        #region Actions
        /// <summary>
        /// Saves the workspace Name
        /// </summary>
        public void SaveWorkspaceName()
        {
            if (ActiveWorkspace != null)
            {
                ActiveWorkspace.Name = WorkspaceName;
                WorkspaceXML.WriteWorkspaces(Workspaces.ToList());
            }
        }

        /// <summary>
        /// Adds a Workspace
        /// </summary>
        public void AddWorkspace()
        {
            Workspace newWorkspace = new Workspace() { Name = "New Workspace", ID = IDGenerator.getNewWorkspaceID(), Associations = new List<Association>(), Built = false, BuildDate = "" };
            Workspaces.Add(newWorkspace);
            WorkspaceXML.WriteWorkspaces(Workspaces.ToList());
        }

        /// <summary>
        /// Deletes the selected workspace
        /// </summary>
        public void DeleteWorkspace()
        {
            if (ActiveWorkspace != null)
            {
                if (ActiveWorkspace.ID != 0)
                {
                    bool proceed = false;
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this workspace ?", "Workspace Deletion", MessageBoxButton.YesNo);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            proceed = true;
                            break;
                        case MessageBoxResult.No:
                            break;
                    }
                    if (proceed)
                    {
                        Workspaces.Remove(ActiveWorkspace);
                        ActiveWorkspace = Workspaces[0];
                        WorkspaceXML.WriteWorkspaces(Workspaces.ToList());
                    }
                }
            }
        }

        /// <summary>
        /// Empties the selected workspace
        /// </summary>
        public void EmptyWorkspace()
        {
            if(ActiveWorkspace != null)
            {
                bool proceed = false;
                MessageBoxResult result = MessageBox.Show("Are you sure you want to empty this workspace ?", "Workspace Reset", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        proceed = true;
                        break;
                    case MessageBoxResult.No:
                        break;
                }
                if (proceed)
                {
                    ActiveWorkspace.Associations = new List<Association>();
                    WorkspaceXML.WriteWorkspaces(Workspaces.ToList());
                }
                
            }
        }

        /// <summary>
        /// Duplicates a workspace into a new one
        /// </summary>
        public void DuplicateWorkspace()
        {
            Workspace Clone = new Workspace() { Name = String.Format("{0} - Copy", ActiveWorkspace.Name), ID = IDGenerator.getNewWorkspaceID(), Associations = ActiveWorkspace.Associations, Built = false, BuildDate = "" };
            Workspaces.Add(Clone);
            WorkspaceXML.WriteWorkspaces(Workspaces.ToList());
        }

        /// <summary>
        /// Activates a workspace for ARCropolis
        /// </summary>
        public void ActivateWorkspace()
        {

        }
        #endregion

    }
}