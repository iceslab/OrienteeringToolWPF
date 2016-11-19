using GecoSI.Net;
using Microsoft.Win32;
using OrienteeringToolWPF.Enumerations;
using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Utils;
using OrienteeringToolWPF.Views;
using OrienteeringToolWPF.Windows.Dialogs;
using OrienteeringToolWPF.Windows.Forms.KidsCompetition;
using Simple.Data;
using System;
using System.ComponentModel;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Windows
{
    public partial class MainWindow : Window, INotifyPropertyChanged, ICurrentView
    {
        #region MainWindow fields
        public static SiHandler Handler { get; private set; }
        public static SiListener Listener { get; private set; }
        #region ICurrentView implementation
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
        #endregion

        private static string DatabasePath;
        private static DatabaseConnectionData databaseConnectionData;
        private static DatabaseType DatabaseType = DatabaseType.NONE;

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
            databaseConnectionData = new DatabaseConnectionData();
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
        private static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(
                ConnectionStringUtils.GetSqliteConnectionString(DatabasePath));
        }

        // Get dynamic database object for SQLite3
        private static dynamic GetDatabaseSQLite3()
        {
            //return Database.OpenConnection(
            //    ConnectionStringUtils.GetSqliteConnectionString(DatabasePath));
            return Database.Opener.OpenConnection(
                ConnectionStringUtils.GetSqliteConnectionString(DatabasePath),
                Properties.Resources.ProviderNameSqlite);
        }

        private static dynamic GetDatabaseMysql()
        {
            return Database.Opener.OpenConnection(
                ConnectionStringUtils.GetMySqlConnectionString(databaseConnectionData),
                Properties.Resources.ProviderNameMysql);
        }

        // Get dynamic database object
        public static dynamic GetDatabase()
        {
            switch (DatabaseType)
            {
                case DatabaseType.SQLITE3:
                    return GetDatabaseSQLite3();
                case DatabaseType.MYSQL:
                    return GetDatabaseMysql();
                case DatabaseType.NONE:
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
            var window = new SiConnectionDialog();
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

        // Open local project
        private void openLocalMItem_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
#if DEBUG
            ofd.InitialDirectory = @"C:\Users\Bartosz\Desktop\testowe_bazy";
#else
            ofd.InitialDirectory = Directory.GetCurrentDirectory();            
#endif
            ofd.Filter = Properties.Resources.DatabaseDialogFilters;
            ofd.FilterIndex = 1;
            if (ofd.ShowDialog() == true)
            {
                DatabasePath = ofd.FileName;
                DatabaseType = DatabaseType.SQLITE3;
                CurrentView = new MainView();
            }
        }

        // Open remote project
        private void openRemoteMItem_Click(object sender, RoutedEventArgs e)
        {
            var rdw = new DatabseConnectionDialog(databaseConnectionData);
            rdw.Owner = this;
            if (rdw.ShowDialog() == true)
            {
                databaseConnectionData = rdw.databaseConnectionData;
                DatabaseType = DatabaseType.MYSQL;
                CurrentView = new MainView();
            }
        }

        // Create project "Kids Competition"
        private void kidsCompetitionMItem_Click(object sender, RoutedEventArgs e)
        {

            var sfd = new SaveFileDialog();
#if DEBUG
            sfd.InitialDirectory = @"C:\Users\Bartosz\Desktop\testowe_bazy";
#else
            sfd.InitialDirectory = Directory.GetCurrentDirectory();            
#endif
            sfd.Filter = Properties.Resources.DatabaseDialogFilters;
            sfd.FilterIndex = 1;

            if (sfd.ShowDialog() == true)
            {
                var window = new TournamentForm(true);
                window.Owner = this;

                if (window.ShowDialog() == true)
                {
                    DatabasePath = sfd.FileName;
                    DatabaseType = DatabaseType.SQLITE3;
                    CreateKCDatabase();
                    window.Save();
                    CurrentView = new MainView();
                }
            }
        }
        #endregion
    }
}
