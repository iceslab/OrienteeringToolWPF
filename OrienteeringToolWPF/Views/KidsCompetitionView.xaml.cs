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

        public KidsCompetitionView(MainWindow mw)
        {
            InitializeComponent();
            currentTimeL.DataContext = mw;
        }

        private void GetTournaments()
        {
            var dao = new TournamentDAO();
            var TournamentList = dao.findAll();
            for (int i = 1; i < TournamentList.Count; i++)
                dao.deleteById(TournamentList[i]);

            if (TournamentList.Count > 1)
                TournamentList = dao.findAll();
            tournament = TournamentList[0];

            tournamentG.DataContext = tournament;
        }

        private void GetCompetitors()
        {
            var dao = new CompetitorDAO();
            CompetitorsList = dao.findAll();

            competitorsLV.ItemsSource = CompetitorsList;
        }

        private void GetRelays()
        {
            var dao = new RelayDAO();
            RelaysList = dao.findAll();

            relaysLV.ItemsSource = RelaysList;
        }

        private void GetRoutes()
        {
            var dao = new RouteDAO();
            RoutesList = dao.findAll();

            routesLV.ItemsSource = RoutesList;
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
                    ManageButtons(competitorsLV);
                    GetCompetitors();
                    break;
                case TabIndexEnum.RELAYS:
                    addB.Visibility = Visibility.Visible;
                    deleteB.Visibility = Visibility.Visible;
                    ManageButtons(relaysLV);
                    GetRelays();
                    break;
                case TabIndexEnum.ROUTES:
                    addB.Visibility = Visibility.Visible;
                    deleteB.Visibility = Visibility.Visible;
                    ManageButtons(routesLV);
                    GetRoutes();
                    break;
                default:
                    break;
            }
        }

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
                    window = new CompetitorForm((Competitor)competitorsLV.SelectedItem);
                    break;
                case TabIndexEnum.RELAYS:
                    window = new RelayForm((Relay)relaysLV.SelectedItem);
                    break;
                case TabIndexEnum.ROUTES:
                    window = new RouteForm((Route)routesLV.SelectedItem);
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
            object dao = null;
            var index = (TabIndexEnum)tabControl.SelectedIndex;
            switch (index)
            {
                case TabIndexEnum.COMPETITORS:
                    if (ShowDeleteWarning() != MessageBoxResult.OK)
                        break;
                    dao = new CompetitorDAO();
                    foreach (Competitor c in competitorsLV.SelectedItems)
                        ((CompetitorDAO)dao).deleteById(c);
                    break;
                case TabIndexEnum.RELAYS:
                    if (ShowDeleteWarning() != MessageBoxResult.OK)
                        break;
                    dao = new RelayDAO();
                    foreach (Relay r in relaysLV.SelectedItems)
                        ((RelayDAO)dao).deleteById(r);
                    break;
                case TabIndexEnum.ROUTES:
                    if (ShowDeleteWarning() != MessageBoxResult.OK)
                        break;
                    dao = new RouteDAO();
                    foreach (Route r in routesLV.SelectedItems)
                        ((RouteDAO)dao).deleteById(r);
                    break;
                default:
                    return;
            }
            PrepareTab(index);
        }

        private MessageBoxResult ShowDeleteWarning()
        {
            return MessageBox.Show(
                Window.GetWindow(this),
                "Uwaga!\nUsunięte zostaną także wszystkie powiązane dane.",
                "Ostrzeżenie",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning);
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
    }
}
