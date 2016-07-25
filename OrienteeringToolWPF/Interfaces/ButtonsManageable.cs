using System.Windows;

namespace OrienteeringToolWPF.Interfaces
{
    interface ButtonsManageable
    {
        void SetButtonsVisibility(Visibility all);
        void SetButtonsVisibility(Visibility add, Visibility edit, Visibility delete);
    }
}
