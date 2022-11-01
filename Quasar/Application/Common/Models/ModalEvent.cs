using Quasar.Helpers;

namespace Quasar.Common
{
    public class ModalEvent
    {
        public string EventName { get; set; }
        public string Action { get; set; }
        public ModalType Type { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string OkButtonText { get; set; }
        public string CancelButtonText { get; set; }

    }

    public enum ModalType { Warning, OkCancel, Loader}
    public enum Modal { TransferWarning, DeleteMod,AskMoveInstall, MoveInstall, Update, RecoverInstallation, Exit, ImportFiles, Shutdown }
    public enum Help { Library, Skin,Stage, Music, Other, Settings}
    public static class Popup
    {
        //Public accessible functions
        public static void CallModal(Modal _modal)
        {
            switch (_modal)
            {
                case Modal.TransferWarning:
                    ShowModal("TransferWarning", ModalType.OkCancel, Properties.Resources.Library_Modal_TransferWarningTitle, 
                        Properties.Resources.Library_Modal_TransferWarningContent, Properties.Resources.Library_Modal_TransferWarningOK, 
                        Properties.Resources.Library_Modal_TransferWarningCancel);
                    break;
                case Modal.DeleteMod:
                    ShowModal("DeleteMod", ModalType.OkCancel, Properties.Resources.Library_Modal_DeleteTitle,
                        Properties.Resources.Library_Modal_DeleteContent, Properties.Resources.Library_Modal_DeleteOK,
                        Properties.Resources.Library_Modal_DeleteCancel);
                    break;
                case Modal.AskMoveInstall:
                    ShowModal("AskMoveInstall", ModalType.OkCancel, Properties.Resources.Settings_Modal_Title_AskMoveInstall,
                        Properties.Resources.Settings_Modal_Content_AskMoveInstall);
                    break;
                case Modal.MoveInstall:
                    ShowModal("MoveInstall", ModalType.OkCancel, Properties.Resources.Settings_Modal_Title_MovingInstall,
                        Properties.Resources.Settings_Modal_Content_MovingInstall, Properties.Resources.Settings_Modal_OkButton_MovingInstall,
                        Properties.Resources.Modal_Label_DefaultCancel);
                    break;
                case Modal.Update:
                    ShowModal("Update", ModalType.Loader, Properties.Resources.MainUI_Modal_UpdateProgressTitle, Properties.Resources.MainUI_Modal_UpdateProgressContent);
                    break;
                case Modal.RecoverInstallation:
                    ShowModal("RecoverInstallation", ModalType.Loader, Properties.Resources.MainUI_Modal_RecoverProgressTitle, Properties.Resources.MainUI_Modal_RecoverProgressContent);
                    break;
                case Modal.Exit:
                    ShowModal("Exit", ModalType.OkCancel, Properties.Resources.Quasar_Modal_Title_Exit, Properties.Resources.Quasar_Modal_Content_Exit);
                    break;
                case Modal.ImportFiles:
                    ShowModal("ImportFiles", ModalType.Loader, Properties.Resources.Library_Modal_Title_ImportFiles, Properties.Resources.Library_Modal_Content_ImportFiles);
                    break;
                case Modal.Shutdown:
                    ShowModal("Shutdown",ModalType.Warning,Properties.Resources.Settings_Modal_Title_ShutdownWarning,Properties.Resources.Settings_Modal_Content_ShutdownWarning,
                        Properties.Resources.Settings_Modal_Button_ShutdownWarning,Properties.Resources.Modal_Label_DefaultCancel);
                    break;
            }
        }

