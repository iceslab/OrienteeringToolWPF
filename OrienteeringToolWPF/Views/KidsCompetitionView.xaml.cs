using OrienteeringToolWPF.DAO.Implementation;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Windows;
using OrienteeringToolWPF.Windows.Forms.KidsCompetition;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Views
{
    /// <summary>
    /// Interaction logic for KidsCompetition.xaml
    /// </summary>

    public partial class KidsCompetitionView : UserControl
    {
        #region KidsCompetitionView fields
        enum TabIndexEnum
        {
            TOURNAMENT = 0,
            COMPETITORS = 1,
            RELAYS = 2,
            ROUTES = 3
        }

        public Tournament tournament { get; private set; }
        public List<Competitor> CompetitorsList { get; private set; }
        public List<Relay> RelaysList { get; private set; }
        public List<Route> RoutesList { get; private set; }
        #endregion

        public KidsCompetitionView()
        {
            InitializeComponent();
        }

        #region Tab and data handling methods
        private void GetTournaments()
        {
            var db = MainWindow.GetDatabase();
            tournament = db.Tournament.Get(1);
            db.Tournament.DeleteAll(db.Tournament.Id != tournament.Id);

            tournamentView.tournamentG.DataContext = tournament;
        }

        private void GetCompetitors()
        {
            var db = MainWindow.GetDatabase();
            CompetitorsList = db.Competitors.All();
            competitorsView.competitorsLV.ItemsSource = CompetitorsList;
        }

        private void GetRelays()
        {
            var db = MainWindow.GetDatabase();
            RelaysList = db.Relays.All();
            relaysView.relaysLV.ItemsSource = RelaysList;
        }

        private void GetRoutes()
        {
            var db = MainWindow.GetDatabase();
            RoutesList = db.Routes.All();
            routesView.routesLV.ItemsSource = RoutesList;
        }

        private void PrepareTab(TabIndexEnum index)
        {
            switch (index)
            {
                case TabIndexEnum.TOURNAMENT:
                    addB.Visibility = Visibility.Collapsed;
                    deleteB.Visibility = Visibility.Collapsed;
                    editB.IsEnabled = true;
                    GetTournaments();
                    break;
                case TabIndexEnum.COMPETITORS:
                    addB.Visibility = Visibility.Visible;
                    deleteB.Visibility = Visibility.Visible;
                    ManageButtons(competitorsView.competitorsLV);
                    GetCompetitors();
                    break;
                case TabIndexEnum.RELAYS:
                    addB.Visibility = Visibility.Visible;
                    deleteB.Visibility = Visibility.Visible;
                    ManageButtons(relaysView.relaysLV);
                    GetRelays();
                    break;
                case TabIndexEnum.ROUTES:
                    addB.Visibility = Visibility.Visible;
                    deleteB.Visibility = Visibility.Visible;
                    ManageButtons(routesView.routesLV);
                    GetRoutes();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Button callback methods
        private void addB_Click(object sender, RoutedEventArgs e)
        {
            Window window = null;
            var index = (TabIndexEnum)tabControl.SelectedIndex;
            switch (index)
            {
                case TabIndexEnum.COMPETITORS:
                    window = new CompetitorForm();
                    break;
                case TabIndexEnum.RELAYS:
                    window = new RelayForm();
                    break;
                case TabIndexEnum.ROUTES:
                    window = new RouteForm();
                    break;
                default:
                    return;
            }

            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
            PrepareTab(index);
        }

        private void editB_Click(object sender, RoutedEventArgs e)
        {
            Window window = null;
            var index = (TabIndexEnum)tabControl.SelectedIndex;
            switch (index)
            {
                case TabIndexEnum.TOURNAMENT:
                    window = new TournamentForm(tournament);
                    break;
                case TabIndexEnum.COMPETITORS:
                    window = new CompetitorForm((Competitor)competitorsView.competitorsLV.SelectedItem);
                    break;
                case TabIndexEnum.RELAYS:
                    window = new RelayForm((Relay)relaysView.relaysLV.SelectedItem);
                    break;
                case TabIndexEnum.ROUTES:
                    window = new RouteForm((Route)routesView.routesLV.SelectedItem);
                    break;
                default:
                    return;
            }

            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
            PrepareTab(index);
        }

        private void deleteB_Click(object sender, RoutedEventArgs e)
        {
            if (ShowDeleteWarning() == MessageBoxResult.OK)
            {
                var db = MainWindow.GetDatabase();
                var index = (TabIndexEnum)tabControl.SelectedIndex;
                switch (index)
                {
                    case TabIndexEnum.COMPETITORS:
                        foreach (Competitor c in competitorsView.competitorsLV.SelectedItems)
                            db.Competitors.DeleteById(c.Id);
                        break;
                    case TabIndexEnum.RELAYS:
                        foreach (Relay r in relaysView.relaysLV.SelectedItems)
                            db.Relays.DeleteById(r.Id);
                        break;
                    case TabIndexEnum.ROUTES:
                        foreach (Route r in routesView.routesLV.SelectedItems)
                            db.Routes.DeleteById(r.Id);
                        break;
                    default:
                        return;
                }
                PrepareTab(index);
            }
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ManageButtons(e.Source);
        }

        private void ManageButtons(object source)
        {
            if (source is TabControl)
                PrepareTab((TabIndexEnum)tabControl.SelectedIndex);
            else if (source is ListView)
            {
                var lv = (ListView)source;
                if (lv.SelectedItem != null)
                {
                    editB.IsEnabled = true;
                    deleteB.IsEnabled = true;
                }
                else
                {
                    editB.IsEnabled = false;
                    deleteB.IsEnabled = false;
                }
            }
            else
                editB.IsEnabled = true;
        }

        private void startTournamentB_Click(object sender, RoutedEventArgs e)
        {
            var kcWindow = new KidsCompetitionManagerWindow(tournament);
            kcWindow.Owner = Window.GetWindow(this);
            kcWindow.Start();
        }
#endregion
        private MessageBoxResult ShowDeleteWarning()
        {
            return MessageBox.Show(
                Window.GetWindow(this),
                "Uwaga!\nUsunięte zostaną także wszystkie powiązane dane.",
                "Ostrzeżenie",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning);
        }

    }
}
