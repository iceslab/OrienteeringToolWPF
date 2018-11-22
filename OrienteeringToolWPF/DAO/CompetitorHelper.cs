using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrienteeringToolWPF.DAO
{
    public static class CompetitorHelper
    {
        public static List<Competitor> CompetitorsJoinedWhereRelayId(long RelayId)
        {
            var db = DatabaseUtils.GetDatabase();
            dynamic resultsAlias, punchAlias;
            var CompetitorList = (List<Competitor>)db.Competitors.FindAllByRelayId(RelayId)
                        .LeftJoin(db.Results, out resultsAlias)
                        .On(db.Results.Chip == db.Competitors.Chip)
                        .LeftJoin(db.Punches, out punchAlias)
                        .On(db.Punches.Chip == db.Competitors.Chip)
                        .With(resultsAlias)
                        .With(punchAlias);

            foreach (var competitor in CompetitorList)
            {
                var punches = (List<Punch>)competitor.Punches;
                try
                {
                    punches?.Sort();
                    Punch.CalculateDeltaStart(ref punches, competitor.Result.StartTime);
                    Punch.CalculateDeltaPrevious(ref punches);
                }
                catch (NullReferenceException) { }
                competitor.Punches = punches;
            }

            return CompetitorList;
        }
    }
}
