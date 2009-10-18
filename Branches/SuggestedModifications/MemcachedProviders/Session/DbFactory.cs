using System;
using MemcachedProviders.Session.Db;

namespace MemcachedProviders.Session
{
    public abstract class DbFactory
    {
        public static IDbOperations CreateDbOperations(DatabaseEngine databaseEngine, string connectionString)
        {
            switch (databaseEngine)
            {
                case DatabaseEngine.None:
                    return null;
                case DatabaseEngine.SQLServer:
                    return new SQLDbOperations(connectionString);
                case DatabaseEngine.MySQL:
                    return new MySQLDbOperations(connectionString);
                default:
                    string error = String.Format("Database engine type '{0}' is not yet supported.", databaseEngine);
                    throw new NotSupportedException(error);
            }
        }
    }
}