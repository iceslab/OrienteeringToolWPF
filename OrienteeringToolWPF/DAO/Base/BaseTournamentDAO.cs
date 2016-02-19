using OrienteeringToolWPF.Model;

namespace OrienteeringToolWPF.DAO.Base
{
    public abstract class BaseTournamentDAO : CommonDAO<Tournament>
    {
        public abstract int deleteById(Tournament obj);
        public abstract int deleteById(long id);
    }
}
