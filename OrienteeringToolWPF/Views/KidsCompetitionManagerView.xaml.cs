using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Views
{
    /// <summary>
    /// Interaction logic for KidsCompetition.xaml
    /// </summary>

    public partial class KidsCompetitionManagerView : UserControl
    {
        public KidsCompetitionManagerView()
        {
            InitializeComponent();
            MainWindow.Listener.PropertyChanged += Listener_PropertyChanged;
        }

        private void Listener_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataFrame")
            {
                var result = new Result(MainWindow.Listener.DataFrame);
                Competitor competitor = null;
                foreach (var relay in relaysTV.RelayList)
                {
                    competitor = ((List<Competitor>)relay.Competitors).Find(y
                        => y.Chip == result.Chip);
                    if (competitor != null)
                        break;
                }

                //foreach(var item in relaysTV.)

                if(competitor != null)
                {
                    var punchesList = Punch.Parse(MainWindow.Listener.DataFrame.Punches, result.Chip);
                    //relaysTV.
                }

                if (relaysTV.RelayList.Exists(x =>
                    ((List<Competitor>)x.Competitors).Exists(y
                        => y.Chip == result.Chip)))
                {
                    
                }
            }
        }
    }
}
