using OrienteeringToolWPF.DAO;
using OrienteeringToolWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OrienteeringToolWPF.Utils
{
    public static class ClassificationUtils
    {
        // Performs corectness check on all competitors
        private static void CheckCorrectness(List<Relay> RelayList, DependencyObject obj = null)
        {
            foreach (var relay in RelayList)
            {
                foreach (var competitor in relay.Competitors)
                {
                    var db = DatabaseUtils.GetDatabase();
                    var punchesList = (List<Punch>)competitor.Punches;
                    var routeStepsList = RouteStepsHelper.RouteStepsWhereChip((long)competitor.Chip);
                    try
                    {
                        Punch.CheckCorrectnessSorted(ref punchesList, routeStepsList);
                    }
                    catch (Exception e)
                    {
                        MessageUtils.ShowException(obj, "Nie wszyscy zawodnicy mają wyniki", e);
                    }

                    competitor.Punches = punchesList;
                }
            }
        }

        // Performs general classification
        public static void ClassifyAll(List<Relay> RelayList)
        {
            ClassifyCompetitors(RelayList);
            ClassifyRelays(RelayList);
        }

        // Classifies competitors
        private static void ClassifyCompetitors(List<Relay> RelayList)
        {
            CheckCorrectness(RelayList);
            foreach (var relay in RelayList)
            {
                ((List<Competitor>)relay.Competitors).Sort();
            }
        }

        // Classifies relays
        private static void ClassifyRelays(List<Relay> RelayList)
        {
            RelayList.Sort();
        }
    }
}
