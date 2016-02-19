using OrienteeringToolWPF.DAO.Base;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Windows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace OrienteeringToolWPF.DAO.Implementation
{
    public class RouteStepDAO : BaseRouteStepDAO
    {
        public override List<RouteStep> findAll()
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM ROUTE_STEPS";
            return Select(command);
        }

        public override List<RouteStep> findAllByCode(long code)
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM ROUTE_STEPS WHERE CODE=@code";
            command.Parameters.AddWithValue("@code", code);
            return Select(command);
        }

        public override List<RouteStep> findAllById(long id)
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM ROUTE_STEPS WHERE ID=@id";
            command.Parameters.AddWithValue("@id", id);
            return Select(command);
        }

        public override List<RouteStep> findAllByRouteID(long routeId)
        {
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM ROUTE_STEPS WHERE ROUTE_ID=@routeId";
            command.Parameters.AddWithValue("@routeId", routeId);
            return Select(command);
        }

        public override int insert(RouteStep obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            if (obj.Id != null)
            {
                command.CommandText =
                    "INSERT INTO ROUTE_STEPS (ID, CODE, ROUTE_ID) " +
                    "VALUES (@Id,@Code,@RouteId)";
                command.Parameters.AddWithValue("@Id", obj.Id);
            }
            else
            {
                command.CommandText =
                    "INSERT INTO ROUTE_STEPS (CODE, ROUTE_ID) " +
                    "VALUES (@Code,@RouteId)";
            }

            command.Parameters.AddWithValue("@Code", obj.Code);
            command.Parameters.AddWithValue("@RouteId", obj.RouteId);

            return ExecuteNonQuery(connection, command);
        }

        public override int update(RouteStep obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "UPDATE ROUTE_STEPS " +
                "SET CODE=@Code, ROUTE_ID=@RouteId " +
                "WHERE ID=@Id";

            command.Parameters.AddWithValue("@Id", obj.Id);
            command.Parameters.AddWithValue("@Code", obj.Code);
            command.Parameters.AddWithValue("@RouteId", obj.RouteId);

            return ExecuteNonQuery(connection, command);
        }

        public override int delete(RouteStep obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM ROUTE_STEPS " +
                "WHERE ID=@Id AND CODE=@Code AND ROUTE_ID=@RouteId";

            command.Parameters.AddWithValue("@Id", obj.Id);
            command.Parameters.AddWithValue("@Code", obj.Code);
            command.Parameters.AddWithValue("@RouteId", obj.RouteId);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteByCode(long code)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM ROUTE_STEPS WHERE CODE=@code";

            command.Parameters.AddWithValue("@code", code);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteByCode(RouteStep obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM ROUTE_STEPS WHERE CODE=@Code";

            command.Parameters.AddWithValue("@Code", obj.Code);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteById(long id)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM ROUTE_STEPS WHERE ID=@Id";

            command.Parameters.AddWithValue("@Id", id);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteById(RouteStep obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM ROUTE_STEPS WHERE ID=@Id";

            command.Parameters.AddWithValue("@Id", obj.Id);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteByRouteId(long routeId)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM ROUTE_STEPS WHERE ROUTE_ID=@routeId";

            command.Parameters.AddWithValue("@routeId", routeId);

            return ExecuteNonQuery(connection, command);
        }

        public override int deleteByRouteId(RouteStep obj)
        {
            var connection = MainWindow.GetConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM ROUTE_STEPS WHERE ROUTE_ID=@RouteId";

            command.Parameters.AddWithValue("@RouteId", obj.RouteId);

            return ExecuteNonQuery(connection, command);
        }

        protected override List<RouteStep> Select(SQLiteCommand command)
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

            var routeSteps = new List<RouteStep>();

            foreach (DataTable dt in dataSet.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var r = new RouteStep();

                    r.Id = (long)dr.ItemArray[0];
                    r.Code = (long)dr.ItemArray[1];
                    r.RouteId = (long)dr.ItemArray[2];

                    routeSteps.Add(r);
                }
            }

            return routeSteps;
        }

    }
}