        public static void CallHelp(Help _modal)
        {
            switch (_modal)
            {
                case Help.Library:
                    ShowModal("LibraryHelp", ModalType.Warning, Properties.Resources.Help_Library_Title, Properties.Resources.Help_Library_Content, true);
                    break;
                case Help.Skin:
                    ShowModal("SkinHelp", ModalType.Warning, Properties.Resources.Help_Skin_Title, Properties.Resources.Help_Skin_Content, true);
                    break;
                case Help.Stage:
                    ShowModal("StageHelp", ModalType.Warning, Properties.Resources.Help_Stage_Title, Properties.Resources.Help_Stage_Content, true);
                    break;
                case Help.Music:
                    ShowModal("MusicHelp", ModalType.Warning, Properties.Resources.Help_Music_Title, Properties.Resources.Help_Music_Content, true);
                    break;
                case Help.Other:
                    ShowModal("OtherHelp", ModalType.Warning, Properties.Resources.Help_Other_Title, Properties.Resources.Help_Other_Content, true);
                    break;
                case Help.Settings:
                    ShowModal("SettingsHelp", ModalType.Warning, Properties.Resources.Help_Settings_Title, Properties.Resources.Help_Settings_Content, true);
                    break;
             }
        }
        public static void UpdateModalStatus(Modal _modal, bool _operation_success)
        {
            switch (_modal)
            {
                case Modal.MoveInstall:
                    SendModalStatus("MoveInstall", _operation_success);
                    break;
                case Modal.ImportFiles:
                    if (_operation_success)
                    {
                        SendModalStatus("ImportFiles", _operation_success, Properties.Resources.Library_Modal_Title_ImportFiles_Success, Properties.Resources.Library_Modal_Content_ImportFiles_Success);
                    }
                    else
                    {
                        SendModalStatus("ImportFiles", _operation_success, Properties.Resources.Library_Modal_Title_ImportFiles_Failure, Properties.Resources.Library_Modal_Content_ImportFiles_Failure);
                    }
                    break;
                case Modal.RecoverInstallation:
                    SendModalStatus("RecoverInstallation",_operation_success);
                    break;
                case Modal.Update:
                    SendModalStatus("Update",_operation_success,Properties.Resources.MainUI_Modal_UpdateFinishedTitle,Properties.Resources.MainUI_Modal_UpdateFinishedContent);
                    break;
            }
        }
        
        //Functions to call a Modal to the front
        private static void ShowModal(string _event_name, ModalType _type, string _title, string _content, bool _help = false)
        {
            ModalEvent meuh = new ModalEvent()
            {
                EventName = _event_name,
                Type = _type,
                Action = _help ? "ShowHelp" : "Show",
                Title = _title,
                Content = _content,
                OkButtonText = Properties.Resources.Modal_Label_DefaultOK,
                CancelButtonText = Properties.Resources.Modal_Label_DefaultCancel

            };

            EventSystem.Publish<ModalEvent>(meuh);
        }
        private static void ShowModal(string _event_name, ModalType _type, string _title, string _content, string _ok_button_text, string _cancel_button_text)
        {
            ModalEvent meuh = new ModalEvent()
            {
                EventName = _event_name,
                Type = _type,
                Action = "Show",
                Title = _title,
                Content = _content,
                OkButtonText = _ok_button_text,
                CancelButtonText = _cancel_button_text

            };

            EventSystem.Publish<ModalEvent>(meuh);
        }

        //Functions to update the Modal Status
        private static void SendModalStatus(string _event_name, bool _operation_success)
        {
            ModalEvent Meuh = new ModalEvent()
            {
                EventName = _event_name,
                Action = _operation_success ? "LoadOK" : "LoadKO",

            };

            EventSystem.Publish<ModalEvent>(Meuh);
        }
        private static void SendModalStatus(string _event_name, bool _operation_success, string _title, string _content)
        {
            ModalEvent Meuh = new ModalEvent()
            {
                EventName = _event_name,
                Action = _operation_success ? "LoadOK" : "LoadKO",
                Title = _title,
                Content = _content,
            };

            EventSystem.Publish<ModalEvent>(Meuh);
        }

        private static void SendModalStatus(string _event_name, bool _operation_success, string _title, string _content, string _ok_button_text, string _cancel_button_text)
        {
            ModalEvent Meuh = new ModalEvent()
            {
                EventName = _event_name,
                Action = _operation_success ? "LoadOK" : "LoadKO",
                Title = _title,
                Content = _content,
                OkButtonText = _ok_button_text,
                CancelButtonText = _cancel_button_text

            };

            EventSystem.Publish<ModalEvent>(Meuh);
        }
    }
}
