using System;
using System.Drawing;
using System.Media;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OrienteeringToolWPF.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for ExceptionMessageBox.xaml
    /// </summary>
    public partial class ExceptionMessageBox : Window
    {

        private ExceptionMessageBox(Window owner, string messageBoxText, string title, Exception ex)
        {
            InitializeComponent();
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                SystemIcons.Error.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            errorIcon.Source = imageSource;
            Owner = owner;
            descriptionTB.Text = messageBoxText;
            Title = title;
            exceptionTB.Text = ex?.ToString();
        }

        public static void Show(Window owner, string messageBoxText, string title, Exception ex)
        {
            var emb = new ExceptionMessageBox(owner, messageBoxText, title, ex);
            SystemSounds.Exclamation.Play();
            emb.Activate();
            emb.ShowDialog();
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
