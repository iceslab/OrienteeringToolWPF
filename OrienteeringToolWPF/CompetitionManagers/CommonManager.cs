using OrienteeringToolWPF.Model;
using System.ComponentModel;
using System.Windows;

namespace OrienteeringToolWPF.CompetitionManagers
{
    public abstract class CommonManager : Window, INotifyPropertyChanged
    {
        public CommonManager()
        {
            _isRunning = false;
            _hasStarted = false;
            _hasFinished = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                OnPropertyChanged("IsRunning");
            }
        }
        protected bool _hasStarted;
        public bool HasStarted
        {
            get { return _hasStarted; }
            set
            {
                _hasStarted = value;
                OnPropertyChanged("HasStarted");
            }
        }
        protected bool _hasFinished;
        public bool HasFinished
        {
            get { return _hasFinished; }
            set
            {
                _hasFinished = value;
                OnPropertyChanged("HasFinished");
            }
        }
        public abstract void Start();
        protected abstract void Resume();
        public abstract void Finish();
    }
}
