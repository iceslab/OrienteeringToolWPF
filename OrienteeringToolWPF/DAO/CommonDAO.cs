using OrienteeringToolWPF.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrienteeringToolWPF.DAO.Implementation;

namespace OrienteeringToolWPF.DAO
{
    public abstract class CommonDAO<T> where T : BaseModel
    {
        public abstract List<T> findAll();
        public abstract int insert(T obj);
        public abstract int update(T obj);
        public abstract int delete(T obj);
        protected abstract List<T> Select(SQLiteCommand command);
        protected static int ExecuteNonQuery(SQLiteConnection connection, SQLiteCommand command)
        {
            try
            {
                connection.Open();
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

    }
}
