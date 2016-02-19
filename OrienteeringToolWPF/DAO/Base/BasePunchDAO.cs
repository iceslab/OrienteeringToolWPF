using OrienteeringToolWPF.Model;
using System.Collections.Generic;

namespace OrienteeringToolWPF.DAO.Base
{
    public abstract class BasePunchDAO : CommonDAO<Punch>
    {
        public abstract List<Punch> findAllById(long id);
        public abstract List<Punch> findAllByChip(long chip);
        public abstract int deleteById(Punch obj);
        public abstract int deleteById(long id);
        public abstract int deleteByChip(Punch obj);
        public abstract int deleteByChip(long chip);
    }
}
