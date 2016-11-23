using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrienteeringToolWPF.DAO
{
    public static class RelayHelper
    {
        public static List<Relay> RelaysWithCompetitors()
        {
            var db = DatabaseUtils.GetDatabase();
            dynamic alias;
            var RelayList = (List<Relay>)db.Relays
                            .All()
                            .LeftJoin(db.Competitors, out alias)
                            .On(db.Competitors.RelayId == db.Relays.Id)
                            .With(alias);
            return RelayList;
        }
    }
}
