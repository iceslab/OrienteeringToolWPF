using OrienteeringToolWPF.CompetitionManagers;
using OrienteeringToolWPF.DAO.Implementation;
using OrienteeringToolWPF.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OrienteeringToolWPF.Windows
{
    /// <summary>
    /// Interaction logic for KidsCompetitionManagerWindow.xaml
    /// </summary>
    public partial class KidsCompetitionManagerWindow : CommonManager
    {
        private Tournament tournament;
        public KidsCompetitionManagerWindow(Tournament tournament) : base()
        {
            InitializeComponent();
            this.tournament = tournament;
            if (this.tournament.StartedAtTime != null)
                _hasStarted = true;
            if (this.tournament.FinishedAtTime != null)
                _hasFinished = true;
        }

        public override void Start()
        {
            if (HasStarted == true)
            {
                Resume();
            }
            else
            {
                if (HasFinished == true)
                {
                    MessageBox.Show(this,
                        "Nie można rozpocząć zakończonych zawodów",
                        "Zawody zostały zakończone",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

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

                // Rozpoczęcie zawodów
                if (result == MessageBoxResult.Yes)
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
                                return;
                        }
                    }

                    tournament.StartedAtTime = DateTime.Now;
                    var dao = new TournamentDAO();
                    dao.update(tournament);

                    Show();

                    HasStarted = true;
                    IsRunning = true;
                    MainWindow.Listener.PropertyChanged += Listener_PropertyChanged;
                }
            }
        }

        public override void Finish()
        {
            tournament.FinishedAtTime = DateTime.Now;
            var dao = new TournamentDAO();
            dao.update(tournament);
            IsRunning = false;
            HasFinished = true;
            Close();
        }

        protected override void Resume()
        {
            Show();
            IsRunning = true;
            MainWindow.Listener.PropertyChanged += Listener_PropertyChanged;
        }

        private void Listener_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataFrame")
            {

            }
        }
    }
}
