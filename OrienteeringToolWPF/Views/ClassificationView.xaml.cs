using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Windows;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Views
{
    /// <summary>
    /// Interaction logic for ClassificationView.xaml
    /// </summary>
    public partial class ClassificationView : UserControl
    {
        public List<Relay> RelayList { get; set; }
        public ClassificationView()
        {
            InitializeComponent();
            Refresh();
        }

        public void Refresh()
        {
            // NOTE: In this version of Simple.Data there is no populating of fields in nested objects 
            // (so called grandchild's fields). You need to populate them manually

            var db = MainWindow.GetDatabase();
            RelayList = (List<Relay>)db.Relays.All();
            dynamic resultsAlias, punchAlias;
            foreach (var relay in RelayList)
            {
                relay.Competitors =
                    (List<Competitor>)db.Competitors.FindAllByRelayId(relay.Id)
                        .LeftJoin(db.Results, out resultsAlias)
                        .On(db.Results.Chip == db.Competitors.Chip)
                        .LeftJoin(db.Punches, out punchAlias)
                        .On(db.Punches.Chip == db.Competitors.Chip)
                        .With(resultsAlias)
                        .With(punchAlias);
            }

            CheckCorrectness();
        }

        // Performs corectness check on all competitors
        private void CheckCorrectness()
        {
            foreach (var relay in RelayList)
            {
                foreach (var competitor in relay.Competitors)
                {
                    var db = MainWindow.GetDatabase();
                    dynamic routesAlias, competitorAlias;
                    var punchesList = (List<Punch>)competitor.Punches;
                    var routeStepsList = (List<RouteStep>)db.RouteSteps
                                    .All()
                                    .Join(db.Routes, out routesAlias)
                                    .On(db.RouteSteps.RouteId == db.Routes.Id)
                                    .Join(db.Competitors, out competitorAlias)
                                    .On(db.Routes.Category == db.Competitors.Category)
                                    .With(routesAlias)
                                    .With(competitorAlias)
                                    .Where(db.Competitors.Chip == competitor.Chip)
                                    .OrderBy(db.RouteSteps.Order);
                    try
                    {
                        Punch.CheckCorrectnessOrdered(ref punchesList, routeStepsList);
                    }
                    catch (ArgumentNullException) { };

                    competitor.Punches = punchesList;
                }
            }
        }

    }
}
