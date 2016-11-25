using OrienteeringToolWPF.DAO;
using OrienteeringToolWPF.Enumerations;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Windows.Forms.KidsCompetition
{
    /// <summary>
    /// Interaction logic for KCCompetitorsForm.xaml
    /// </summary>
    public partial class CompetitorForm : Window, IForm
    {
        private Competitor competitor;
        private List<Relay> relaysList;
        private List<Category> categoriesList;

        private void Listener_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataFrame")
            {
                ChipTB.Dispatcher.Invoke(
                    new Action(
                        delegate ()
                        {
                            ChipTB.Text = MainWindow.Listener.DataFrame.SiNumber;
                        }));
            }
        }
        public CompetitorForm()
        {
            InitializeComponent();
            MainWindow.Listener.PropertyChanged += Listener_PropertyChanged;
            competitor = new Competitor();
            BirthDateDP.SelectedDate =
                new DateTime(DateTime.Now.Year - 8, 1, 1);
        }

        public CompetitorForm(Competitor c) : this()
        {
            InitializeComponent();
            competitor = c;
            ObjectToForm();
        }

        private void SaveB_Click(object sender, RoutedEventArgs e)
        {
            var errors = FormToObject();
            if (errors.HasErrors() == false)
            {
                var db = DatabaseUtils.GetDatabase();
                db.Competitors.Upsert(competitor);

                Close();
            }
            else
            {
                MessageUtils.ShowValidatorErrors(this, errors);
            }
        }

        private void SaveAndNextB_Click(object sender, RoutedEventArgs e)
        {
            var errors = FormToObject();
            if (errors.HasErrors() == false)
            {
                var db = DatabaseUtils.GetDatabase();
                db.Competitors.Upsert(competitor);

                competitor = new Competitor();

                NameTB.Text = "";
                ChipTB.Text = "";
                RelayIdCB.SelectedIndex = -1;
                CategoryCB.SelectedIndex = 0;
                MaleRB.IsChecked = true;
                BirthDateDP.SelectedDate =
                    new DateTime(DateTime.Now.Year - 8, 1, 1);
            }
            else
            {
                MessageUtils.ShowValidatorErrors(this, errors);
            }
        }

        public void ObjectToForm()
        {
            PopulateRelayCB();
            PopulateCategoryCB();

            NameTB.Text = competitor.Name;
            ChipTB.Text = competitor.Chip.ToString();

            RelayIdCB.SelectedItem = null;
            foreach (var el in RelayIdCB.Items)
            {
                if (el is Relay)
                {
                    var r = (Relay)el;
                    if (r.Id == competitor.RelayId)
                    {
                        RelayIdCB.SelectedItem = r;
                        break;
                    }
                }
            }

            CategoryCB.SelectedItem = null;
            foreach (var el in CategoryCB.Items)
            {
                if (el is Category)
                {
                    var c = (Category)el;
                    if (c.Id == competitor.Category)
                    {
                        CategoryCB.SelectedItem = c;
                        break;
                    }
                }
            }

            if (competitor.Gender == Gender.MALE)
                MaleRB.IsChecked = true;
            else
                FemaleRB.IsChecked = true;
            BirthDateDP.SelectedDate = competitor.BirthDate;
        }

        public ErrorList FormToObject()
        {
            var errors = ValidateForm();
            if (errors.HasErrors() == false)
            {
                competitor.Name = NameTB.Text;
                if (string.IsNullOrWhiteSpace(ChipTB.Text))
                    competitor.Chip = null;
                else
                    competitor.Chip = long.Parse(ChipTB.Text);
                competitor.RelayId = (long)((Relay)RelayIdCB.SelectedItem).Id;
                competitor.Category = (long)((Category)CategoryCB.SelectedItem).Id;
                competitor.Gender = (bool)MaleRB.IsChecked ? Gender.MALE : Gender.FEMALE;
                competitor.BirthDate = (DateTime)BirthDateDP.SelectedDate;
            }
            return errors;
        }

        public ErrorList ValidateForm()
        {
            var errors = new ErrorList();
            if (string.IsNullOrWhiteSpace(NameTB.Text))
                errors.Add(Properties.Resources.CompetitorName, Properties.Resources.NullOrEmptyError);
            if (!string.IsNullOrWhiteSpace(ChipTB.Text))
            {
                long n;
                if (!long.TryParse(ChipTB.Text, out n))
                    errors.Add(Properties.Resources.CompetitorChip, Properties.Resources.NotANumberError);
                else
                {
                    var count = DatabaseUtils.GetDatabase().Competitors.GetCountByChip(n);
                    if (count > 1)
                    {
                        errors.Add(Properties.Resources.CompetitorChip, Properties.Resources.ValueAlreadyExistsError);
                    }
                    else if (count == 1)
                    {
                        Competitor c = DatabaseUtils.GetDatabase().Competitors.FindByChip(n);
                        if (c.Id != competitor?.Id)
                            errors.Add(Properties.Resources.CompetitorChip, Properties.Resources.ValueAlreadyExistsError);
                    }
                }
            }

            if (RelayIdCB.SelectedIndex < 0)
                errors.Add(Properties.Resources.CompetitorRelay, Properties.Resources.InvalidRelayError);
            if (CategoryCB.SelectedIndex < 0)
                errors.Add(Properties.Resources.CompetitorCategory, Properties.Resources.InvalidCategoryError);
            if (BirthDateDP.SelectedDate == null)
                errors.Add(Properties.Resources.CompetitorBirthDate, Properties.Resources.InvalidDateError);
            return errors;
        }

        #region RelayCB methods
        private void PopulateRelayCB()
        {
            relaysList = RelayHelper.Relays();
            RelayIdCB.Items.Clear();

            foreach (var relay in relaysList)
            {
                RelayIdCB.Items.Add(relay);
            }
            RelayIdCB.Items.Add("<Dodaj...>");
        }

        private void RelayIdCB_DropDownOpened(object sender, EventArgs e)
        {
            PopulateRelayCB();
        }

        private void RelayIdCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RelayIdCB.SelectedIndex >= 0)
            {
                // When selected index is last possible it means that add option was chosen
                if (RelayIdCB.SelectedIndex == (RelayIdCB.Items.Count - 1))
                {
                    // Create and show proper form
                    var window = new RelayForm();
                    window.Owner = this;
                    window.ShowDialog();

                    // Get current data
                    var newRelays = RelayHelper.Relays();

                    // Check if data was inserted
                    if (newRelays.Count > relaysList.Count)
                    {
                        int i = 0;
                        for (; i < relaysList.Count; ++i)
                        {
                            // Find not matching object
                            if (!newRelays[i].Equals(relaysList[i]))
                                break;
                        }

                        // Refresh view and select new item
                        PopulateRelayCB();
                        RelayIdCB.SelectedIndex = i;
                    }
                    else
                        RelayIdCB.SelectedIndex = -1;
                }
                else
                {
                    // Finds selected item in list and assigns item's id to proper field
                    foreach (var relay in relaysList)
                    {
                        if (relay.Name == ((Relay)RelayIdCB.SelectedItem).Name)
                        {
                            competitor.RelayId = (long)relay.Id;
                            break;
                        }
                    }
                }
            }
        }
        #endregion
        #region CategoryCB methods
        private void PopulateCategoryCB()
        {
            var db = DatabaseUtils.GetDatabase();
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
                    var db = DatabaseUtils.GetDatabase();
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
                            competitor.Category = (long)category.Id;
                            break;
                        }
                    }
                }
            }
        }
        #endregion
    }
}
