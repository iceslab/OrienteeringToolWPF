using GecoSI.Net;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Windows;
using OrienteeringToolWPF.Windows.Dialogs;
using OrienteeringToolWPF.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Windows;

namespace OrienteeringToolWPF.Utils
{
    static class MessageUtils
    {
        public static void ShowValidatorErrors(DependencyObject obj, ErrorList errors)
        {
            MessageBox.Show(GetWindow(obj),
                            Properties.Resources.Errors + "\n" + errors.ToString(),
                            Properties.Resources.InvalidDataTitle,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
        }

        public static void ShowExtendedValidatorErrors(DependencyObject obj, ErrorList errors, string extendedError)
        {
            // TODO: Change to collapsable textbox
            MessageBox.Show(GetWindow(obj),
                            Properties.Resources.Errors + "\n" + errors.ToString() +
                            "\n\n" + extendedError,
                            Properties.Resources.InvalidDataTitle,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
        }

        public static void ShowException(DependencyObject obj, string description, Exception ex)
        {
            ExceptionMessageBox.Show(GetWindow(obj),
                description,
                "Wystąpił wyjątek",
                ex);
        }

        public static void ShowSiHandlerError(CommStatus errorStatus, string errorMessage)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MessageBox.Show(GetWindow(null),
                                Properties.Resources.OriginalMessage + "\n" + errorMessage,
                                Properties.Resources.SiHandlerError,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }));

        }

        public static bool ShowDeleteWarning(DependencyObject obj)
        {
            var mbr = MessageBox.Show(GetWindow(obj),
                                    Properties.Resources.DatabaseCascadeDeleteWarning,
                                    Properties.Resources.Warning,
                                    MessageBoxButton.OKCancel,
                                    MessageBoxImage.Warning);
            return GetBoolFromMessageBoxResult(mbr);
        }

        public static bool ShowOverwriteWarning(DependencyObject obj)
        {
            var mbr = MessageBox.Show(GetWindow(obj),
                                    Properties.Resources.DatabaseOverwriteWarning,
                                    Properties.Resources.Warning,
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Warning);
            return GetBoolFromMessageBoxResult(mbr);
        }

        public static bool ShowConnectionNeeddedInfo(DependencyObject obj)
        {
            var mbr = MessageBox.Show(GetWindow(obj),
                        Properties.Resources.ConnectionNeeddedInfo,
                        Properties.Resources.ConnectionNotConnected,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information,
                        MessageBoxResult.Yes);
            return GetBoolFromMessageBoxResult(mbr);
        }

        public static void ShowCannotStartFinishedInfo(DependencyObject obj)
        {
            MessageBox.Show(GetWindow(obj),
                    Properties.Resources.CompetitionCannotStart,
                    Properties.Resources.CompetitionHasFinished,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
        }

        /// <summary>Prompts user for connection to station if not connected</summary> 
        /// <param name="obj">Represents control which called this method (used for getting window)</param>
        /// <returns>true when connected, false when user refuses to connect</returns>
        public static bool PromptForConnection(DependencyObject obj)
        {
            while (MainWindow.Handler.NotIsConnected)
            {
                var siConnectionDialog = new SiConnectionDialog();
                siConnectionDialog.Owner = GetWindow(obj);
                if (siConnectionDialog.ShowDialog() != true)
                {
                    // User refuses to connect
                    if (ShowConnectionNeeddedInfo(obj) == false)
                        return false;
                }
            }

            return true;
        }

        public static bool ShowStartBeforeTimeWarning(DependencyObject obj)
        {
            var mbr = MessageBox.Show(GetWindow(obj),
                        Properties.Resources.StartTimeNotPassedWarning,
                        Properties.Resources.Warning,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning,
                        MessageBoxResult.No);
            return GetBoolFromMessageBoxResult(mbr);
        }

        private static void ShowCompetitorsIncosistencyError(DependencyObject obj)
        {
            // Change to resources
            MessageBox.Show(GetWindow(obj),
                        "Nie wszyscy zawodnicy mają przydzielony chip",
                        "Błąd",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
        }

        public static bool CheckCompetitorsCosistency(DependencyObject obj)
        {
            var retVal = false;
            try
            {
                var db = DatabaseUtils.GetDatabase();
                var competitors = (List<Competitor>)db.Competitors.All();
                retVal = competitors.TrueForAll(c => c.Chip != null);
                if (!retVal)
                    ShowCompetitorsIncosistencyError(obj);
            }
            catch (Exception e)
            {
                ShowException(obj, "Nie można sprawdzić spójności zawodników", e);
            }

            return retVal;
        }

        /// <summary>Shows successful save popup</summary> 
        /// <param name="obj">Represents control which called this method (used for getting window)</param>
        /// <returns>true when connected, false when user refuses to connect</returns>
        public static void ShowSuccessfulSave(DependencyObject obj)
        {
            MessageBox.Show(GetWindow(obj),
                            "Zapis do pliku wykonany pomyślnie",
                            "Zapisano dokument",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        private static bool GetBoolFromMessageBoxResult(MessageBoxResult mbr)
        {
            bool result = false;
            switch (mbr)
            {
                default:
                    break;
                case MessageBoxResult.OK:
                case MessageBoxResult.Yes:
                    result = true;
                    break;
            }
            return result;
        }

        private static Window GetWindow(DependencyObject obj)
        {
            return obj == null ? Application.Current.MainWindow : Window.GetWindow(obj);
        }
    }
}
