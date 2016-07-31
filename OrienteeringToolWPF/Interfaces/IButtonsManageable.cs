using System.Windows;

namespace OrienteeringToolWPF.Interfaces
{
    interface IButtonsManageable
    {
        void SetButtonsVisibility(Visibility all);
        void SetButtonsVisibility(Visibility add, Visibility edit, Visibility delete);
    }
}
