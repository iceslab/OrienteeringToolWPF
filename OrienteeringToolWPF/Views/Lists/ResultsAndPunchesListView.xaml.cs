using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using OrienteeringToolWPF.Windows;
using OrienteeringToolWPF.Windows.Forms.KidsCompetition;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OrienteeringToolWPF.Views.Lists
{
    /// <summary>
    /// Interaction logic for ResultsAndPunchesListView.xaml
    /// </summary>
    public partial class ResultsAndPunchesListView : UserControl, IRefreshable, IButtonsManageable
    {
        private long? Chip;
        private Result Result;
        private List<Punch> PunchesList;
        private List<RouteStep> RouteStepList;

        public ResultsAndPunchesListView() : this(null) { }

        public ResultsAndPunchesListView(long? Chip)
        {
            this.Chip = Chip;
            InitializeComponent();
            ManageButtons(null);
            Refresh();
        }

        public void Refresh()
        {
            var db = MainWindow.GetDatabase();
            if (Chip != null)
            {
                Result = db.Results.FindAllByChip(Chip).FirstOrDefault() ?? new Result { Chip = (long)Chip };
                PunchesList = db.Punches.FindAllByChip(Chip).OrderBy(db.Punches.Timestamp) ?? new List<Punch>();

                // Gets Route associated with Competitor and his Category for proper validation
                dynamic routesAlias, competitorAlias;
                RouteStepList = db.RouteSteps
                                .All()
                                .Join(db.Routes, out routesAlias)
                                .On(db.RouteSteps.RouteId == db.Routes.Id)
                                .Join(db.Competitors, out competitorAlias)
                                .On(db.Routes.Category == db.Competitors.Category)
                                .With(routesAlias)
                                .With(competitorAlias)
                                .Where(db.Competitors.Chip == Chip)
                                .OrderBy(db.RouteSteps.Order);
                Punch.CheckCorrectnessOrdered(ref PunchesList, RouteStepList);
            }
            else
            {
                Result = new Result();
                PunchesList = new List<Punch>();
            }

            labelsWP.DataContext = Result;
            punchesLV.ItemsSource = PunchesList;
        }

        private void addB_Click(object sender, RoutedEventArgs e)
        {
            Window window = new PunchForm();
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
            Refresh();
        }

        private void editB_Click(object sender, RoutedEventArgs e)
        {
            Window window = new PunchForm((Punch)punchesLV.SelectedItem);
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
            Refresh();
        }

        private void deleteB_Click(object sender, RoutedEventArgs e)
        {
            if (MessageUtils.ShowDeleteWarning(this) == true)
            {
                var db = MainWindow.GetDatabase();
                foreach (Punch p in punchesLV.SelectedItems)
                    db.Punches.DeleteById(p.Id);
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

        private void resultsLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (e.Source is ListView)
            //    ManageButtons((ListView)e.Source);
            //else
            //    Console.WriteLine("Not ListView: " + e.Source);
            //e.Handled = true;
        }

        private void punchesLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is ListView)
                ManageButtons((ListView)e.Source);
            else
                Console.WriteLine("Not ListView: " + e.Source);
            e.Handled = true;
        }

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
    }
}
