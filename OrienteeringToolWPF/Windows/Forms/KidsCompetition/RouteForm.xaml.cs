using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace OrienteeringToolWPF.Windows.Forms.KidsCompetition
{
    public partial class RouteForm : Window, IForm
    {
        private Route route;
        private ObservableCollection<RouteStep> routeStepsList;
        private List<Category> categoriesList;

        public RouteForm()
        {
            InitializeComponent();
            route = new Route();
            routeStepsList = new ObservableCollection<RouteStep>();
            routeStepsLV.ItemsSource = routeStepsList;
            MainWindow.Listener.PropertyChanged += Listener_PropertyChanged;
        }

        public RouteForm(Route r) : this()
        {
            Debug.Assert(r != null, "Route object passed to form is null");
            route = r;
            ObjectToForm();
            PrepareRouteStepsList();
        }

        private void Listener_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataFrame")
            {
                var list = RouteStep.Parse(MainWindow.Listener.DataFrame.Punches, (long)route.Id);
                routeStepsList.Clear();
                foreach (var rs in list)
                {
                    routeStepsList.Add(rs);
                }
            }
        }

        private void SaveB_Click(object sender, RoutedEventArgs e)
        {
            var errors = FormToObject();
            if (errors.HasErrors() == false)
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
                        rs.RouteId = (long)route.Id;
                        inserted = tx.RouteSteps.Insert(rs);
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
            var window = new RouteStepForm(route, routeStepsList.Count, true);
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                routeStepsList.Insert(Convert.ToInt32(window.routeStep.Order - 1), window.routeStep);
                RefreshOrder();
            }
        }

        private void editStepB_Click(object sender, RoutedEventArgs e)
        {
            // TODO: When editing SelectedItem does not deselect
            var rs = (RouteStep)routeStepsLV.SelectedItem;
            if (rs != null)
            {
                var window = new RouteStepForm(rs, routeStepsList.Count, true);
                window.Owner = this;
                if (window.ShowDialog() == true)
                {
                    routeStepsList.Remove(rs);
                    routeStepsList.Insert(Convert.ToInt32(window.routeStep.Order - 1), window.routeStep);
                    RefreshOrder();
                }
            }
        }

        private void deleteStepB_Click(object sender, RoutedEventArgs e)
        {
            var si = routeStepsLV.SelectedItems;
            var sidx = new List<RouteStep>();

            foreach (RouteStep rs in si)
                sidx.Add(rs);
            foreach (RouteStep rs in sidx)
                routeStepsList.Remove(rs);

            RefreshOrder();
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

        public void ObjectToForm()
        {
            PopulateCategoryCB();
            NameTB.Text = route.Name;

            CategoryCB.SelectedItem = null;
            foreach (var el in CategoryCB.Items)
            {
                if (el is Category)
                {
                    var c = (Category)el;
                    if (c.Id == route.Category)
                    {
                        CategoryCB.SelectedItem = c;
                        break;
                    }
                }
            }
        }

        public ErrorList FormToObject()
        {
            var errors = ValidateForm();
            if (errors.HasErrors() == false)
            {
                route.Name = NameTB.Text;
                route.Category = (long)((Category)CategoryCB.SelectedItem).Id;
            }
            return errors;
        }

        public ErrorList ValidateForm()
        {
            var errors = new ErrorList();
            if (string.IsNullOrWhiteSpace(NameTB.Text))
                errors.Add(Properties.Resources.RouteName, Properties.Resources.NotANumberError);
            if (CategoryCB.SelectedIndex < 0)
                errors.Add(Properties.Resources.CompetitorCategory, Properties.Resources.InvalidCategoryError);
            return errors;
        }

        private void PrepareRouteStepsList()
        {
            var db = MainWindow.GetDatabase();
            List<RouteStep> list = db.RouteSteps.FindAllByRouteId(route.Id);
            routeStepsList.Clear();
            foreach (var rs in list)
            {
                routeStepsList.Add(rs);
            }
        }

        private void RefreshOrder()
        {
            for (int i = 0; i < routeStepsList.Count; ++i)
                routeStepsList[i].Order = i + 1;
            Dispatcher.Invoke(
                delegate 
                {
                    routeStepsLV.UnselectAll();
                });
        }

        #region CategoryCB methods
        private void PopulateCategoryCB()
        {
            var db = MainWindow.GetDatabase();
            categoriesList = db.Categories.All();

            CategoryCB.Items.Clear();

            foreach (var category in categoriesList)
            {
                CategoryCB.Items.Add(category);
            }
            CategoryCB.Items.Add("<Dodaj...>");
        }

        private void CategoryCB_DropDownOpened(object sender, EventArgs e)
        {
            PopulateCategoryCB();
        }

        private void CategoryCB_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // When selected index is valid
            if (CategoryCB.SelectedIndex >= 0)
            {
                // When selected index is last possible it means that add option was chosen
                if (CategoryCB.SelectedIndex == (CategoryCB.Items.Count - 1))
                {
                    // Create and show proper form
                    var window = new CategoryForm();
                    window.Owner = this;
                    window.ShowDialog();

                    // Get current data
                    var db = MainWindow.GetDatabase();
                    List<Category> newCategories = db.Categories.All();

                    // Check if data was inserted
                    if (newCategories.Count > categoriesList.Count)
                    {
                        int i = 0;
                        for (; i < categoriesList.Count; ++i)
                        {
                            // Find not matching object
                            if (!newCategories[i].Equals(categoriesList[i]))
                                break;
                        }

                        // Refresh view and select new item
                        PopulateCategoryCB();
                        CategoryCB.SelectedIndex = i;
                    }
                    else
                        CategoryCB.SelectedIndex = -1;
                }
                else
                {
                    // Finds selected item in list and assigns item's id to proper field
                    foreach (var category in categoriesList)
                    {
                        if (category.Name == ((Category)CategoryCB.SelectedItem).Name)
                        {
                            route.Category = (long)category.Id;
                            break;
                        }
                    }
                }
            }
        }
        #endregion
    }
}
