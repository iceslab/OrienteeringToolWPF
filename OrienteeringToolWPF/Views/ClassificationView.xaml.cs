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
            resultsAndPunchesLV.RefreshEnabled = true;

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
            ClassificationUtils.ClassifyAll(RelayList);
            RefreshSetSource();
        }

        // Gets current data from database
        private void RefreshData()
        {
            RelayList = RelayHelper.RelaysWithCompetitorsJoined();
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

        private void refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Refresh();
        }
    }
}
