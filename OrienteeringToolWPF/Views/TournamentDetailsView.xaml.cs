﻿using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using OrienteeringToolWPF.Windows;
using OrienteeringToolWPF.Windows.Forms.KidsCompetition;
using System;
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
            var db = MainWindow.GetDatabase();
            tournament = db.Tournament.All().FirstOrDefault();
            if(tournament.Id != null)
                db.Tournament.DeleteAll(db.Tournament.Id != tournament.Id);

            tournamentG.DataContext = tournament;

            // Assigning proper button name
            if (tournament.HasFinished)
                startTournamentB.Content = Properties.Resources.TournamentClassification;
            else if (tournament.HasStarted)
                startTournamentB.Content = Properties.Resources.TournamentContinue;
            else
                startTournamentB.Content = Properties.Resources.TournamentStart;
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
            if (tournament.HasFinished || MessageUtils.PromptForConnection(this) == true)
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
