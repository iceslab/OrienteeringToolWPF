using System;
using System.ComponentModel;

namespace OrienteeringToolWPF.Model
{
    public enum CourseEnum : long
    {
        [Description("Czas startu to czas odbity na chipie")]
        START_ON_CHIP = 0L,
        [Description("Czas startu to czas mety poprzednika")]
        START_CALCULATED = 1L

    }

    public class Tournament : BaseModel, INotifyPropertyChanged
    {
        public long? Id { get; set; }
        protected DateTime _startTime;
        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    OnPropertyChanged("StartTime");
                }
            }
        }
        protected DateTime? _startedAtTime;
        public DateTime? StartedAtTime
        {
            get { return _startedAtTime; }
            set
            {
                if(_startedAtTime != value)
                {
                    _startedAtTime = value;
                    OnPropertyChanged("StartedAtTime");
                    if (_startedAtTime != null)
                        HasStarted = true;
                    else
                    {
                        HasStarted = false;
                        FinishedAtTime = null;
                    }
                }
            }
        }
        protected DateTime? _finishedAtTime;
        public DateTime? FinishedAtTime
        {
            get { return _finishedAtTime; }
            set
            {
                if(_finishedAtTime != value)
                {
                    _finishedAtTime = value;
                    OnPropertyChanged("FinishedAtTime");
                    if (_finishedAtTime != null)
                        HasFinished = true;
                    else
                        HasFinished = false;

                    DetermineIsRunning();
                }
            }
        }
        public string Name { get; set; }
        public CourseEnum CourseType { get; set; }
        public string Description { get; set; }

        protected bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            private set
            {
                if(_isRunning != value)
                {
                    _isRunning = value;
                    OnPropertyChanged("IsRunning");
                }
            }
        }
        protected bool _hasStarted;
        public bool HasStarted
        {
            get { return _hasStarted; }
            private set
            {
                if(_hasStarted != value)
                {
                    _hasStarted = value;
                    OnPropertyChanged("HasStarted");
                    DetermineIsRunning();
                }
            }
        }
        protected bool _hasFinished;
        public bool HasFinished
        {
            get { return _hasFinished; }
            private set
            {
                if(_hasFinished != value)
                {
                    _hasFinished = value;
                    OnPropertyChanged("HasFinished");
                    DetermineIsRunning();
                }
            }
        }

        public Tournament()
        {
            Id = null;
            _startTime = DateTime.Now;
            _startedAtTime = null;
            _finishedAtTime = null;
            Name = "";
            CourseType = CourseEnum.START_ON_CHIP;
            Description = null;
            _isRunning = false;
            _hasStarted = false;
            _hasFinished = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Determines value of IsRunning property
        private void DetermineIsRunning()
        {
            // If tournament has started
            if (HasStarted)
            {
                if (HasFinished)
                    IsRunning = false;
                else
                    IsRunning = true;
            }
        }
    }
}
