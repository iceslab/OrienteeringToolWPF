using OrienteeringToolWPF.DAO.Implementation;
using OrienteeringToolWPF.Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Windows.Forms.KidsCompetition
{
    /// <summary>
    /// Interaction logic for KCCompetitorsForm.xaml
    /// </summary>
    public partial class CompetitorForm : Window
    {
        private Competitor competitor;
        private List<Relay> relaysList;

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

        public CompetitorForm(Competitor c)
        {
            InitializeComponent();
            competitor = c;
            ObjectToForm();
        }

        private void SaveB_Click(object sender, RoutedEventArgs e)
        {
            if (FormToObject())
            {
                var db = MainWindow.GetDatabase();
                db.Competitors.Upsert(competitor);
                Close();
            }
            else
            {
                MessageBox.Show(this, "Nieprawidłowe dane", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveAndNextB_Click(object sender, RoutedEventArgs e)
        {
            if (FormToObject())
            {
                var db = MainWindow.GetDatabase();
                db.Competitors.Upsert(competitor);

                competitor = new Competitor();

                NameTB.Text = "";
                ChipTB.Text = "";
                RelayIdCB.SelectedIndex = -1;
                ClassCB.SelectedIndex = 0;
                MaleRB.IsChecked = true;
                BirthDateDP.SelectedDate =
                    new DateTime(DateTime.Now.Year - 8, 1, 1);
            }
            else
            {
                MessageBox.Show(this, "Nieprawidłowe dane", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool FormToObject()
        {
            if (ValidateForm())
            {
                competitor.Name = NameTB.Text;
                competitor.Chip = long.Parse(ChipTB.Text);
                competitor.RelayId = (long)((Relay)RelayIdCB.SelectedItem).Id;
                competitor.Class = long.Parse((string)((ComboBoxItem)ClassCB.SelectedItem).Content);
                competitor.Gender = (bool)MaleRB.IsChecked ? GenderEnum.MALE : GenderEnum.FEMALE;
                competitor.BirthDate = (DateTime)BirthDateDP.SelectedDate;
                return true;
            }
            return false;
        }

        private void ObjectToForm()
        {
            PopulateRelayCB();

            NameTB.Text = competitor.Name;
            ChipTB.Text = competitor.Chip.ToString();

            RelayIdCB.SelectedItem = null;
            foreach(var el in RelayIdCB.Items)
            {
                if(el is Relay)
                {
                    var r = (Relay) el;
                    if (r.Id == competitor.RelayId)
                    {
                        RelayIdCB.SelectedItem = r;
                        break;
                    }
                }
            }

            ClassCB.SelectedIndex = -1;
            foreach (ComboBoxItem cbi in ClassCB.Items)
            {
                if (long.Parse((string)cbi.Content) == competitor.Class)
                {
                    ClassCB.SelectedItem = cbi;
                    break;
                }
            }
            
            if (competitor.Gender == GenderEnum.MALE)
                MaleRB.IsChecked = true;
            else
                FemaleRB.IsChecked = true;
            BirthDateDP.SelectedDate = competitor.BirthDate;
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(NameTB.Text))
                return false;
            long n;
            if (!long.TryParse(ChipTB.Text, out n))
                return false;
            if (RelayIdCB.SelectedIndex < 0)
                return false;
            if (ClassCB.SelectedIndex < 0)
                return false;
            if (BirthDateDP.SelectedDate == null)
                return false;
            return true;
        }

        private void PopulateRelayCB()
        {
            var db = MainWindow.GetDatabase();
            relaysList = db.Relays.All();

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
            if(RelayIdCB.SelectedIndex >= 0)
            {
                if(RelayIdCB.SelectedIndex == (RelayIdCB.Items.Count - 1))
                {
                    var window = new RelayForm();
                    window.Owner = this;
                    window.ShowDialog();

                    var db = MainWindow.GetDatabase();
                    List<Relay> newRelays = db.Relays.All();

                    if(newRelays.Count > relaysList.Count)
                    {
                        int i = 0;
                        for (; i < relaysList.Count; ++i)
                        {
                            if (!newRelays[i].Equals(relaysList[i]))
                                break;
                        }
                        PopulateRelayCB();
                        RelayIdCB.SelectedIndex = i;
                    }
                    else
                        RelayIdCB.SelectedIndex = -1;
                }
                else
                {
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
    }
}
