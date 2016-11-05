using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using OrienteeringToolWPF.Windows;
using OrienteeringToolWPF.Windows.Forms.KidsCompetition;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Views.Lists
{
    /// <summary>
    /// Interaction logic for BestCompetitorsListView.xaml
    /// </summary>
    public partial class BestCompetitorsListView : UserControl
    {
        private List<Relay> _RelaysList;
        public List<Relay> RelaysList
        {
            get { return _RelaysList; }
            set
            {
                _RelaysList = value;
                FindThreeBestCompetitors();
                competitorsLV.ItemsSource = CompetitorsList;
            }
        }

        public List<Competitor> CompetitorsList { get; private set; }

        public BestCompetitorsListView()
        {
            InitializeComponent();
        }

        private void FindThreeBestCompetitors()
        {
            CompetitorsList = new List<Competitor>();

            foreach (var relay in RelaysList)
            {
                CompetitorsList.AddRange(relay.Competitors);
            }

            CompetitorsList.Sort();

            if (CompetitorsList.Count > 3)
                CompetitorsList.RemoveRange(3, CompetitorsList.Count - 3);
        }
    }
}
