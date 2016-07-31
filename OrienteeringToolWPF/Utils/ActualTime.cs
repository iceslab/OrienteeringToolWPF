using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace OrienteeringToolWPF.Utils
{
    class ActualTime : INotifyPropertyChanged
    {
        public ActualTime()
        {
            FormatTimeString = "HH:mm:ss";
            _currentTime = DateTime.Now;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now;
        }

        private DateTime _currentTime;
        public DateTime CurrentTime
        {
            get { return _currentTime; }
            set
            {
                _currentTime = value;
                OnPropertyChanged("CurrentTime");
                OnPropertyChanged("FormattedCurrentTime");
            }
        }
        public string FormattedCurrentTime
        {
            get { return CurrentTime.ToString(FormatTimeString); }
        }
        public string FormatTimeString;

        private DispatcherTimer timer;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
