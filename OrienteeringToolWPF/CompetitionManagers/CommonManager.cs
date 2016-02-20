using OrienteeringToolWPF.Model;
using System.ComponentModel;
using System.Windows;

namespace OrienteeringToolWPF.CompetitionManagers
{
    public abstract class CommonManager : Window, INotifyPropertyChanged
    {
        public CommonManager() { }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract void Start();
        protected abstract void Resume();
        public abstract void Finish();
    }
}
