using DataModels.Resource;
using DataModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Common
{
    public class ScanTestResult : ObservableObject
    {
        private ScanFile _ScanFile { get; set; }
        private ContentItem _ContentItem { get; set; }
        private QuasarModType _QuasarModType { get; set; }
        private QuasarModTypeFileDefinition _QuasarModTypeFileDefinition { get; set; }
        private GameElement _GameElement { get; set; }
        private string _Output { get; set; }


        public ScanFile ScanFile
        {
            get => _ScanFile;
            set
            {
                _ScanFile = value;
                OnPropertyChanged("ScanFile");
            }
        }
        public ContentItem ContentItem
        {
            get => _ContentItem;
            set
            {
                _ContentItem = value;
                OnPropertyChanged("ContentItem");
            }
        }
        public QuasarModType QuasarModType
        {
            get => _QuasarModType;
            set
            {
                _QuasarModType = value;
                OnPropertyChanged("QuasarModType");
            }
        }
        public QuasarModTypeFileDefinition QuasarModTypeFileDefinition
        {
            get => _QuasarModTypeFileDefinition;
            set
            {
                _QuasarModTypeFileDefinition = value;
                OnPropertyChanged("QuasarModTypeFileDefinition");
            }
        }
        public GameElement GameElement
        {
            get => _GameElement;
            set
            {
                _GameElement = value;
                OnPropertyChanged("GameElement");
            }
        }
        public string Output
        {
            get => _Output;
            set
            {
                _Output = value;
                OnPropertyChanged("Output");
            }
        }
        public bool NoContent => ContentItem == null;
        public bool NoGameElement => GameElement == null;

    }
}
