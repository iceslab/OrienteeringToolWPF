using GecoSI.Net;
using OrienteeringToolWPF.Windows;
using OrienteeringToolWPF.Windows.Dialogs;
using OrienteeringToolWPF.Windows.Forms;
using System;
using System.Windows;

namespace OrienteeringToolWPF.Utils
{
    static class MessageUtils
    {
        public static void ShowValidatorErrors(DependencyObject obj, ErrorList errors)
        {
            MessageBox.Show(Window.GetWindow(obj),
                            Properties.Resources.Errors + "\n" + errors.ToString(),
                            Properties.Resources.InvalidDataTitle,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
        }

        public static void ShowExtendedValidatorErrors(DependencyObject obj, ErrorList errors, string extendedError)
        {
            // TODO: Change to collapsable textbox
            MessageBox.Show(Window.GetWindow(obj),
                            Properties.Resources.Errors + "\n" + errors.ToString() +
                            "\n\n" + extendedError,
                            Properties.Resources.InvalidDataTitle,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
        }

        public static void ShowSiHandlerError(CommStatus errorStatus, string errorMessage)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Window owner = Application.Current.MainWindow;

                MessageBox.Show(owner,
                                Properties.Resources.OriginalMessage + "\n" + errorMessage,
                                Properties.Resources.SiHandlerError,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }));

        }

        public static bool ShowDeleteWarning(DependencyObject obj)
        {
            var mbr = MessageBox.Show(Window.GetWindow(obj),
                                    Properties.Resources.DatabaseCascadeDeleteWarning,
                                    Properties.Resources.Warning,
                                    MessageBoxButton.OKCancel,
                                    MessageBoxImage.Warning);
            return GetBoolFromMessageBoxResult(mbr);
        }

        public static bool ShowOverwriteWarning(DependencyObject obj)
        {
            var mbr = MessageBox.Show(Window.GetWindow(obj),
                                    Properties.Resources.DatabaseOverwriteWarning,
                                    Properties.Resources.Warning,
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Warning);
            return GetBoolFromMessageBoxResult(mbr);
        }

        public static bool ShowConnectionNeeddedInfo(DependencyObject obj)
        {
            var mbr = MessageBox.Show(Window.GetWindow(obj),
                        Properties.Resources.ConnectionNeeddedInfo,
                        Properties.Resources.ConnectionNotConnected,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information,
                        MessageBoxResult.Yes);
            return GetBoolFromMessageBoxResult(mbr);
        }

        public static void ShowCannotStartFinishedInfo(DependencyObject obj)
        {
            MessageBox.Show(Window.GetWindow(obj),
                    Properties.Resources.CompetitionCannotStart,
                    Properties.Resources.CompetitionHasFinished,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
        }

        // Prompts user for connection to station if not connected
        // returns true when connected, false when user refuses to connect
        public static bool PromptForConnection(DependencyObject obj)
        {
            while (MainWindow.Handler.NotIsConnected)
            {
                var siConnectionDialog = new SiConnectionDialog();
                siConnectionDialog.Owner = Window.GetWindow(obj);
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
            var mbr = MessageBox.Show(Window.GetWindow(obj),
                        Properties.Resources.StartTimeNotPassedWarning,
                        Properties.Resources.Warning,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning,
                        MessageBoxResult.No);
            return GetBoolFromMessageBoxResult(mbr);
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
    }
}
