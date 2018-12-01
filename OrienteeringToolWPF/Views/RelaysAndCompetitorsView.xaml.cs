using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Views.Lists;
using OrienteeringToolWPF.Windows;
using OrienteeringToolWPF.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using OrienteeringToolWPF.DAO;

namespace OrienteeringToolWPF.Views
{
    /// <summary>
    /// Interaction logic for RelaysAndCompetitorsTreeView.xaml
    /// </summary>
    public partial class RelaysAndCompetitorsView : UserControl, IRefreshable, ICurrentView
    {
        public List<Relay> RelayList { get; private set; }
        
        public RelaysAndCompetitorsView()
        {
            InitializeComponent();
            MainWindow.Listener.PropertyChanged += Listener_PropertyChanged;
            Refresh();
        }

        private void Listener_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataFrame")
            {
                var result = new Result(MainWindow.Listener.DataFrame);
                Competitor competitor = null;
                foreach (var relay in RelayList)
                {
                    competitor = ((List<Competitor>)relay.Competitors).Find(y
                        => y.Chip == result.Chip);
                    if (competitor != null)
                        break;
                }

                // If competitor exists insert punches and select from tree view
                if (competitor != null)
                {
                    var punchesList = Punch.Parse(MainWindow.Listener.DataFrame.Punches, result.Chip);
                    var db = DatabaseUtils.GetDatabase();
                    using (var tx = db.BeginTransaction())
                    {
                        tx.Results.Upsert(result);
                        var deleted = tx.Punches.DeleteByChip(result.Chip);
                        Punch inserted = null;
                        foreach (var p in punchesList)
                        {
                            inserted = tx.Punches.Insert(p);
                        }
                        tx.Commit();
                    }
                    SelectCompetitor(competitor);
                }
            }
        }

        // Handler for SelectedItemChanged event
        private void relaysAndCompetitorsTV_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SetNewView(e.NewValue);
            Console.WriteLine(sender);
        }

        // Sets new view on the right side by type of NewValue
        private void SetNewView(object NewValue)
        {
            try
            {
                if (NewValue is Competitor)
                {
                    var c = (Competitor)NewValue;
                    var uc = new ResultsAndPunchesListView(c.Chip);
                    CurrentView = uc;
                }
                else if (NewValue is Relay)
                {
                    var r = (Relay)NewValue;
                    var uc = new CompetitorsListView(r.Id);
                    CurrentView = uc;
                }
            }
            catch (InvalidCastException) { }
        }

        // Selects and focuses on desired competitor in tree view
        public void SelectCompetitor(Competitor competitor)
        {
            Dispatcher.Invoke(new Action(() => 
            {
                relaysAndCompetitorsTV.Focus();
                relaysAndCompetitorsTV.SetSelectedItem(competitor);
            }));
        }

        #region ICurrentView implementation
        private UserControl _currentView;
        public UserControl CurrentView
        {
            get { return _currentView; }

            set
            {
                _currentView = value;
                OnPropertyChanged("CurrentView");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        #region IRefreshable implementation
        // Refreshes data
        public void Refresh()
        {
            RelayList = RelayHelper.RelaysWithCompetitors();
            relaysAndCompetitorsTV.ItemsSource = RelayList;
        }
        #endregion
    }
}
