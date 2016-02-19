using OrienteeringToolWPF.Model;
using System.Collections.Generic;

namespace OrienteeringToolWPF.DAO.Base
{
    public abstract class BaseRelayDAO : CommonDAO<Relay>
    {
        public abstract List<Relay> findAllById(long id);
        public abstract List<Relay> findAllByName(string name);
        public abstract int deleteById(Relay obj);
        public abstract int deleteById(long id);
    }
}
