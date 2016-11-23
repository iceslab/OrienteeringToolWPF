using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Windows.Forms.KidsCompetition
{
    public partial class ResultForm : Window, IForm
    {
        private Result result;
        private List<Punch> punchesList;

        public ResultForm()
        {
            InitializeComponent();
            result = new Result();
            //MainWindow.Listener.PropertyChanged += Listener_PropertyChanged;
        }

        public ResultForm(Result r)
        {
            InitializeComponent();
            result = r;
            ObjectToForm();
            PrepareRouteStepsList();
            //MainWindow.Listener.PropertyChanged += Listener_PropertyChanged;
        }

        //private void Listener_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "DataFrame")
        //    {
        //        routeStepsList = RouteStep.Parse(MainWindow.Listener.DataFrame.Punches, (long)result.Id);
        //        Dispatcher.Invoke(
        //            new Action(
        //                delegate ()
        //                {
        //                    routeStepsLV.ItemsSource = routeStepsList;
        //                }));
        //    }
        //}

        private void SaveB_Click(object sender, RoutedEventArgs e)
        {
            var errors = FormToObject();
            if (errors.HasErrors() == false)
            {
                var db = DatabaseUtils.GetDatabase();

                using (var tx = db.BeginTransaction())
                {
                    // Update result
                    result = tx.Results.Upsert(result);

                    // Clear old content
                    tx.Punches.DeleteByChip(result.Chip);

                    Punch inserted = null;
                    // Insert new punches
                    foreach (var p in punchesList)
                    {
                        inserted = tx.Punches.Insert(p);
                    }
                    tx.Commit();
                }

                Close();
            }
            else
            {
                MessageUtils.ShowValidatorErrors(this, errors);
            }
        }

        private void addStepB_Click(object sender, RoutedEventArgs e)
        {
            var window = new PunchForm(result, true);
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                //RefreshOrder();
                punchesList.Add(window.punch);
                punchesLV.Items.Refresh();
            }
        }

        private void editStepB_Click(object sender, RoutedEventArgs e)
        {
            var p = (Punch)punchesLV.SelectedItem;
            if (p != null)
            {
                var window = new PunchForm(p, true);
                window.Owner = this;
                if (window.ShowDialog() == true)
                {
                    //RefreshOrder();
                    punchesLV.Items.Refresh();
                }
            }
        }

        private void deleteStepB_Click(object sender, RoutedEventArgs e)
        {
            var si = punchesLV.SelectedItems;
            foreach (Punch p in si)
                punchesList.Remove(p);

            //RefreshOrder();
            punchesLV.Items.Refresh();
        }

        private void punchesLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (punchesLV.SelectedItem != null)
            {
                editStepB.IsEnabled = true;
                deleteStepB.IsEnabled = true;
            }
            else
            {
                editStepB.IsEnabled = false;
                deleteStepB.IsEnabled = false;
            }
        }

        public void ObjectToForm()
        {
            ChipTB.Text = result.Chip.ToString();
            StartTimeDP.Value = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(result.StartTime);
            CheckTimeDP.Value = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(result.CheckTime);
            FinishTimeDP.Value = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(result.FinishTime);
        }

        public ErrorList FormToObject()
        {
            var errors = ValidateForm();
            if (errors.HasErrors() == false)
            {
                result.Chip = long.Parse(ChipTB.Text);

                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

                TimeSpan span = ((DateTime)StartTimeDP.Value - epoch);
                result.StartTime = Convert.ToInt64(span.TotalSeconds);

                span = ((DateTime)CheckTimeDP.Value - epoch);
                result.CheckTime = Convert.ToInt64(span.TotalSeconds);

                span = ((DateTime)FinishTimeDP.Value - epoch);
                result.FinishTime = Convert.ToInt64(span.TotalSeconds);
            }
            return errors;
        }

        public ErrorList ValidateForm()
        {
            var errors = new ErrorList();
            long n;
            if (long.TryParse(ChipTB.Text, out n) == false)
                errors.Add(Properties.Resources.ResultChip, Properties.Resources.NotANumberError);
            if (StartTimeDP.Value == null)
                errors.Add(Properties.Resources.ResultStartTime, Properties.Resources.InvalidDateError);
            if (CheckTimeDP.Value == null)
                errors.Add(Properties.Resources.ResultCheckTime, Properties.Resources.InvalidDateError);
            if (FinishTimeDP.Value == null)
                errors.Add(Properties.Resources.ResultFinishTime, Properties.Resources.InvalidDateError);
            return errors;
        }

        private void PrepareRouteStepsList()
        {
            var db = DatabaseUtils.GetDatabase();
            punchesList = db.RouteSteps.FindAllByChip(result.Chip);

            punchesLV.ItemsSource = punchesList;
        }

        //private void RefreshOrder()
        //{
        //    for (int i = 0; i < punchesList.Count; ++i)
        //        punchesList[i].Order = i + 1;
        //}
    }
}
