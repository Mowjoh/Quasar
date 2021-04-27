using System.Windows;
using Quasar.Common.Models;
using Quasar.Helpers;
using Quasar.MainUI.ViewModels;

namespace Quasar
{
    public partial class MainWindow : Window
    {
        MainUIViewModel MUVM { get; set; }

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

                ModalEvent meuh = new ModalEvent()
                {
                    Action = "Show",
                    EventName = "QuasarClose",
                    Title = "Are you sure you want to Exit ?",
                    Content = "Quasar is currently active.\rExiting now will probably result in some errors later on",
                    OkButtonText = "I'm sure",
                    CancelButtonText = "Nope",
                    Type = ModalType.OkCancel
                };

                EventSystem.Publish<ModalEvent>(meuh);
            }
        }

        /// <summary>
        /// Processes any incoming Modal Events
        /// </summary>
        /// <param name="meuh"></param>
        public void ProcessIncomingModalEvent(ModalEvent meuh)
        {
            if(meuh.EventName == "QuasarClose")
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
