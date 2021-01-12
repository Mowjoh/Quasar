using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Data.V2
{
    public class QuasarModType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        public int GameElementFamilyID { get; set; }
        public bool IgnoreGameElementFamily { get; set; }
        public bool NoGameElement { get; set; }
        public bool IsExternal { get; set; }
        public string ExternalFolderPath { get; set; }
        public ObservableCollection<QuasarModTypeFileDefinition> QuasarModTypeFileDefinitions { get; set; }

    }

    public class QuasarModTypeFileDefinition
    {
        public int ID { get; set; }
        public string SearchPath { get; set; }
        public string SearchFileName { get; set; }
        public ObservableCollection<QuasarModTypeBuilderDefinition> QuasarModTypeBuilderDefinitions { get; set; }
    }

    public class QuasarModTypeBuilderDefinition
    {
        public int ModLoaderID { get; set; }
        public string OutputPath { get; set; }
        public string OutputFileName { get; set; }
    }
}
