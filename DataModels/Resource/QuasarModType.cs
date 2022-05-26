using DataModels.Common;
using System.Collections.ObjectModel;

namespace DataModels.Resource
{
    public class QuasarModType
    {
        public int ID { get; set; }
        public int SlotCount { get; set; }
        public int TypePriority { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        public int GameElementFamilyID { get; set; }
        public bool IgnoreGameElementFamily { get; set; }
        public bool NoGameElement { get; set; }
        public bool IsExternal { get; set; }
        public string ExternalFolderPath { get; set; }
        public ObservableCollection<QuasarModTypeFileDefinition> QuasarModTypeFileDefinitions { get; set; }

    }

    public class QuasarModTypeGroup : ObservableObject
    {
        #region Fields
        private string _Name { get; set; }
        private ObservableCollection<QuasarModType> _QuasarModTypeCollection { get; set; }
        #endregion

        #region Properties
        public string Name
        {
            get => _Name;
            set
            {
                if (_Name == value)
                    return;

                _Name = value;
                OnPropertyChanged("Name");
            }
        }
        public ObservableCollection<QuasarModType> QuasarModTypeCollection
        {
            get => _QuasarModTypeCollection;
            set
            {
                if (_QuasarModTypeCollection == value)
                    return;

                _QuasarModTypeCollection = value;
                OnPropertyChanged("QuasarModTypeCollection");
            }
        }
        #endregion



    }
    public class QuasarModTypeFileDefinition
    {
        public int ID { get; set; }
        public string SearchPath { get; set; }
        public string SearchFileName { get; set; }
        public string OutputPath { get; set; }
        public bool FolderMatch => @"{ModName}/" + SearchPath == OutputPath;
        public int FilePriority { get; set; }
        public ObservableCollection<QuasarModTypeBuilderDefinition> QuasarModTypeBuilderDefinitions { get; set; }
    }
    public class QuasarModTypeBuilderDefinition
    {
        public int ModLoaderID { get; set; }
        public string OutputPath { get; set; }
        public string OutputFileName { get; set; }
    }
}
