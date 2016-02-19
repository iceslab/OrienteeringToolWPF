using OrienteeringToolWPF.Model;
using System.Collections.Generic;

namespace OrienteeringToolWPF.DAO.Base
{
    public abstract class BaseRouteStepDAO : CommonDAO<RouteStep>
    {
        public abstract List<RouteStep> findAllById(long id);
        public abstract List<RouteStep> findAllByCode(long code);
        public abstract List<RouteStep> findAllByRouteID(long routeId);

        public abstract int deleteById(RouteStep obj);
        public abstract int deleteById(long id);
        public abstract int deleteByCode(RouteStep obj);
        public abstract int deleteByCode(long code);
        public abstract int deleteByRouteId(RouteStep obj);
        public abstract int deleteByRouteId(long routeId);
    }
}
