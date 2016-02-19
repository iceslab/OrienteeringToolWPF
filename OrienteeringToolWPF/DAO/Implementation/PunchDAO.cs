using OrienteeringToolWPF.DAO.Base;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Windows;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace OrienteeringToolWPF.DAO.Implementation
{
    public class PunchDAO : BasePunchDAO
    {
        public override List<Punch> findAll()
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM PUNCHES";
            return Select(command);
        }

        public override List<Punch> findAllByChip(long chip)
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM PUNCHES WHERE CHIP=@chip";
            command.Parameters.AddWithValue("@chip", chip);
            return Select(command);
        }

        public override List<Punch> findAllById(long id)
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM PUNCHES WHERE ID=@id";
            command.Parameters.AddWithValue("@id", id);
            return Select(command);
        }

        public override int insert(Punch obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            if (obj.Id != null)
            {
                command.CommandText =
                    "INSERT INTO PUNCHES (ID, CHIP, CODE, TIMESTAMP) " +
                    "VALUES (@Id,@Chip,@Code,@Timestamp)";
                command.Parameters.AddWithValue("@Id", obj.Id);
            }
            else
            {
                command.CommandText =
                    "INSERT INTO PUNCHES (CHIP, CODE, TIMESTAMP) " +
                    "VALUES (@Chip,@Code,@Timestamp)";
            }

            command.Parameters.AddWithValue("@Chip", obj.Chip);
            command.Parameters.AddWithValue("@Code", obj.Code);
            command.Parameters.AddWithValue("@Timestamp", obj.Timestamp);

            return ExecuteNonQuery(connection, command);
        }

        public override int update(Punch obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "UPDATE PUNCHES " +
                "SET CHIP=@Chip, CODE=@Code, TIMESTAMP=@Timestamp " +
                "WHERE ID=@Id";

            command.Parameters.AddWithValue("@Id", obj.Id);
            command.Parameters.AddWithValue("@Chip", obj.Chip);
            command.Parameters.AddWithValue("@Code", obj.Code);
            command.Parameters.AddWithValue("@Timestamp", obj.Timestamp);

            return ExecuteNonQuery(connection, command);
        }

        public override int delete(Punch obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM PUNCHES " +
                "WHERE ID=@Id AND CHIP=@Chip AND CODE=@Code AND TIMESTAMP=@Timestamp";

            command.Parameters.AddWithValue("@Id", obj.Id);
            command.Parameters.AddWithValue("@Chip", obj.Chip);
            command.Parameters.AddWithValue("@Code", obj.Code);
            command.Parameters.AddWithValue("@Timestamp", obj.Timestamp);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteByChip(long chip)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM PUNCHES WHERE CHIP=@chip";

            command.Parameters.AddWithValue("@chip", chip);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteByChip(Punch obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM PUNCHES WHERE CHIP=@chip";

            command.Parameters.AddWithValue("@chip", obj.Chip);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteById(long id)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM PUNCHES WHERE ID=@Id";

            command.Parameters.AddWithValue("@Id", id);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteById(Punch obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM PUNCHES WHERE ID=@Id";

            command.Parameters.AddWithValue("@Id", obj.Id);

            return ExecuteNonQuery(connection, command);
        }

        protected override List<Punch> Select(SQLiteCommand command)
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

            var competitors = new List<Punch>();

            foreach (DataTable dt in dataSet.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var p = new Punch();

                    p.Id = (long)dr.ItemArray[0];
                    p.Chip = (long)dr.ItemArray[1];
                    p.Code = (long)dr.ItemArray[2];
                    p.Timestamp = (long)dr.ItemArray[3];

                    competitors.Add(p);
                }
            }

            return competitors;
        }
    }
}
