using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace OrienteeringToolWPF.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for StartListDialog.xaml
    /// </summary>
    public partial class StartListDialog : Window
    {
        public StartListDialog()
        {
            InitializeComponent();
        }

        private void organizerB_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
#if DEBUG
            ofd.InitialDirectory = @"C:\Users\Bartosz\Desktop\testowe_bazy";
#else
            ofd.InitialDirectory = Directory.GetCurrentDirectory();
#endif
            ofd.Filter = Properties.Resources.ImageDialogFilter;
            ofd.FilterIndex = 1;
            if (ofd.ShowDialog() == true)
            {
                organizerL.Content = ofd.FileName;
            }
        }

        private void tournamentB_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
#if DEBUG
            ofd.InitialDirectory = @"C:\Users\Bartosz\Desktop\testowe_bazy";
#else
            ofd.InitialDirectory = Directory.GetCurrentDirectory();
#endif
            ofd.Filter = Properties.Resources.ImageDialogFilter;
            ofd.FilterIndex = 1;
            if (ofd.ShowDialog() == true)
            {
                tournamentL.Content = ofd.FileName;
            }
        }

        private void acceptB_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
