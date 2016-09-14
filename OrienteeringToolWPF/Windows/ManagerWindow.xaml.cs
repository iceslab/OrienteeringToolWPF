using OrienteeringToolWPF.CompetitionManagers;
using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using OrienteeringToolWPF.Views;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Windows
{
    public partial class ManagerWindow : Window, INotifyPropertyChanged, ICurrentView
    {
        public Tournament tournament { get; private set; }
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

        public ManagerWindow(Tournament tournament) : base()
        {
            InitializeComponent();
            //managerWindowCC.DataContext = this;
            this.tournament = tournament;
        }

        // Starts or resumes competition
        public void Start()
        {
            // Competition in progress - resume
            if (tournament.IsRunning == true)
            {
                Resume();
            }
            // Competition finished - show information
            else if (tournament.HasFinished == true)
            {
                MessageUtils.ShowCannotStartFinishedInfo(this);
                // Set current view to Summary
                CurrentView = new SummaryView();
            }
            // Competition starting
            else
            {
                var result = true;

                // When current date is earlier than planned start date
                if (DateTime.Now < tournament.StartTime)
                {
                    // Prompt warning and allow override
                    result = MessageUtils.ShowStartBeforeTimeWarning(this);
                }

                // Actual start of tournament
                if (result == true)
                {
                    // If not connected to station, ask for connection
                    if (MessageUtils.PromptForConnection(this) == true)
                    {
                        // Save actual start of tournament
                        tournament.StartedAtTime = DateTime.Now;
                        var db = MainWindow.GetDatabase();
                        db.Tournament.Update(tournament);

                        // Set current view to Manager
                        CurrentView = new RelaysAndCompetitorsView();
                        // Show window
                        Show();
                    }
                }
            }
        }

        // Finish competition
        public void Finish()
        {
            // Save competition finish time
            tournament.FinishedAtTime = DateTime.Now;
            var db = MainWindow.GetDatabase();
            db.Tournament.Update(tournament);

            // Close window
            Close();
        }

        // Resume competition
        protected void Resume()
        {
            // If not connected to station, ask for connection
            if (MessageUtils.PromptForConnection(this) == true)
            {
                // Set current view to Manager
                CurrentView = new RelaysAndCompetitorsView();
                // Show window
                Show();
            }
        }

        private void finishB_Click(object sender, RoutedEventArgs e)
        {
            Finish();
        }
    }
}
