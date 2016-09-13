using OrienteeringToolWPF.Interfaces;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Views
{
    /// <summary>
    /// Interaction logic for KidsCompetition.xaml
    /// </summary>

    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv = (tabControl.SelectedItem as TabItem)?.Content as IRefreshable;
            lv?.Refresh();
            e.Handled = true;
        }

    }
}
