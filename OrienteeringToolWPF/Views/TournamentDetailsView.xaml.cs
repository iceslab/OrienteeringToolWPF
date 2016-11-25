using OrienteeringToolWPF.DAO;
using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using OrienteeringToolWPF.Windows;
using OrienteeringToolWPF.Windows.Forms.KidsCompetition;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Views
{
    /// <summary>
    /// Interaction logic for TournamentDetailsView.xaml
    /// </summary>
    public partial class TournamentDetailsView : UserControl, IRefreshable
    {
        public Tournament tournament { get; private set; }

        public TournamentDetailsView()
        {
            InitializeComponent();
        }

        public void Refresh()
        {
            try
            {
                tournament = TournamentHelper.Tournament();
                tournamentG.DataContext = tournament;

                // Assigning proper button name
                if (tournament.HasFinished)
                    startTournamentB.Content = Properties.Resources.TournamentClassification;
                else if (tournament.HasStarted)
                    startTournamentB.Content = Properties.Resources.TournamentContinue;
                else
                    startTournamentB.Content = Properties.Resources.TournamentStart;
            }
            catch (Exception e)
            {
                MessageUtils.ShowException(null, "Nie można pobrać obiektu zawodów", e);
            }
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
            if (tournament.HasFinished ||
                (MessageUtils.CheckCompetitorsCosistency(this) && MessageUtils.PromptForConnection(this)))
            {
                var kcWindow = new ManagerWindow(tournament);
                kcWindow.Closed += KcWindow_Closed;
                kcWindow.Owner = Window.GetWindow(this);
                kcWindow.Start();
            }
        }

        private void KcWindow_Closed(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}
