using OrienteeringToolWPF.DAO.Base;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Windows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace OrienteeringToolWPF.DAO.Implementation
{
    public class CompetitorDAO : BaseCompetitorDAO
    {
        public override List<Competitor> findAll()
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM COMPETITORS";
            return Select(command);
        }

        public override List<Competitor> findAllByChip(long chip)
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM COMPETITORS WHERE CHIP=@chip";
            command.Parameters.AddWithValue("@chip", chip);
            return Select(command);
        }

        public override List<Competitor> findAllById(long id)
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM COMPETITORS WHERE ID=@id";
            command.Parameters.AddWithValue("@id", id);
            return Select(command);
        }

        public override List<Competitor> findAllByName(string name)
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM COMPETITORS WHERE NAME=@name";
            command.Parameters.AddWithValue("@name", name);
            return Select(command);
        }

        public override int insert(Competitor obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            if (obj.Id != null)
            {
                command.CommandText =
                    "INSERT INTO COMPETITORS (ID, NAME, CHIP, RELAY_ID, CLASS, GENDER, BIRTH_DATE) " +
                    "VALUES (@Id,@Name,@Chip,@RelayId,@Class,@Gender,@BirthDate)";
                command.Parameters.AddWithValue("@Id", obj.Id);
            }
            else
            {
                command.CommandText =
                    "INSERT INTO COMPETITORS (NAME, CHIP, RELAY_ID, CLASS, GENDER, BIRTH_DATE) " +
                    "VALUES (@Name,@Chip,@RelayId,@Class,@Gender,@BirthDate)";
            }

            command.Parameters.AddWithValue("@Name", obj.Name);
            command.Parameters.AddWithValue("@Chip", obj.Chip);
            command.Parameters.AddWithValue("@RelayId", obj.RelayId);
            command.Parameters.AddWithValue("@Class", obj.Category);
            command.Parameters.AddWithValue("@Gender", obj.Gender);
            command.Parameters.AddWithValue("@BirthDate", obj.BirthDate);

            return ExecuteNonQuery(connection, command);
        }

        public override int update(Competitor obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "UPDATE COMPETITORS " +
                "SET NAME=@Name, CHIP=@Chip, RELAY_ID=@RelayId, CLASS=@Class, GENDER=@Gender, BIRTH_DATE=@BirthDate " +
                "WHERE ID=@Id";

            command.Parameters.AddWithValue("@Id", obj.Id);
            command.Parameters.AddWithValue("@Name", obj.Name);
            command.Parameters.AddWithValue("@Chip", obj.Chip);
            command.Parameters.AddWithValue("@RelayId", obj.RelayId);
            command.Parameters.AddWithValue("@Class", obj.Category);
            command.Parameters.AddWithValue("@Gender", obj.Gender);
            command.Parameters.AddWithValue("@BirthDate", obj.BirthDate);

            return ExecuteNonQuery(connection, command);
        }

        public override int delete(Competitor obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM COMPETITORS " +
                "WHERE ID=@Id AND NAME=@Name AND CHIP=@Chip AND RELAY_ID=@RelayId AND " +
                "CLASS=@Class AND GENDER=@Gender AND BIRTH_DATE=@BirthDate";

            command.Parameters.AddWithValue("@Id", obj.Id);
            command.Parameters.AddWithValue("@Name", obj.Name);
            command.Parameters.AddWithValue("@Chip", obj.Chip);
            command.Parameters.AddWithValue("@RelayId", obj.RelayId);
            command.Parameters.AddWithValue("@Class", obj.Category);
            command.Parameters.AddWithValue("@Gender", obj.Gender);
            command.Parameters.AddWithValue("@BirthDate", obj.BirthDate);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteById(Competitor obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM COMPETITORS WHERE ID=@Id";

            command.Parameters.AddWithValue("@Id", obj.Id);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteById(long id)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM COMPETITORS WHERE ID=@Id";

            command.Parameters.AddWithValue("@Id", id);

            return ExecuteNonQuery(connection, command);
        }

        protected override List<Competitor> Select(SQLiteCommand command)
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

            var competitors = new List<Competitor>();

            foreach (DataTable dt in dataSet.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var c = new Competitor();

                    c.Id = (long)dr.ItemArray[0];
                    c.Name = (string)dr.ItemArray[1];
                    c.Chip = (long)dr.ItemArray[2];
                    c.RelayId = (long)dr.ItemArray[3];
                    c.Category = (long)dr.ItemArray[4];
                    c.Gender = (GenderEnum)dr.ItemArray[5];
                    c.BirthDate = (DateTime)dr.ItemArray[6];

                    competitors.Add(c);
                }
            }

            return competitors;
        }
    }
}
