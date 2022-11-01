using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DataModels.Common;
using DataModels.Resource;
using DataModels.User;
using GongSolutions.Wpf.DragDrop;
using Quasar.Common.Models;
using Quasar.Common.Views;
using Quasar.Helpers;

namespace Quasar.Common
{
    public class SlotViewModel : ObservableObject, IDropTarget
    {
        private bool _Source { get; set; }

        public bool Source
        {
            get => _Source;
            set
            {
                _Source = value;
                OnPropertyChanged("Source");
            }
        }
        private string _Image { get; set; }

        public string Image
        {
            get => _Image;
            set
            {
                _Image = value;
                OnPropertyChanged("Image");
            }
        }
        public SlotModel _SlotModel { get; set; }

        public SlotModel SlotModel
        {
            get => _SlotModel;
            set
            {
                _SlotModel = value;
                OnPropertyChanged("SlotModel");
                OnPropertyChanged("SlotText");
                OnPropertyChanged("CurrentModuloSlot");
                OnPropertyChanged("BaseImage");
            }
        }

        public int CurrentModuloSlot => SlotModel.Slot < 8 ? SlotModel.Slot : SlotModel.Slot % 8;
        public string SlotText => $"Player {SlotModel.Slot + 1}";

        public string ScreenshotName => SlotModel.GameElement == null ? "" : SlotModel.GameElement.GameFolderName.Split(";").Length > 0
            ? SlotModel.GameElement.GameFolderName.Split(";")[0]
            : SlotModel.GameElement.GameFolderName;

        public string BaseImage => $@"../../../Resources/images/Characters/{ScreenshotName}_0{CurrentModuloSlot}.png";

        public void Refresh()
        {
            OnPropertyChanged("SlotModel");
            OnPropertyChanged("SlotText");
            OnPropertyChanged("CurrentModuloSlot");
            OnPropertyChanged("BaseImage");
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is ContentItemView)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            EventSystem.Publish<(SlotModel, ContentItemView)>((SlotModel, (ContentItemView)dropInfo.Data));
        }
    }
}
