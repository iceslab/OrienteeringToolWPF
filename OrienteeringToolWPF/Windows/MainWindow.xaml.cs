using GecoSI.Net;
using Microsoft.Win32;
using OrienteeringToolWPF.Views;
using OrienteeringToolWPF.Windows.Forms.KidsCompetition;
using System;
using System.ComponentModel;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
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
        private DateTime _currentTime;
        public DateTime CurrentTime
        {
            get { return _currentTime; }
            set
            {
                _currentTime = value;
                OnPropertyChanged("CurrentTime");
                OnPropertyChanged("FormattedCurrentTime");
            }
        }
        public string FormattedCurrentTime
        {
            get { return CurrentTime.ToString(FormatTimeString); }
        }
        public string FormatTimeString;

        private DispatcherTimer timer;
        public static string DatabasePath { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
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

            InitializeComponent();
            FormatTimeString = "HH:mm:ss.f";
            _currentTime = DateTime.Now;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();

            CurrentView = null;

            // Do bindowania
            menuBar.DataContext = Handler;
            label.DataContext = Listener;
            contentControl.DataContext = this;

            Listener.Notify(CommStatus.Off);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now;
        }

        // Rozłącz ze stacją
        private void Disconnect()
        {
            Handler?.Stop();
        }

        // Pobierz połączenie z bazą
        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection("Data Source=" + DatabasePath + ";Version=3;foreign keys=True");
        }

        // Utwórz projekt i bazę danych "Dziecinady"
        private void CreateKCDatabase()
        {
            //TODO: Sprawdzić czy nie nadpisujemy bazy
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

        // Zamykanie okna
        private void mainWindow_Closing(object sender, CancelEventArgs e)
        {
            Disconnect();
            timer.Stop();
        }

        // Połącz ze stacją - menu
        private void connectToMItem_Click(object sender, RoutedEventArgs e)
        {
            ConnectionWindow window = new ConnectionWindow();
            window.Owner = this;
            window.ShowDialog();
        }

        // Rozłącz ze stacją - menu
        private void disconnectMItem_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();
        }

        // Zamknij z menu
        private void exitMItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Otwórz projekt
        private void openMItem_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            //ofd.InitialDirectory = Directory.GetCurrentDirectory();
            ofd.InitialDirectory = @"C:\Users\Bartosz\Desktop\testowe_bazy";
            ofd.Filter = "Baza danych SQLite3 (.s3db)|*.s3db|Wszystkie pliki (*.*)|*.*";
            ofd.FilterIndex = 1;

            if (ofd.ShowDialog() == true)
            {
                DatabasePath = ofd.FileName;
                var kc = new KidsCompetitionView();
                kc.currentTimeL.DataContext = this;
                CurrentView = kc;
            }
        }

        // Utwórz projekt "Dziecinada"
        private void kidsCompetitionMItem_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            //sfd.InitialDirectory = Directory.GetCurrentDirectory();
            sfd.InitialDirectory = @"C:\Users\Bartosz\Desktop\testowe_bazy";
            sfd.Filter = "Baza danych SQLite3 (.s3db)|*.s3db|Wszystkie pliki (*.*)|*.*";
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
