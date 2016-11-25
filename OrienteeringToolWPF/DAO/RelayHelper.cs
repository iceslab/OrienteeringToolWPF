using OrienteeringToolWPF.Enumerations;
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
        public static List<Relay> Relays()
        {
            var RelayList = new List<Relay>();
            var db = DatabaseUtils.GetDatabase();
            switch (DatabaseUtils.DatabaseType)
            {
                case DatabaseType.MYSQL:
                    RelayList = (List<Relay>)
                        db.Clubs.All()
                        .Select(db.Clubs.Id.As("Id"), db.Clubs.Name.As("Name"))
                        .Where(db.Clubs.Id != 0);
                    break;
                case DatabaseType.SQLITE3:
                    RelayList = (List<Relay>)db.Relays.All();
                    break;
                default:
                    break;
            }
            return RelayList;
        }

        public static List<Relay> RelaysWithCompetitors()
        {
            var RelayList = new List<Relay>();
            var db = DatabaseUtils.GetDatabase();
            dynamic alias;
            switch (DatabaseUtils.DatabaseType)
            {
                case DatabaseType.MYSQL:
                    RelayList = (List<Relay>)db.Clubs
                            .All()
                            .LeftJoin(db.Competitors, out alias)
                            .On(db.Competitors.RelayId == db.Clubs.Id)
                            .With(alias)
                            .Where(db.Clubs.Id != 0);
                    break;
                case DatabaseType.SQLITE3:
                    RelayList = (List<Relay>)db.Relays
                            .All()
                            .LeftJoin(db.Competitors, out alias)
                            .On(db.Competitors.RelayId == db.Relays.Id)
                            .With(alias);
                    break;
                default:
                    break;
            }
            
            return RelayList;
        }
    }
}
