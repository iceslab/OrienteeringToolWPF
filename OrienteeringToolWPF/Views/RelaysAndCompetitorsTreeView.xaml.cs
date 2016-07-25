using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Windows;
using System.Collections.Generic;
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
    }
}
