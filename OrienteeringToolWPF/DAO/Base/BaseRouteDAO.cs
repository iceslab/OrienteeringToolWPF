using OrienteeringToolWPF.Model;
using System.Collections.Generic;

namespace OrienteeringToolWPF.DAO.Base
{
    public abstract class BaseRouteDAO : CommonDAO<Route>
    {
        public abstract List<Route> findAllById(long id);
        public abstract List<Route> findAllByName(string name);
        public abstract int deleteById(Route obj);
        public abstract int deleteById(long id);
    }
}
