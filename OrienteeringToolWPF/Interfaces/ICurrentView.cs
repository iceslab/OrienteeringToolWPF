using System.ComponentModel;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Interfaces
{
    interface ICurrentView
    {
        UserControl CurrentView { get; set; }
    }
}
