using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Windows;
using DataModels.User;
using Quasar.Common;
using Quasar.Common.Models;
using Quasar.Controls;
using Quasar.Helpers;
using Quasar.MainUI.ViewModels;
using Workshop.Associations;

namespace Quasar
{
    public partial class MainWindow : Window
    {
        public MainUIViewModel MUVM { get; set; }

        /// <summary>
        /// Basic constructor
        /// </summary>
        public MainWindow()
        {
            MUVM = new MainUIViewModel();

            //Aww, here we go again
            InitializeComponent();

            QuasarGrid.DataContext = MUVM;


            EventSystem.Subscribe<ModalEvent>(ProcessIncomingModalEvent);
            
        }

        /// <summary>
        /// Asks if the user really wants to quit Quasar while it's doing stuff
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AskQuitQuasar(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MUVM.TabLocked)
            {
                e.Cancel = true;

                Popup.CallModal(Modal.Exit);
            }
        }

        /// <summary>
        /// Processes any incoming Modal Events
        /// </summary>
        /// <param name="meuh"></param>
        public void ProcessIncomingModalEvent(ModalEvent meuh)
        {

            if (meuh.EventName == "Exit")
            {
                if(meuh.Action == "OK")
                {
                    //Quits the app
                    System.Environment.Exit(0);
                }
            }
        }


    }
    
}
