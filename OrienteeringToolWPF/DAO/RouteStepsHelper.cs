using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrienteeringToolWPF.DAO
{
    public static class RouteStepsHelper
    {
        // Gets Route associated with Competitor and his Category
        public static List<RouteStep> RouteStepsWhereChip(long Chip)
        {
            var db = DatabaseUtils.GetDatabase();
            dynamic routesAlias, competitorAlias;
            var RouteStepList = (List<RouteStep>)db.RouteSteps
                            .All()
                            .Join(db.Routes, out routesAlias)
                            .On(db.RouteSteps.RouteId == db.Routes.Id)
                            .Join(db.Competitors, out competitorAlias)
                            .On(db.Routes.Category == db.Competitors.Category)
                            .With(routesAlias)
                            .With(competitorAlias)
                            .Where(db.Competitors.Chip == Chip)
                            .OrderBy(db.RouteSteps.Order);
            return RouteStepList;
        }
    }
}
