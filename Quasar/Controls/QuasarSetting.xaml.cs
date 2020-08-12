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

            if(dat.Data != null)
            {
                ComboVisible = true;
            }
            else
            {
                ComboVisible = false;
            }

            InitializeComponent();
        }
    }

    public class QuasarSettingData
    {
        public string SettingName { get; set; }
        public bool SettingCheck { get; set; }
        public List<QuasarSettingComboData> Data { get; set; }
    }

    public class QuasarSettingComboData
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
