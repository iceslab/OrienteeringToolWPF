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
    /// Interaction logic for CompetitorsListView.xaml
    /// </summary>
    public partial class CompetitorsListView : UserControl, IRefreshable, IButtonsManageable
    {
        public long? RelayId { get; set; }
        public List<Competitor> CompetitorsList { get; private set; }
        public bool RefreshEnabled { get; set; } = true;
        public ListView View
        {
            get { return competitorsLV; }
        }

        public CompetitorsListView() : this(null) { }

        public CompetitorsListView(long? RelayId)
        {
            this.RelayId = RelayId;
            InitializeComponent();
            ManageButtons(null);
            Refresh();
        }

        public void Refresh()
        {
            if (RefreshEnabled)
            {
                try
                {
                    var db = DatabaseUtils.GetDatabase();
                    if (RelayId != null)
                        CompetitorsList = db.Competitors.FindAllByRelayId(RelayId);
                    else
                        CompetitorsList = db.Competitors.All();
                    competitorsLV.ItemsSource = CompetitorsList;
                }
                catch (Exception e)
                {
                    MessageUtils.ShowException(this, "Nie można pobrać listy zawodników", e);
                }
            }
        }

        #region Buttons
        private void addB_Click(object sender, RoutedEventArgs e)
        {
            Window window = new CompetitorForm();
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
            Refresh();
        }

        private void editB_Click(object sender, RoutedEventArgs e)
        {
            Window window = new CompetitorForm((Competitor)competitorsLV.SelectedItem);
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
            Refresh();
        }

        private void deleteB_Click(object sender, RoutedEventArgs e)
        {
            if (MessageUtils.ShowDeleteWarning(this) == true)
            {
                var db = DatabaseUtils.GetDatabase();
                foreach (Competitor c in competitorsLV.SelectedItems)
                    db.Competitors.DeleteById(c.Id);
                Refresh();
            }
        }

        private void ManageButtons(ListView lv)
        {
            if (lv?.SelectedItem != null)
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
        #endregion
        #region IButtonsManageable implementation
        public void SetButtonsVisibility(Visibility all)
        {
            addB.Visibility = all;
            editB.Visibility = all;
            deleteB.Visibility = all;
        }

        public void SetButtonsVisibility(Visibility add, Visibility edit, Visibility delete)
        {
            addB.Visibility = add;
            editB.Visibility = edit;
            deleteB.Visibility = delete;
        }
        #endregion
        public void SetSource(List<Competitor> competitors)
        {
            CompetitorsList = competitors;
            competitorsLV.ItemsSource = CompetitorsList;
        }

        private void competitorsLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is ListView)
                ManageButtons((ListView)e.Source);
            else
                Console.WriteLine("Not ListView: " + e.Source);
            e.Handled = RefreshEnabled;
        }
    }
}
