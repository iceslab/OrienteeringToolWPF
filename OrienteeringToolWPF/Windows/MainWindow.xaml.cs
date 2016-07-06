using GecoSI.Net;
using Microsoft.Win32;
using OrienteeringToolWPF.Views;
using OrienteeringToolWPF.Windows.Forms.KidsCompetition;
using System;
using System.ComponentModel;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;

namespace OrienteeringToolWPF.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static SiHandler Handler { get; private set; }
        public static SiListener Listener { get; private set; }
        private UserControl _currentView;
        public UserControl CurrentView
        {
            get { return _currentView; }

            set
            {
                _currentView = value;
                OnPropertyChanged("CurrentView");
            }
        }
        
        public static string DatabasePath { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        static MainWindow()
        {
            Listener = new SiListener();
            Handler = new SiHandler(Listener);
        }

        public MainWindow()
        {
            var culture = new CultureInfo("pl");

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(
                        CultureInfo.CurrentCulture.IetfLanguageTag)));

#if DEBUG
            var asm = AppDomain.CurrentDomain.GetAssemblies();

            Console.WriteLine("=====================");
            foreach (var a in asm)
                Console.WriteLine(a.GetName().Name);
            Console.WriteLine("=====================");
#endif
            InitializeComponent();
            _currentView = null;

            // Do bindowania
            menuBar.DataContext = Handler;
            label.DataContext = Listener;
            contentControl.DataContext = this;

            Listener.Notify(CommStatus.Off);
        }

        // Disconnect from station
        private void Disconnect()
        {
            Handler?.Stop();
        }

        // Get connection to database
        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection("Data Source=" + DatabasePath + ";Version=3;foreign keys=True");
        }

        // Create project and database for "Kids Competition"
        private void CreateKCDatabase()
        {
            //TODO: Check if database is overwritten
            SQLiteConnection.CreateFile(DatabasePath);
            var connection = GetConnection();
            var command = connection.CreateCommand();

            command.CommandText = Properties.Resources.CreateKCDatabase;

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        // Closing window
        private void mainWindow_Closing(object sender, CancelEventArgs e)
        {
            Disconnect();
        }

        // Connect to station - menu
        private void connectToMItem_Click(object sender, RoutedEventArgs e)
        {
            ConnectionWindow window = new ConnectionWindow();
            window.Owner = this;
            window.ShowDialog();
        }

        // Disconnect from station - menu
        private void disconnectMItem_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();
        }

        // Cloe from menu
        private void exitMItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Open project
        private void openMItem_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            //ofd.InitialDirectory = Directory.GetCurrentDirectory();
            ofd.InitialDirectory = @"C:\Users\Bartosz\Desktop\testowe_bazy";
            ofd.Filter = Properties.Resources.DatabaseDialogFilters;
            ofd.FilterIndex = 1;

            if (ofd.ShowDialog() == true)
            {
                DatabasePath = ofd.FileName;
                CurrentView = new KidsCompetitionView();
            }
        }

        // Create project "Kids Competition"
        private void kidsCompetitionMItem_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            //sfd.InitialDirectory = Directory.GetCurrentDirectory();
            sfd.InitialDirectory = @"C:\Users\Bartosz\Desktop\testowe_bazy";
            sfd.Filter = Properties.Resources.DatabaseDialogFilters;
            sfd.FilterIndex = 1;

            if (sfd.ShowDialog() == true)
            {
                DatabasePath = sfd.FileName;
                CreateKCDatabase();

                var window = new TournamentForm();
                window.Owner = this;

                if (window.ShowDialog() == true)
                {
                    CurrentView = new KidsCompetitionView();
                }
                else
                {
                    File.Delete(DatabasePath);
                    DatabasePath = "";
                }
            }
        }
    }
}
