﻿using Quasar.Controls.Content.ViewModels;
using Quasar.Data.V2;
using Quasar.FileSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for ModListItem.xaml
    /// </summary>
    public partial class ContentListItem : UserControl
    {
        public ContentListItemViewModel CLIVM { get; set; }

        public ContentListItem(ContentItem ContentItem, LibraryItem LibraryItem, QuasarModType qmt, List<GameElement> GameElements, int colorID)
        {
            
            CLIVM = new ContentListItemViewModel(ContentItem, LibraryItem, qmt, GameElements, colorID);
            InitializeComponent();
            DataContext = CLIVM;
        }

        public void setColor(int color)
        {/*
            SolidColorBrush brush;
            switch (color)
            {
                case 0:
                    ContentOddEven = FindResource("ColorA") as SolidColorBrush;
                    break;
                case 1:
                    ContentOddEven = FindResource("ColorB") as SolidColorBrush;
                    break;
                default:
                    ContentOddEven = FindResource("ColorA") as SolidColorBrush;
                    break;
            }*/
        }

        private void ExpandRetract_Click(object sender, RoutedEventArgs e)
        {/*
            Smol = true;*/
        }

        private void SaveContentMapping(object sender, RoutedEventArgs e)
        {
                   
        }
    }
}
