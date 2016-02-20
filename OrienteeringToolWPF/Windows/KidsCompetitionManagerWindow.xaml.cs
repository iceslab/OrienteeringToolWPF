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

        public override void Start()
        {
            if (tournament.IsRunning == true)
            {
                Resume();
            }
            else if (tournament.HasFinished == true)
            {
                MessageBox.Show(this,
                    "Nie można rozpocząć zakończonych zawodów",
                    "Zawody zostały zakończone",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBoxResult result = MessageBoxResult.Yes;
                if (DateTime.Now < tournament.StartTime)
                {
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
                        tournament.StartedAtTime = DateTime.Now;
                        var dao = new TournamentDAO();
                        dao.update(tournament);

                        Show();
                        MainWindow.Listener.PropertyChanged += Listener_PropertyChanged;
                    }
                }
            }
        }

        public override void Finish()
        {
            tournament.FinishedAtTime = DateTime.Now;
            var dao = new TournamentDAO();
            dao.update(tournament);
            Close();
        }

        protected override void Resume()
        {
            // If not connected to station, ask for connection
            if (PromptForConnection())
            {
                Show();
                MainWindow.Listener.PropertyChanged += Listener_PropertyChanged;
            }
        }

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

                    // Użytkownik odmawia połączenia
                    if (connectionResult == MessageBoxResult.No)
                        return false;
                }
            }

            return true;
        }

        private void Listener_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataFrame")
            {

            }
        }
    }
}
