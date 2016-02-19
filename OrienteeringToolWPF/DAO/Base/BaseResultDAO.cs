using OrienteeringToolWPF.Model;
using System.Collections.Generic;

namespace OrienteeringToolWPF.DAO.Base
{
    public abstract class BaseResultDAO : CommonDAO<Result>
    {
        public abstract List<Result> findAllByChip(long chip);
        public abstract int deleteByChip(Result obj);
        public abstract int deleteByChip(long chip);
    }
}
