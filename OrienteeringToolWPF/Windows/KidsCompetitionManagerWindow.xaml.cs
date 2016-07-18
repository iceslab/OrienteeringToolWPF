using OrienteeringToolWPF.CompetitionManagers;
using OrienteeringToolWPF.DAO.Implementation;
using OrienteeringToolWPF.Model;
using System;
using System.ComponentModel;
using System.Windows;

namespace OrienteeringToolWPF.Windows
{
    public partial class KidsCompetitionManagerWindow : CommonManager
    {
        public Tournament tournament { get; private set; }

        public KidsCompetitionManagerWindow(Tournament tournament) : base()
        {
            InitializeComponent();
            this.tournament = tournament;
        }

        // Starts or resumes competition
        public override void Start()
        {
            // Competition in progress - resume
            if (tournament.IsRunning == true)
            {
                Resume();
            }
            // Competition finished - show information
            else if (tournament.HasFinished == true)
            {
                MessageBox.Show(this,
                    "Nie można rozpocząć zakończonych zawodów",
                    "Zawody zostały zakończone",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            // Competition starting
            else
            {
                MessageBoxResult result = MessageBoxResult.Yes;

                // When current date is earlier than planned start date
                if (DateTime.Now < tournament.StartTime)
                {
                    // Prompt warning and allow override
                    result = MessageBox.Show(this,
                        "Czas rozpoczęcia jeszcze nie minął.\nRozpocząć mimo to?",
                        "Ostrzeżenie",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning,
                        MessageBoxResult.No);
                }

                // Actual start of tournament
                if (result == MessageBoxResult.Yes)
                {
                    // If not connected to station, ask for connection
                    if(PromptForConnection())
                    {
                        // Save actual start of tournament
                        tournament.StartedAtTime = DateTime.Now;
                        var db = MainWindow.GetDatabase();
                        db.Update(tournament);
                        //var dao = new TournamentDAO();
                        //dao.update(tournament);

                        // Show window and register listener
                        Show();
                        MainWindow.Listener.PropertyChanged += Listener_PropertyChanged;
                    }
                }
            }
        }

        // Finish competition
        public override void Finish()
        {
            // Save competition finish time
            tournament.FinishedAtTime = DateTime.Now;
            var db = MainWindow.GetDatabase();
            db.Tournament.Update(tournament);
            //var dao = new TournamentDAO();
            //dao.update(tournament);

            // Close window
            Close();
        }

        // Resume competition
        protected override void Resume()
        {
            // If not connected to station, ask for connection
            if (PromptForConnection())
            {
                // Show window and register listener
                Show();
                MainWindow.Listener.PropertyChanged += Listener_PropertyChanged;
            }
        }

        // Prompts user for connection to station if not connected
        // returns true when connected, false when user refuses to connect
        private bool PromptForConnection()
        {
            while (MainWindow.Handler.NotIsConnected)
            {
                var connectionW = new ConnectionWindow();
                connectionW.Owner = Owner;
                if (connectionW.ShowDialog() != true)
                {
                    var connectionResult = MessageBox.Show(Owner,
                        "Aby móc rozpocząć zawody należy połączyć się ze stacją." +
                        "\nSpróbować ponownie?",
                        "Nie połączono się ze stacją.",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information,
                        MessageBoxResult.Yes);

                    // User refuses to connect
                    if (connectionResult == MessageBoxResult.No)
                        return false;
                }
            }

            return true;
        }

        // Listener for incoming chip dataframes
        private void Listener_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataFrame")
            {

            }
        }
    }
}
