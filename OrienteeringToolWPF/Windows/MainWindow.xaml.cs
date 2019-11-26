using GecoSI.Net;
using Microsoft.Win32;
using OrienteeringToolWPF.DAO;
using OrienteeringToolWPF.Enumerations;
using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Utils;
using OrienteeringToolWPF.Utils.Documents;
using OrienteeringToolWPF.Views;
using OrienteeringToolWPF.Windows.Dialogs;
using OrienteeringToolWPF.Windows.Forms.KidsCompetition;
using System;
using System.ComponentModel;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
#if !DEBUG
using System.IO;
#endif

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

            Listener.Notify(CommStatus.Off);
        }

        // Disconnect from station
        private void Disconnect()
        {
            Handler?.Stop();
        }

        ///<summary>
        ///Creates project and database for "Kids Competition"
        ///</summary>
        private void CreateKCDatabase()
        {
            // Create database file
            SQLiteConnection.CreateFile(DatabaseUtils.DatabasePath);
            var connection = DatabaseUtils.GetConnection();
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
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Disconnect();
        }

        #region Menu callback methods
        // Connect to station - menu
        private void ConnectToMItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new SiConnectionDialog
            {
                Owner = this
            };
            window.ShowDialog();
        }

        // Disconnect from station - menu
        private void DisconnectMItem_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();
        }

        // Close from menu
        private void ExitMItem_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            MessageUtils.ShowException(this,
                "Sample long and meaningless string whose only purpose is testing this simple dialog",
                new Exception(Properties.Resources.LoremIpsum));
#endif
            Close();
        }

        // Open local project
        private void OpenLocalMItem_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
#if DEBUG
                InitialDirectory = @"C:\Users\Bartosz\Desktop\testowe_bazy",
#else
                InitialDirectory = Directory.GetCurrentDirectory(),
#endif
                Filter = Properties.Resources.DatabaseDialogFilters,
                FilterIndex = 1
            };
            if (ofd.ShowDialog() == true)
            {
                DatabaseUtils.DatabasePath = ofd.FileName;
                DatabaseUtils.DatabaseType = DatabaseType.SQLITE3;
                CurrentView = new MainView();
            }
        }

        // Open remote project
        private void OpenRemoteMItem_Click(object sender, RoutedEventArgs e)
        {
            var rdw = new DatabseConnectionDialog(DatabaseUtils.DatabaseConnectionData)
            {
                Owner = this
            };
            if (rdw.ShowDialog() == true)
            {
                DatabaseUtils.DatabaseConnectionData = rdw.databaseConnectionData;
                DatabaseUtils.DatabaseType = DatabaseType.MYSQL;
                CurrentView = new MainView();
            }
        }

        // Closes connection to database
        private void CloseDatabaseMItem_Click(object sender, RoutedEventArgs e)
        {
            CurrentView = null;
            DatabaseUtils.DatabaseType = DatabaseType.NONE;
            DatabaseUtils.DatabaseConnectionData.Password = null;
        }

        // Create project "Kids Competition"
        private void KidsCompetitionMItem_Click(object sender, RoutedEventArgs e)
        {

            var sfd = new SaveFileDialog
            {
#if DEBUG
                InitialDirectory = @"C:\Users\Bartosz\Desktop\testowe_bazy",
#else
                InitialDirectory = Directory.GetCurrentDirectory(),
#endif
                Filter = Properties.Resources.DatabaseDialogFilters,
                FilterIndex = 1
            };

            if (sfd.ShowDialog() == true)
            {
                var window = new TournamentForm(true)
                {
                    Owner = this
                };

                if (window.ShowDialog() == true)
                {
                    DatabaseUtils.DatabasePath = sfd.FileName;
                    DatabaseUtils.DatabaseType = DatabaseType.SQLITE3;
                    CreateKCDatabase();
                    window.Save();
                    CurrentView = new MainView();
                }
            }
        }

        // Create starting list
        private void StartingListMItem_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog
            {
#if DEBUG
                InitialDirectory = @"C:\Users\Bartosz\Desktop\testowe_bazy",
#else
                InitialDirectory = Directory.GetCurrentDirectory(),
#endif
                // TODO: Change to resources
                Filter = "Dokument Word|*.docx",
                FilterIndex = 1
            };
#if !DEBUG
            if (sfd.ShowDialog() == true)
#else
            sfd.FileName = sfd.InitialDirectory + @"\test.docx";
#endif
            {
                try
                {
                    var relays = RelayHelper.RelaysWithCompetitors();
                    DocumentUtils.CreateStartingList(sfd.FileName, relays);
                    MessageUtils.ShowSuccessfulSave(this);
                }
                catch(Exception ex)
                {
                    MessageUtils.ShowException(this, "Nie można utworzyć listy startowej", ex);
                }
            }
        }

        private void ExportMItem_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog
            {
#if DEBUG
                InitialDirectory = @"C:\Users\Bartosz\Desktop\testowe_bazy",
#else
                InitialDirectory = Directory.GetCurrentDirectory(),
#endif
                // TODO: Change to resources
                Filter = "Dokument tekstowy|*.txt",
                FilterIndex = 1
            };
#if !DEBUG
            if (sfd.ShowDialog() == true)
#else
            sfd.FileName = sfd.InitialDirectory + @"\test.txt";
#endif
            {
                try
                {
                    var relays = RelayHelper.RelaysWithCompetitors();
                    var categories = CategoryHelper.Categories();
                    DocumentUtils.ExportCompetitors(sfd.FileName, relays, categories);
                    MessageUtils.ShowSuccessfulSave(this);
                }
                catch (Exception ex)
                {
                    MessageUtils.ShowException(this, "Nie można wyeksportować listy zawodników", ex);
                }
            }
        }

        private void GeneralClassificationMItem_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
#if DEBUG
                InitialDirectory = @"C:\Users\Bartosz\Desktop\testowe_bazy",
#else
                InitialDirectory = Directory.GetCurrentDirectory(),
#endif
                // TODO: Change to resources
                Filter = "Dokument tekstowy|*.txt",
                FilterIndex = 1
            };
#if !DEBUG
            if (ofd.ShowDialog() == true)
#else
            ofd.FileName = ofd.InitialDirectory + @"\gen.TXT";
#endif
            {
                try
                {
                    GeneralClassificationUtils.Classify(ofd.FileName, ofd.FileName + "classified.txt");
                }
                catch (Exception ex)
                {
                    MessageUtils.ShowException(this, "Nie można utworzyć listy startowej", ex);
                }
            }
        }

        #endregion

        private void GenerateReportMItem_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog
            {
#if DEBUG
                InitialDirectory = @"C:\Users\Bartosz\Desktop\testowe_bazy",
#else
                InitialDirectory = Directory.GetCurrentDirectory(),
#endif
                // TODO: Change to resources
                Filter = "Dokument Excel|*.xlsx",
                FilterIndex = 1
            };
#if !DEBUG
            if (sfd.ShowDialog() == true)
#else
            sfd.FileName = sfd.InitialDirectory + @"\test.xlsx";
#endif
            {
                try
                {
                    var relays = RelayHelper.RelaysWithCompetitorsJoined();
                    DocumentUtils.CreateReport(sfd.FileName, relays);
                    MessageUtils.ShowSuccessfulSave(this);
                }
                catch (Exception ex)
                {
                    MessageUtils.ShowException(this, "Nie można utworzyć raportu końcowego", ex);
                }
            }
        }
    }
}
