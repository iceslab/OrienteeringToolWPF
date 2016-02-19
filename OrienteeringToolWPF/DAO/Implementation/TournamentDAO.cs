using OrienteeringToolWPF.DAO.Base;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Windows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace OrienteeringToolWPF.DAO.Implementation
{
    public class TournamentDAO : BaseTournamentDAO
    {
        public override List<Tournament> findAll()
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM TOURNAMENT";
            return Select(command);
        }

        public override int insert(Tournament obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            if (obj.Id != null)
            {
                command.CommandText =
                "INSERT INTO TOURNAMENT " +
                "(ID, START_TIME, STARTED_AT_TIME, FINISHED_AT_TIME, NAME, COURSE_TYPE, DESCRIPTION) " +
                "VALUES (@Id,@StartTime,@StartedAtTime,@FinishedAtTime,@Name,@CourseType,@Description)";
                command.Parameters.AddWithValue("@Id", obj.Id);
            }
            else
            {
                command.CommandText =
                "INSERT INTO TOURNAMENT " +
                "(START_TIME, STARTED_AT_TIME, FINISHED_AT_TIME, NAME, COURSE_TYPE, DESCRIPTION) " +
                "VALUES (@StartTime,@StartedAtTime,@FinishedAtTime,@Name,@CourseType,@Description)";
            }

            command.Parameters.AddWithValue("@StartTime", obj.StartTime);
            command.Parameters.AddWithValue("@StartedAtTime", obj.StartedAtTime);
            command.Parameters.AddWithValue("@FinishedAtTime", obj.FinishedAtTime);
            command.Parameters.AddWithValue("@Name", obj.Name);
            command.Parameters.AddWithValue("@CourseType", obj.CourseType);
            command.Parameters.AddWithValue("@Description", obj.Description);

            return ExecuteNonQuery(connection, command);
        }

        public override int update(Tournament obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE TOURNAMENT " +
            "SET START_TIME=@StartTime, STARTED_AT_TIME=@StartedAtTime, FINISHED_AT_TIME=@FinishedAtTime, " +
            "NAME=@Name, COURSE_TYPE=@CourseType, DESCRIPTION=@Description " +
            "WHERE ID=@Id";

            command.Parameters.AddWithValue("@Id", obj.Id);
            command.Parameters.AddWithValue("@StartTime", obj.StartTime);
            command.Parameters.AddWithValue("@StartedAtTime", obj.StartedAtTime);
            command.Parameters.AddWithValue("@FinishedAtTime", obj.FinishedAtTime);
            command.Parameters.AddWithValue("@Name", obj.Name);
            command.Parameters.AddWithValue("@CourseType", obj.CourseType);
            command.Parameters.AddWithValue("@Description", obj.Description);

            return ExecuteNonQuery(connection, command);
        }

        public override int delete(Tournament obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM TOURNAMENT " +
            "WHERE ID=@Id AND START_TIME=@StartTime AND  " +
            "STARTED_AT_TIME=@StartedAtTime AND FINISHED_AT_TIME=@FinishedAtTime AND " +
            "NAME=@Name AND COURSE_TYPE=@CourseType AND DESCRIPTION=@Description";

            command.Parameters.AddWithValue("@Id", obj.Id);
            command.Parameters.AddWithValue("@StartTime", obj.StartTime);
            command.Parameters.AddWithValue("@StartedAtTime", obj.StartedAtTime);
            command.Parameters.AddWithValue("@FinishedAtTime", obj.FinishedAtTime);
            command.Parameters.AddWithValue("@Name", obj.Name);
            command.Parameters.AddWithValue("@CourseType", obj.CourseType);
            command.Parameters.AddWithValue("@Description", obj.Description);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteById(Tournament obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM TOURNAMENT WHERE ID=@Id";

            command.Parameters.AddWithValue("@Id", obj.Id);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteById(long id)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM TOURNAMENT WHERE ID=@Id";

            command.Parameters.AddWithValue("@Id", id);

            return ExecuteNonQuery(connection, command);
        }

        protected override List<Tournament> Select(SQLiteCommand command)
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

            var tournaments = new List<Tournament>();

            foreach (DataTable dt in dataSet.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Tournament t = new Tournament();

                    t.Id = (long)dr.ItemArray[0];
                    t.StartTime = (DateTime)dr.ItemArray[1];
                    if (dr.ItemArray[2] != DBNull.Value)
                        t.StartedAtTime = (DateTime?)dr.ItemArray[2];
                    if (dr.ItemArray[3] != DBNull.Value)
                        t.FinishedAtTime = (DateTime?)dr.ItemArray[3];
                    t.Name = (string)dr.ItemArray[4];
                    t.CourseType = (CourseEnum)dr.ItemArray[5];
                    t.Description = (string)dr.ItemArray[6];

                    tournaments.Add(t);
                }
            }

            return tournaments;
        }
    }
}
