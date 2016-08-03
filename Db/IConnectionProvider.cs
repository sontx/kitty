using System.Data.Common;

namespace Kitty.Db
{
    public interface IConnectionProvider
    {
        DbConnection CreateConnection();
    }
}
