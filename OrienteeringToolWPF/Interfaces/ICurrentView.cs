using System.ComponentModel;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Interfaces
{
    public interface ICurrentView : INotifyPropertyChanged
    {
        UserControl CurrentView { get; set; }
    }
}
