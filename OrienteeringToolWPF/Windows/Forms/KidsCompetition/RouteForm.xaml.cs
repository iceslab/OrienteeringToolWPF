using OrienteeringToolWPF.DAO.Implementation;
using OrienteeringToolWPF.Model;
using System;
using System.Collections.Generic;
using System.Windows;

namespace OrienteeringToolWPF.Windows.Forms.KidsCompetition
{
    public partial class RouteForm : Window
    {
        private Route route;
        private List<RouteStep> routeStepsList;

        public RouteForm()
        {
            InitializeComponent();
            route = new Route();
            MainWindow.Listener.PropertyChanged += Listener_PropertyChanged;
        }

        public RouteForm(Route r)
        {
            InitializeComponent();
            route = r;
            ObjectToForm();
            PrepareRouteStepsList();
            MainWindow.Listener.PropertyChanged += Listener_PropertyChanged;
        }

        private void Listener_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataFrame")
            {
                routeStepsList = RouteStep.Parse(MainWindow.Listener.DataFrame.Punches, (long)route.Id);
                Dispatcher.Invoke(
                    new Action(
                        delegate ()
                        {
                            routeStepsLV.ItemsSource = routeStepsList;
                        }));
            }
        }

        private void SaveB_Click(object sender, RoutedEventArgs e)
        {
            if (FormToObject())
            {
                var db = MainWindow.GetDatabase();

                using (var tx = db.BeginTransaction())
                {
                    // Update route
                    route = tx.Routes.Upsert(route);

                    // Clear old content
                    tx.RouteSteps.DeleteByRouteId(route.Id);

                    RouteStep inserted = null;
                    // Insert new route steps
                    foreach (var rs in routeStepsList)
                    {
                        inserted = tx.RouteSteps.Insert(rs);
                    }
                    tx.Commit();
                }
                /*------------------------------*/
                //var routeDao = new RouteDAO();
                //var routeStepDao = new RouteStepDAO();

                //if (route.Id == null)
                //{
                //    routeDao.insert(route);
                //    route = routeDao.findAllByName(route.Name)[0];
                //}
                //else
                //    routeDao.update(route);

                //// Usuwanie zawartości tabeli
                //routeStepDao.deleteByRouteId((long)route.Id);

                //// Zapisywanie zmian w tabeli
                //foreach (var rs in routeStepsList)
                //    routeStepDao.insert(rs);
                /*------------------------------*/

                Close();
            }
            else
            {
                MessageBox.Show(this, "Nieprawidłowe dane", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void addStepB_Click(object sender, RoutedEventArgs e)
        {
            var window = new RouteStepForm(route, true);
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                RefreshOrder();
                routeStepsList.Add(window.routeStep);
                routeStepsLV.Items.Refresh();
            }
        }

        private void editStepB_Click(object sender, RoutedEventArgs e)
        {
            var rs = (RouteStep)routeStepsLV.SelectedItem;
            if (rs != null)
            {
                var window = new RouteStepForm(rs, true);
                window.Owner = this;
                if (window.ShowDialog() == true)
                {
                    RefreshOrder();
                    routeStepsLV.Items.Refresh();
                }
            }
        }

        private void deleteStepB_Click(object sender, RoutedEventArgs e)
        {
            var si = routeStepsLV.SelectedItems;
            foreach (RouteStep rs in si)
                routeStepsList.Remove(rs);

            RefreshOrder();
            routeStepsLV.Items.Refresh();
        }

        private void routeStepsLV_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (routeStepsLV.SelectedItem != null)
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

        private bool FormToObject()
        {
            if (ValidateForm())
            {
                route.Name = NameTB.Text;
                return true;
            }
            return false;
        }

        private void ObjectToForm()
        {
            NameTB.Text = route.Name;
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(NameTB.Text))
                return false;
            return true;
        }

        private void PrepareRouteStepsList()
        {
            var db = MainWindow.GetDatabase();
            if (route.Id != null)
                routeStepsList = db.RouteSteps.FindAllByRouteId(route.Id);

            routeStepsLV.ItemsSource = routeStepsList;
        }

        private void RefreshOrder()
        {
            for (int i = 0; i < routeStepsList.Count; ++i)
                routeStepsList[i].Order = i + 1;
        }
    }
}
