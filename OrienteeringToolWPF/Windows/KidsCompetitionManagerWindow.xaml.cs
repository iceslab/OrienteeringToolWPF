using OrienteeringToolWPF.CompetitionManagers;
using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Windows
{
    public partial class KidsCompetitionManagerWindow : CommonManager, ICurrentView
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

        public KidsCompetitionManagerWindow(Tournament tournament) : base()
        {
            InitializeComponent();
            managerView.DataContext = this;
            this.tournament = tournament;
        }

        // Starts or resumes competition
        public override void Start()
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
                        db.Update(tournament);

                        // Show window and register listener
                        Show();
                        MainWindow.Listener.PropertyChanged += Listener_PropertyChanged;
                    }
                }
            }
        }

        // Finish competition
        public override void Finish()
        {
            // Save competition finish time
            tournament.FinishedAtTime = DateTime.Now;
            var db = MainWindow.GetDatabase();
            db.Tournament.Update(tournament);

            // Close window
            Close();
        }

        // Resume competition
        protected override void Resume()
        {
            // If not connected to station, ask for connection
            if (MessageUtils.PromptForConnection(this) == true)
            {
                // Show window and register listener
                Show();
                MainWindow.Listener.PropertyChanged += Listener_PropertyChanged;
            }
        }

        // Listener for incoming chip dataframes
        private void Listener_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataFrame")
            {

            }
        }
    }
}
