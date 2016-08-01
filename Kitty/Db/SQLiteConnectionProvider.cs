using System;
using System.Data.Common;
using System.Data.SQLite;

namespace Kitty.Db
{
    internal sealed class SQLiteConnectionProvider : IConnectionProvider
    {
        private SQLiteConnection connection;

        public DbConnection Connection { get { return connection; } }
        
        public SQLiteConnectionProvider(string databasePath)
        {
            connection = new SQLiteConnection(string.Format("Data Source={0}", databasePath));
        }

        public void Dispose()
        {
            connection?.Dispose();
            connection = null;
        }
    }
}
