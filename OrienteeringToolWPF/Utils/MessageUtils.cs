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
        public static MessageBoxResult ShowDeleteWarning(DependencyObject obj)
        {
            return MessageBox.Show(
                Window.GetWindow(obj),
                "Uwaga!\nUsunięte zostaną także wszystkie powiązane dane.",
                "Ostrzeżenie",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning);
        }
    }
}
