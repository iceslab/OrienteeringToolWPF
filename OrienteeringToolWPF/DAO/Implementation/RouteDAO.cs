using OrienteeringToolWPF.DAO.Base;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Windows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace OrienteeringToolWPF.DAO.Implementation
{
    public class RouteDAO : BaseRouteDAO
    {
        public override List<Route> findAll()
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM ROUTES";
            return Select(command);
        }

        public override List<Route> findAllById(long id)
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM ROUTES WHERE ID=@id";
            command.Parameters.AddWithValue("@id", id);
            return Select(command);
        }

        public override List<Route> findAllByName(string name)
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM ROUTES WHERE NAME=@name";
            command.Parameters.AddWithValue("@name", name);
            return Select(command);
        }

        public override int insert(Route obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            if (obj.Id != null)
            {
                command.CommandText =
                    "INSERT INTO ROUTES (ID, NAME) " +
                    "VALUES (@Id,@Name)";
                command.Parameters.AddWithValue("@Id", obj.Id);
            }
            else
            {
                command.CommandText =
                    "INSERT INTO ROUTES (NAME) " +
                    "VALUES (@Name)";
            }

            command.Parameters.AddWithValue("@Name", obj.Name);

            return ExecuteNonQuery(connection, command);
        }

        public override int update(Route obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "UPDATE ROUTES SET NAME=@Name WHERE ID=@Id";

            command.Parameters.AddWithValue("@Id", obj.Id);
            command.Parameters.AddWithValue("@Name", obj.Name);

            return ExecuteNonQuery(connection, command);
        }

        public override int delete(Route obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM ROUTES WHERE ID=@Id AND NAME=@Name";

            command.Parameters.AddWithValue("@Id", obj.Id);
            command.Parameters.AddWithValue("@Name", obj.Name);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteById(long id)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM ROUTES WHERE ID=@Id";

            command.Parameters.AddWithValue("@Id", id);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteById(Route obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM ROUTES WHERE ID=@Id";

            command.Parameters.AddWithValue("@Id", obj.Id);

            return ExecuteNonQuery(connection, command);
        }

        protected override List<Route> Select(SQLiteCommand command)
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

            var route = new List<Route>();

            foreach (DataTable dt in dataSet.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var r = new Route();

                    r.Id = (long)dr.ItemArray[0];
                    r.Name = (string)dr.ItemArray[1];

                    route.Add(r);
                }
            }

            return route;
        }

    }
}
