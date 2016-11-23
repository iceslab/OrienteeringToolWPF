using OrienteeringToolWPF.Enumerations;
using Simple.Data;
using System.ComponentModel;
using System.Data.SQLite;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Utils
{
    public static class DatabaseUtils
    {
        #region Properties
        public static bool IsDatabaseAccessible
        {
            get
            {
                return DatabaseType != DatabaseType.NONE;
            }
        }
        public static string DatabasePath { get; set; }
        private static DatabaseConnectionData _DatabaseConnectionData;

        public static DatabaseConnectionData DatabaseConnectionData
        {
            get { return _DatabaseConnectionData; }
            set
            {
                if (value != null)
                    _DatabaseConnectionData = value;
                else
                    _DatabaseConnectionData = new DatabaseConnectionData();
            }
        }
        private static DatabaseType _DatabaseType;
        public static DatabaseType DatabaseType
        {
            get { return _DatabaseType; }
            set
            {
                _DatabaseType = value;
                OnGlobalPropertyChanged(nameof(IsDatabaseAccessible));
            }
        }
        #endregion
        #region Notify GlobalPropertyChanged
        public static event PropertyChangedEventHandler GlobalPropertyChanged = delegate { };
        private static void OnGlobalPropertyChanged(string propertyName)
        {
            GlobalPropertyChanged(
                typeof(MenuItem),
                new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        static DatabaseUtils()
        {
            DatabasePath = null;
            DatabaseConnectionData = new DatabaseConnectionData();
            DatabaseType = DatabaseType.NONE;
        }

        ///<summary>Get SQLite connection to database</summary>
        ///<returns>SQLite connection made from DatabasePath</returns>
        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(
                ConnectionStringUtils.GetSqliteConnectionString(DatabasePath));
        }

        ///<summary>Get dynamic database object for SQLite3</summary>
        ///<returns>Dynamic database object from DatabasePath</returns>
        private static dynamic GetDatabaseSQLite3()
        {
            return Database.Opener.OpenConnection(
                ConnectionStringUtils.GetSqliteConnectionString(DatabasePath),
                Properties.Resources.ProviderNameSqlite);
        }

        ///<summary>Get dynamic database object for Mysql</summary>
        ///<returns>
        ///Dynamic database object from credentials stored in databaseConnectionData
        ///</returns>
        private static dynamic GetDatabaseMysql()
        {
            return Database.Opener.OpenConnection(
                ConnectionStringUtils.GetMySqlConnectionString(DatabaseConnectionData),
                Properties.Resources.ProviderNameMysql);
        }

        ///<summary>Get dynamic database object depending on DatabaseType</summary>
        ///<exception cref="InvalidEnumArgumentException">
        ///When DatabaseType other than SQLITE3 or MYSQL
        ///</exception>
        ///<returns>
        ///Dynamic database object
        ///</returns>
        public static dynamic GetDatabase()
        {
            switch (DatabaseType)
            {
                case DatabaseType.SQLITE3:
                    return GetDatabaseSQLite3();
                case DatabaseType.MYSQL:
                    return GetDatabaseMysql();
                case DatabaseType.NONE:
                default:
                    throw new InvalidEnumArgumentException(
                        "Variable "
                        + "\"" + nameof(DatabaseType) + "\""
                        + " does not provide valid database type. Provided type: "
                        + DatabaseType.ToString());
            }
        }
    }
}
