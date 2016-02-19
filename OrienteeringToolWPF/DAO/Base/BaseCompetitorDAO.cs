using OrienteeringToolWPF.Model;
using System.Collections.Generic;

namespace OrienteeringToolWPF.DAO.Base
{
    public abstract class BaseCompetitorDAO : CommonDAO<Competitor>
    {
        public abstract List<Competitor> findAllById(long id);
        public abstract List<Competitor> findAllByName(string name);
        public abstract List<Competitor> findAllByChip(long chip);
        public abstract int deleteById(Competitor obj);
        public abstract int deleteById(long id);
    }
}
