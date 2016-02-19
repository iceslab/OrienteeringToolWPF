using System.Collections.Generic;
using OrienteeringToolWPF.Model;
using System.Data.SQLite;
using System.Data;
using OrienteeringToolWPF.DAO.Base;
using OrienteeringToolWPF.Windows;

namespace OrienteeringToolWPF.DAO.Implementation
{
    public class RelayDAO : BaseRelayDAO
    {
        public override List<Relay> findAll()
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM RELAYS";
            return Select(command);
        }

        public override List<Relay> findAllById(long id)
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM RELAYS WHERE ID=@id";
            command.Parameters.AddWithValue("@id", id);
            return Select(command);
        }

        public override List<Relay> findAllByName(string name)
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM RELAYS WHERE NAME=@name";
            command.Parameters.AddWithValue("@name", name);
            return Select(command);
        }

        public override int insert(Relay obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            if (obj.Id != null)
            {
                command.CommandText =
                    "INSERT INTO RELAYS (ID, NAME) " +
                    "VALUES (@Id,@Name)";
                command.Parameters.AddWithValue("@Id", obj.Id);
            }
            else
            {
                command.CommandText =
                    "INSERT INTO RELAYS (NAME) " +
                    "VALUES (@Name)";
            }

            command.Parameters.AddWithValue("@Name", obj.Name);

            return ExecuteNonQuery(connection, command);
        }

        public override int update(Relay obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "UPDATE RELAYS " +
                "SET NAME=@Name " +
                "WHERE ID=@Id";

            command.Parameters.AddWithValue("@Id", obj.Id);
            command.Parameters.AddWithValue("@Name", obj.Name);

            return ExecuteNonQuery(connection, command);
        }

        public override int delete(Relay obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM RELAYS " +
                "WHERE ID=@Id AND NAME=@Name";

            command.Parameters.AddWithValue("@Id", obj.Id);
            command.Parameters.AddWithValue("@Name", obj.Name);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteById(long id)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM RELAYS WHERE ID=@Id";

            command.Parameters.AddWithValue("@Id", id);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteById(Relay obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM RELAYS WHERE ID=@Id";

            command.Parameters.AddWithValue("@Id", obj.Id);

            return ExecuteNonQuery(connection, command);
        }

        protected override List<Relay> Select(SQLiteCommand command)
        {
            var dataSet = new DataSet();
            using (var connection = MainWindow.GetConnection())
            {
                connection.Open();
                command.Connection = connection;
                var dataAdapter = new SQLiteDataAdapter(command);
                SQLiteCommandBuilder commandBuilder =
                    new SQLiteCommandBuilder(dataAdapter);

                dataAdapter.Fill(dataSet);
            }

            List<Relay> relays = new List<Relay>();

            foreach (DataTable dt in dataSet.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Relay r = new Relay();

                    r.Id = (long)dr.ItemArray[0];
                    r.Name = (string)dr.ItemArray[1];

                    relays.Add(r);
                }
            }

            return relays;
        }
    }
}
