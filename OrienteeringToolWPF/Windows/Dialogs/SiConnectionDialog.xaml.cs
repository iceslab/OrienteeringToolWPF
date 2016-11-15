using GecoSI.Net;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for ConnectionWindow.xaml
    /// </summary>
    public partial class SiConnectionDialog : Window
    {
        string portname;
        public SiConnectionDialog()
        {
            InitializeComponent();
            ProbePorts();
        }

        private void probeButton_Click(object sender, RoutedEventArgs e)
        {
            ProbePorts();
        }

        private void ProbePorts()
        {
            probeButton.IsEnabled = false;
            connectButton.IsEnabled = false;
            portLBox.IsEnabled = false;
            portname = null;

            portLBox.Items.Clear();
            portLBox.Items.Add(Properties.Resources.Searching);
            var portNames = SerialPort.GetPortNames();
            portLBox.Items.Clear();
            foreach (var port in portNames)
            {
                portLBox.Items.Add(port);
            }

            if(portNames.Length == 0)
            {
                portLBox.Items.Add(Properties.Resources.CannotFindStation);
                portLBox.Items.Add(Properties.Resources.ConnectionWindowUsage);
            }
            else
            {
                portLBox.IsEnabled = true;
                portLBox.Focus();
                portLBox.SelectedIndex = 0;
            }

            probeButton.IsEnabled = true;
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Handler.Connect(portname);

            if(MainWindow.Handler.IsConnected)
            {
                DialogResult = true;
                Close();
            }
        }

        private void portLBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (portLBox.SelectedItem != null)
            {
                connectButton.IsEnabled = true;
                portname = (string)portLBox.SelectedValue;
            }
        }
    }
}
