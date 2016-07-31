using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Views.Lists;
using OrienteeringToolWPF.Windows;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Views
{
    /// <summary>
    /// Interaction logic for RelaysAndCompetitorsTreeView.xaml
    /// </summary>
    public partial class RelaysAndCompetitorsTreeView : UserControl, Refreshable
    {
        public List<Relay> RelayList { get; private set; }
        public RelaysAndCompetitorsTreeView()
        {
            InitializeComponent();
            Refresh();
        }

        public void Refresh()
        {
            var db = MainWindow.GetDatabase();
            dynamic alias;
            RelayList = db.Relays
                            .All()
                            .LeftJoin(db.Competitors, out alias)
                            .On(db.Competitors.RelayId == db.Relays.Id)
                            .With(alias);
            relaysAndCompetitorsTV.ItemsSource = RelayList;
        }

        private void relaysAndCompetitorsTV_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ICurrentView window = null;
            try
            {
                window = (ICurrentView)Window.GetWindow(this);
                if (e.NewValue is Competitor)
                {
                    var c = (Competitor)e.NewValue;
                    var uc = new ResultsAndPunchesListView(c.Chip);
                    uc.SetButtonsVisibility(Visibility.Collapsed);
                    window.CurrentView = uc;
                }
                else if (e.NewValue is Relay)
                {
                    var r = (Relay)e.NewValue;
                    var uc = new CompetitorsListView(r.Id);
                    uc.SetButtonsVisibility(Visibility.Collapsed);
                    window.CurrentView = uc;
                }
            }
            catch(InvalidCastException){}

            Console.WriteLine(sender);
        }
    }
}
