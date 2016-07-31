using GecoSI.Net;
using OrienteeringToolWPF.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
