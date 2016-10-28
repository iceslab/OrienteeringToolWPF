using GecoSI.Net;
using Microsoft.Win32;
using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Views;
using OrienteeringToolWPF.Windows.Forms.KidsCompetition;
using Simple.Data;
using System;
using System.ComponentModel;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Windows
{
    public enum DatabaseTypeEnum
    {
        NONE, MYSQL, SQLITE3
    }

    public partial class MainWindow : Window, INotifyPropertyChanged, ICurrentView
    {
        #region MainWindow fields
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
        private static DatabaseTypeEnum DatabaseType = DatabaseTypeEnum.NONE;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        static MainWindow()
        {
            Listener = new SiListener();
            Handler = new SiHandler(Listener);
        }

        public MainWindow()
        {
            InitializeComponent();
            _currentView = null;

            // Needed for bindings
            menuBar.DataContext = Handler;
            label.DataContext = Listener;
            //mainWindowCC.DataContext = this;

            Listener.Notify(CommStatus.Off);
        }

        // Disconnect from station
        private void Disconnect()
        {
            Handler?.Stop();
        }

        #region Database static methods
        // Get connection to database
        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection("Data Source=" + DatabasePath + ";Version=3;foreign keys=True");
        }

        // Get dynamic database object for SQLite3
        private static dynamic GetDatabaseSQLite3()
        {
            return Database.OpenConnection("Data Source=" + DatabasePath + ";Version=3;foreign keys=True");
        }

        private static dynamic GetDatabaseMysql()
        {
            throw new NotImplementedException("Mysql database connection is not implemented yet");
        }

        // Get dynamic database object
        public static dynamic GetDatabase()
        {
            switch (DatabaseType)
            {
                case DatabaseTypeEnum.SQLITE3:
                    return GetDatabaseSQLite3();
                case DatabaseTypeEnum.MYSQL:
                    return GetDatabaseMysql();
                case DatabaseTypeEnum.NONE:
                default:
                    throw new InvalidEnumArgumentException(
                        "Variable "
                        + "\"" + nameof(DatabaseType) + "\""
                        + " does not provide valid database type. Provided type: " 
                        + DatabaseType.ToString());
            }
        }
        #endregion

        // Create project and database for "Kids Competition"
        private void CreateKCDatabase()
        {
            // Create database file
            SQLiteConnection.CreateFile(DatabasePath);
            var connection = GetConnection();
            var command = connection.CreateCommand();

            // Prepare creation command
            command.CommandText = Properties.Resources.CreateKCDatabase;

            // Execute command
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

        #region Menu callback methods
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

        // Close from menu
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
                DatabaseType = DatabaseTypeEnum.SQLITE3;
                CurrentView = new MainView();
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
                var window = new TournamentForm(true);
                window.Owner = this;

                if (window.ShowDialog() == true)
                {
                    DatabasePath = sfd.FileName;
                    DatabaseType = DatabaseTypeEnum.SQLITE3;
                    CreateKCDatabase();
                    window.Save();
                    CurrentView = new MainView();
                }
            }
        }
        #endregion
    }
}
