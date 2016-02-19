using GecoSI.Net;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Windows
{
    /// <summary>
    /// Interaction logic for ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        string portname;
        public ConnectionWindow()
        {
            InitializeComponent();
            probePorts();
        }

        private void probeButton_Click(object sender, RoutedEventArgs e)
        {
            probePorts();
        }

        private void probePorts()
        {
            probeButton.IsEnabled = false;
            connectButton.IsEnabled = false;
            portLBox.IsEnabled = false;
            portname = null;

            portLBox.Items.Clear();
            portLBox.Items.Add("Wyszukiwanie...");
            var portNames = SerialPort.GetPortNames();
            portLBox.Items.Clear();
            foreach (var port in portNames)
            {
                portLBox.Items.Add(port);
            }

            if(portNames.Length == 0)
            {
                portLBox.Items.Add("Nie znaleziono stacji");
                portLBox.Items.Add("Naciśnij \"Wyszukaj\" aby znaleźć stację");
            }
            else
            {
                portLBox.IsEnabled = true;
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
            connectButton.IsEnabled = true;
            portname = (string)portLBox.SelectedValue;
        }
    }
}
