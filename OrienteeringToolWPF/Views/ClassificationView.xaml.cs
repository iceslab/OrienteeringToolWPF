using OrienteeringToolWPF.Enumerations;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            relaysLV.RefreshEnabled = false;
            competitorsLV.RefreshEnabled = false;
            resultsAndPunchesLV.RefreshEnabled = false;

            relaysLV.View.SelectionChanged += RelaysLV_SelectionChanged;
            competitorsLV.View.SelectionChanged += CompetitorsLV_SelectionChanged;
            Refresh();
        }

        #region PropertyChanged handlers
        private void RelaysLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var r = (Relay)relaysLV.View.SelectedItem;
            competitorsLV.SetSource((List<Competitor>)r?.Competitors);

            var c = (Competitor)competitorsLV.View.SelectedItem;
            resultsAndPunchesLV.SetSource(c?.Result, (List<Punch>)c?.Punches);
        }

        private void CompetitorsLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var c = (Competitor)competitorsLV.View.SelectedItem;
            resultsAndPunchesLV.SetSource(c?.Result, (List<Punch>)c?.Punches);
        }

        #endregion
        public void Refresh()
        {
            RefreshData();
            ClassifyAll();
            RefreshSetSource();
        }

        // Gets current data from database
        private void RefreshData()
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

                foreach (var competitor in relay.Competitors)
                {
                    var punches = (List<Punch>)competitor.Punches;
                    try
                    {
                        punches?.Sort();
                        Punch.CalculateDeltaStart(ref punches, competitor.Result.StartTime);
                        Punch.CalculateDeltaPrevious(ref punches);
                    }
                    catch (ArgumentNullException) { }
                    competitor.Punches = punches;
                }
            }
        }

        private void RefreshSetSource()
        {
            relaysLV.SetSource(RelayList);

            var r = (Relay)relaysLV.View.SelectedItem;
            competitorsLV.SetSource((List<Competitor>)r?.Competitors);

            var c = (Competitor)competitorsLV.View.SelectedItem;
            resultsAndPunchesLV.SetSource(c?.Result, (List<Punch>)c?.Punches);
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
                    // Ignoring exception for now, later show warning that not all competitors has results
                    catch (ArgumentNullException) { };

                    competitor.Punches = punchesList;
                }
            }
        }

        #region Classification methods
        // Performs general classification
        private void ClassifyAll()
        {
            ClassifyCompetitors();
            ClassifyRelays();
        }

        // Classifies competitors
        private void ClassifyCompetitors()
        {
            CheckCorrectness();
            foreach (var relay in RelayList)
            {
                ((List<Competitor>)relay.Competitors).Sort(delegate (Competitor left, Competitor right)
                {
                    int retVal = 0;
                    // Can be changed to InvalidPunches?
                    var leftPunches = left.PresentPunches + left.CorrectPunches;
                    var rightPunches = right.PresentPunches + right.CorrectPunches;

                    // When right of competitor made more mistakes
                    if (leftPunches > rightPunches)
                    {
                        retVal = 1;
                    }
                    // When right of competitor made more mistakes
                    else if (leftPunches < rightPunches)
                    {
                        retVal = -1;
                    }
                    // When competitors made the same amount of mistakes (this means none too)
                    else
                    {
                        var leftTime = left.Result.RunningTime;
                        var rightTime = right.Result.RunningTime;
                        // When left one ran longer
                        if (leftTime > rightTime)
                            retVal = -1;
                        // When right one ran longer
                        else if (leftTime < rightTime)
                            retVal = 1;
                        // When both ran the same time
                        else
                            retVal = 0;
                    }

                    return retVal;
                });
            }
        }

        // Classifies relays
        private void ClassifyRelays()
        {
            RelayList.Sort(delegate (Relay left, Relay right)
            {
                var retVal = 0;

                if (left.OverallRunningTime < right.OverallRunningTime)
                    retVal = 1;
                else if (left.OverallRunningTime > right.OverallRunningTime)
                    retVal = -1;
                else
                    retVal = 0;

                return retVal;
            });
        }
        #endregion
    }
}
