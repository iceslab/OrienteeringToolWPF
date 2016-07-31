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
    public partial class RelaysAndCompetitorsTreeView : UserControl, IRefreshable
    {
        public List<Relay> RelayList { get; private set; }
        public RelaysAndCompetitorsTreeView()
        {
            InitializeComponent();
            Refresh();
            foreach(var item in relaysAndCompetitorsTV.Items)
            {
                Console.WriteLine(item);
            }
            RelayList[0].IsExpanded = true;
            RelayList[0].Competitors[0].IsSelected = true;
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
            SetNewView(e.NewValue);
            Console.WriteLine(sender);
        }

        private void SetNewView(object NewValue)
        {
            ICurrentView window = null;
            try
            {
                window = (ICurrentView)Window.GetWindow(this);
                if (NewValue is Competitor)
                {
                    var c = (Competitor)NewValue;
                    var uc = new ResultsAndPunchesListView(c.Chip);
                    uc.SetButtonsVisibility(Visibility.Collapsed);
                    window.CurrentView = uc;
                }
                else if (NewValue is Relay)
                {
                    var r = (Relay)NewValue;
                    var uc = new CompetitorsListView(r.Id);
                    uc.SetButtonsVisibility(Visibility.Collapsed);
                    window.CurrentView = uc;
                }
            }
            catch (InvalidCastException) { }
        }
    }
}
