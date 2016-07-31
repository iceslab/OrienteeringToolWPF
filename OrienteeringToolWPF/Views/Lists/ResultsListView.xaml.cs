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
    public partial class ResultsAndPunchesListView : UserControl, Refreshable, ButtonsManageable
    {
        public long? Chip { get; set; }
        public Result Result { get; private set; }
        public List<Punch> PunchesList { get; private set; }

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
                Result = db.Results.FindAllByChip(Chip).FirstOrDefault() ?? new Result();
                PunchesList = db.Punches.FindAllByChip(Chip) ?? new List<Punch>();
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
            if (e.Source is ListView)
                ManageButtons((ListView)e.Source);
            else
                Console.WriteLine("Not ListView: " + e.Source);
        }

        private void punchesLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (e.Source is ListView)
            //    ManageButtons((ListView)e.Source);
            //else
            //    Console.WriteLine("Not ListView: " + e.Source);
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
