using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Windows;
using OrienteeringToolWPF.Windows.Forms.KidsCompetition;
using System.Windows;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Views
{
    /// <summary>
    /// Interaction logic for TournamentDetailsView.xaml
    /// </summary>
    public partial class TournamentDetailsView : UserControl, Refreshable
    {
        public Tournament tournament { get; private set; }

        public TournamentDetailsView()
        {
            InitializeComponent();
        }

        public void Refresh()
        {
            var db = MainWindow.GetDatabase();
            tournament = db.Tournament.Get(1);
            db.Tournament.DeleteAll(db.Tournament.Id != tournament.Id);

            tournamentG.DataContext = tournament;
        }

        private void editB_Click(object sender, RoutedEventArgs e)
        {
            Window window = new TournamentForm(tournament);
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
            Refresh();
        }

        private void startTournamentB_Click(object sender, RoutedEventArgs e)
        {
            var kcWindow = new KidsCompetitionManagerWindow(tournament);
            kcWindow.Owner = Window.GetWindow(this);
            kcWindow.Start();
        }
    }
}
