using System.Data.Common;
using System.Data.SQLite;

namespace Kitty.Db
{
    public sealed class SQLiteConnectionProvider : IConnectionProvider
    {
        private readonly string databasePath;

        public DbConnection CreateConnection()
        {
            return new SQLiteConnection(string.Format("Data Source={0}", databasePath));
        }
        
        public SQLiteConnectionProvider(string databasePath)
        {
            this.databasePath = databasePath;
        }
    }
}
