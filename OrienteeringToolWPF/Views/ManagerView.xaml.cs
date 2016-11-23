using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using OrienteeringToolWPF.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Views
{
    /// <summary>
    /// Interaction logic for KidsCompetition.xaml
    /// </summary>

    public partial class ManagerView : UserControl, ICurrentView
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

        public ManagerView()
        {
            InitializeComponent();
            //managerViewCC.DataContext = this;
            relaysTV.PropertyChanged += RelaysTV_PropertyChanged;
            MainWindow.Listener.PropertyChanged += Listener_PropertyChanged;
        }

        private void RelaysTV_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentView")
            {
                CurrentView = relaysTV.CurrentView;
            }
        }

        private void Listener_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataFrame")
            {
                var result = new Result(MainWindow.Listener.DataFrame);
                Competitor competitor = null;
                foreach (var relay in relaysTV.RelayList)
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
                    relaysTV.SelectCompetitor(competitor);
                }
            }
        }
    }
}
