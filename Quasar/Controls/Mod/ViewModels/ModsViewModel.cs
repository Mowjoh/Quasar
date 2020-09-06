using Quasar.Controls.Common.Models;
using Quasar.Controls.Mod.Models;
using Quasar.Controls.Mod.ViewModels;
using Quasar.Internal;
using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Quasar.Controls.ModManagement.ViewModels
{
    class ModsViewModel : ObservableObject
    {
        #region Fields
        private ObservableCollection<ModListItem> _ModListItems { get; set; }
        private ObservableCollection<LibraryMod> _Mods { get; set; }
        private ObservableCollection<ContentMapping> _ContentMappings { get; set; }
        private ObservableCollection<Workspace> _Workspaces { get; set; }
        private ObservableCollection<Game> _Games { get; set; }
        private ModListItem _SelectedModListItem { get; set; }
        public ObservableCollection<string> _QuasarDownloads { get; set; }
        #endregion

        #region Properties
        public ObservableCollection<ModListItem> ModListItems
        {
            get => _ModListItems;
            set
            {
                if (_ModListItems == value)
                    return;

                _ModListItems = value;
                OnPropertyChanged("ModListItems");
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
        public ObservableCollection<ContentMapping> ContentMappings
        {
            get => _ContentMappings;
            set
            {
                if (_ContentMappings == value)
                    return;

                _ContentMappings = value;
                OnPropertyChanged("ContentMappings");
            }
        }
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
        public ModListItem SelectedModListItem
        {
            get => _SelectedModListItem;
            set
            {
                if (_SelectedModListItem == value)
                    return;

                if(_SelectedModListItem != null)
                {
                    _SelectedModListItem.ModListItemViewModel.Smol = true;
                }

                _SelectedModListItem = value;
                _SelectedModListItem.ModListItemViewModel.Smol = false;
                OnPropertyChanged("SelectedModListItem");
            }
        }
        #endregion

        public ModsViewModel(ObservableCollection<LibraryMod> _Mods, ObservableCollection<Game> _Games, ObservableCollection<ContentMapping> _ContentMappings, ObservableCollection<Workspace> _Workspaces)
        {
            Mods = _Mods;
            Games = _Games;
            ContentMappings = _ContentMappings;
            Workspaces = _Workspaces;

            ParseModListItems();

            EventSystem.Subscribe<ModListItemViewModel>(GetModListElementTrigger);
            EventSystem.Subscribe<QuasarDownload>(Download);
        }

        #region Actions
        public void Download(QuasarDownload download)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                ModListItem mli = new ModListItem(download.QuasarURL);

                ModListItems.Add(mli);
            });

        }
        public void ParseModListItems()
        {
            ModListItems = new ObservableCollection<ModListItem>();

            foreach (LibraryMod lm in Mods)
            {
                Game gamu = Games.Single(g => g.ID == lm.GameID);

                ModListItem mli = new ModListItem(_OperationActive: false, _LibraryMod: lm, _Game: gamu);

                ModListItems.Add(mli);
            }
        }

        public void GetModListElementTrigger(ModListItemViewModel ModListItemViewModel)
        {
            ModListItem MLI = ModListItems.Single(m => m.ModListItemViewModel == ModListItemViewModel);
            switch (ModListItemViewModel.ActionRequested)
            {
                case "Delete":
                    DeleteMod(MLI);
                    break;
                case "Add":
                    AddMod(MLI);
                    break;
                default:
                    break;
            }
        }

        public void DeleteMod(ModListItem item)
        {
            Boolean proceed = false;
            if (!Properties.Settings.Default.SupressModDeletion)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this mod ?", "Mod Deletion", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        proceed = true;
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }

            if (proceed || Properties.Settings.Default.SupressModDeletion)
            {
                //Removing from ContentMappings
                List<ContentMapping> relatedMappings = ContentMappings.Where(cm => cm.ModID == item.ModListItemViewModel.LibraryMod.ID).ToList();
                foreach (ContentMapping cm in relatedMappings)
                {
                    foreach(Workspace w in Workspaces)
                    {
                        List<Association> associations = w.Associations.FindAll(ass => ass.ContentMappingID == cm.ID);
                        if (associations != null)
                        {
                            foreach (Association ass in associations)
                            {
                                w.Associations.Remove(ass);
                            }
                        }
                        ContentMappings.Remove(cm);
                    }
                    
                }

                ModListItems.Remove(item);
                Mods.Remove(item.ModListItemViewModel.LibraryMod);

                //Writing changes
                Library.WriteModListFile(Mods.ToList());
                ContentXML.WriteContentMappingListFile(ContentMappings.ToList());
                WorkspaceXML.WriteWorkspaces(Workspaces.ToList());
            }
        }

        public void AddMod(ModListItem MLI)
        {

        }
        #endregion
    }
}
