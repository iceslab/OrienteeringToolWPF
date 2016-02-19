using OrienteeringToolWPF.DAO.Base;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Windows;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace OrienteeringToolWPF.DAO.Implementation
{
    public class ResultDAO : BaseResultDAO
    {
        public override List<Result> findAll()
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM RESULTS";
            return Select(command);
        }

        public override List<Result> findAllByChip(long chip)
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM RESULTS WHERE CHIP=@chip";
            command.Parameters.AddWithValue("@chip", chip);
            return Select(command);
        }

        public override int insert(Result obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                        "INSERT INTO RESULTS (CHIP, START_TIME, CHECK_TIME, FINISH_TIME) " +
                        "VALUES (@Chip,@StartTime,@CheckTime,@FinishTime)";

            command.Parameters.AddWithValue("@Chip", obj.Chip);
            command.Parameters.AddWithValue("@StartTime", obj.StartTime);
            command.Parameters.AddWithValue("@CheckTime", obj.CheckTime);
            command.Parameters.AddWithValue("@FinishTime", obj.FinishTime);

            return ExecuteNonQuery(connection, command);
        }

        public override int update(Result obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "UPDATE RESULTS " +
                "SET START_TIME=@StartTime, CHECK_TIME=@CheckTime, FINISH_TIME=@FinishTime " +
                "WHERE CHIP=@Chip";

            command.Parameters.AddWithValue("@Chip", obj.Chip);
            command.Parameters.AddWithValue("@StartTime", obj.StartTime);
            command.Parameters.AddWithValue("@CheckTime", obj.CheckTime);
            command.Parameters.AddWithValue("@FinishTime", obj.FinishTime);

            return ExecuteNonQuery(connection, command);
        }

        public override int delete(Result obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM RESULTS " +
                "WHERE CHIP=@Chip AND START_TIME=@StartTime AND CHECK_TIME=@CheckTime AND FINISH_TIME=@FinishTime";

            command.Parameters.AddWithValue("@Chip", obj.Chip);
            command.Parameters.AddWithValue("@StartTime", obj.StartTime);
            command.Parameters.AddWithValue("@CheckTime", obj.CheckTime);
            command.Parameters.AddWithValue("@FinishTime", obj.FinishTime);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteByChip(Result obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM COMPETITORS WHERE CHIP=@Chip";

            command.Parameters.AddWithValue("@Chip", obj.Chip);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteByChip(long chip)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM RESULTS WHERE CHIP=@Chip";

            command.Parameters.AddWithValue("@Chip", chip);

            return ExecuteNonQuery(connection, command);
        }

        protected override List<Result> Select(SQLiteCommand command)
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

            var competitors = new List<Result>();

            foreach (DataTable dt in dataSet.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var r = new Result();

                    r.Chip = (long)dr.ItemArray[0];
                    r.StartTime = (long)dr.ItemArray[1];
                    r.CheckTime = (long)dr.ItemArray[2];
                    r.FinishTime = (long)dr.ItemArray[3];

                    competitors.Add(r);
                }
            }

            return competitors;
        }
    }
}
