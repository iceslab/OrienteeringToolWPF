﻿using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Views.Lists;
using OrienteeringToolWPF.Windows;
using OrienteeringToolWPF.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace OrienteeringToolWPF.Views
{
    /// <summary>
    /// Interaction logic for RelaysAndCompetitorsTreeView.xaml
    /// </summary>
    public partial class RelaysAndCompetitorsView : UserControl, IRefreshable, ICurrentView
    {
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
                    var db = MainWindow.GetDatabase();
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

        public void Refresh()
        {
            var db = MainWindow.GetDatabase();
            dynamic alias;
            RelayList = db.Relays
                            .All()
                            .LeftJoin(db.Competitors, out alias)
                            .On(db.Competitors.RelayId == db.Relays.Id)
                            .With(alias);
            relaysAndCompetitorsTV.ItemsSource = RelayList;
        }

        private void relaysAndCompetitorsTV_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SetNewView(e.NewValue);
            Console.WriteLine(sender);
        }

        private void SetNewView(object NewValue)
        {
            try
            {
                if (NewValue is Competitor)
                {
                    var c = (Competitor)NewValue;
                    var uc = new ResultsAndPunchesListView(c.Chip);
                    uc.SetButtonsVisibility(Visibility.Collapsed);
                    CurrentView = uc;
                }
                else if (NewValue is Relay)
                {
                    var r = (Relay)NewValue;
                    var uc = new CompetitorsListView(r.Id);
                    uc.SetButtonsVisibility(Visibility.Collapsed);
                    CurrentView = uc;
                }
            }
            catch (InvalidCastException) { }
        }

        public void SelectCompetitor(Competitor competitor)
        {
            Dispatcher.Invoke(new Action(() => 
            {
                relaysAndCompetitorsTV.Focus();
                relaysAndCompetitorsTV.SetSelectedItem(competitor);
            }));
        }
    }
}
