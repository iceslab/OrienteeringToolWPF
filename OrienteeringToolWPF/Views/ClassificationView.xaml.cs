using OrienteeringToolWPF.DAO;
using OrienteeringToolWPF.Enumerations;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
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

            var db = DatabaseUtils.GetDatabase();
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
            bestCompetitorsLV.RelaysList = RelayList;

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
                    var db = DatabaseUtils.GetDatabase();
                    var punchesList = (List<Punch>)competitor.Punches;
                    var routeStepsList = RouteStepsHelper.RouteStepsWhereChip((long)competitor.Chip);
                    try
                    {
                        Punch.CheckCorrectnessSorted(ref punchesList, routeStepsList);
                    }
                    // TODO: Ignoring exception for now, later show warning that not all competitors has results
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
                ((List<Competitor>)relay.Competitors).Sort();
            }
        }

        // Classifies relays
        private void ClassifyRelays()
        {
            RelayList.Sort();
        }
        #endregion
    }
}
