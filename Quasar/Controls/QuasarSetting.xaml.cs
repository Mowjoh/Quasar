using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Quasar.Controls
{
    /// <summary>
    /// Interaction logic for QuasarSetting.xaml
    /// </summary>
    public partial class QuasarSetting : UserControl, INotifyPropertyChanged
    {
        public EventHandler SettingsChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private QuasarSettingData _LocalData;
        public QuasarSettingData LocalData
        {
            get
            {
                return _LocalData;
            }
            set
            {
                _LocalData = value;
                OnPropertyChanged("LocalData");
            }
        }

        private bool _ComboVisible;
        public bool ComboVisible
        {
            get
            {
                return _ComboVisible;
            }
            set
            {
                _ComboVisible = value;
                CheckVisible = !value;
                OnPropertyChanged("ComboVisible");
            }
        }
        private bool _CheckVisible;
        public bool CheckVisible
        {
            get
            {
                return _CheckVisible;
            }
            set
            {
                _CheckVisible = value;
                OnPropertyChanged("CheckVisible");
            }
        }

        public QuasarSetting()
        {
            InitializeComponent();
        }

        public QuasarSetting(QuasarSettingData dat)
        {
            
            LocalData = dat;

            if(dat.NameOnly == true)
            {
                ComboVisible = false;
                CheckVisible = false;
            }
            else
            {
                if (dat.Data != null)
                {
                    ComboVisible = true;
                }
                else
                {
                    ComboVisible = false;
                }
            }
            

            InitializeComponent();
        }

        private void SettingCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(LocalData.Reference != null)
            {
                Properties.Settings.Default[LocalData.Reference] = SettingCheckBox.IsChecked;
                Properties.Settings.Default.Save();
                if(SettingsChanged!= null)
                {
                    this.SettingsChanged(this, new EventArgs());
                }
            }
        }
    }

    public class QuasarSettingData
    {
        public string Reference { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
        public bool SettingCheck { get; set; }
        public bool NameOnly { get; set; }
        public List<QuasarSettingComboData> Data { get; set; }
    }

    public class QuasarSettingComboData
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
