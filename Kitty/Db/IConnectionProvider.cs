using System.Data.Common;

namespace Kitty.Db
{
    internal interface IConnectionProvider
    {
        DbConnection CreateConnection();
    }
}
