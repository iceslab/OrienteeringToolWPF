using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrienteeringToolWPF.DAO
{
    public static class TournamentHelper
    {
        public static Tournament Tournament()
        {
            var tournament = new Tournament();
            var db = DatabaseUtils.GetDatabase();
            switch (DatabaseUtils.DatabaseType)
            {
                case Enumerations.DatabaseType.MYSQL:
                    tournament = db.CompetitionInfo.All()
                        .Select(db.CompetitionInfo.IdCompetitionInfo.As("id"),
                                db.CompetitionInfo.StartTime,
                                db.CompetitionInfo.StartedAtTime,
                                db.CompetitionInfo.FinishedAtTime,
                                db.CompetitionInfo.Name,
                                db.CompetitionInfo.CourseType,
                                db.CompetitionInfo.Description)
                        .FirstOrDefault();
                    if (tournament.Id != null)
                        db.CompetitionInfo.DeleteAll(db.CompetitionInfo.IdCompetitionInfo != tournament.Id);
                    break;
                case Enumerations.DatabaseType.SQLITE3:
                    tournament = db.Tournament.All().FirstOrDefault();
                    if (tournament.Id != null)
                        db.Tournament.DeleteAll(db.Tournament.Id != tournament.Id);
                    break;
                default:
                    break;
            }
            return tournament;
        }
    }
}
