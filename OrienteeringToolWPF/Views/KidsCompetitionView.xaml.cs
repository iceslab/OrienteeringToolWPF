using OrienteeringToolWPF.DAO.Implementation;
using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Windows;
using OrienteeringToolWPF.Windows.Forms.KidsCompetition;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Views
{
    /// <summary>
    /// Interaction logic for KidsCompetition.xaml
    /// </summary>

    public partial class KidsCompetitionView : UserControl
    {
        public KidsCompetitionView()
        {
            InitializeComponent();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv = (tabControl.SelectedItem as TabItem)?.Content as IRefreshable;
            lv?.Refresh();
        }

    }
}
