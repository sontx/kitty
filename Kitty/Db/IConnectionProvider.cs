using System;
using System.Data.Common;

namespace Kitty.Db
{
    internal interface IConnectionProvider : IDisposable
    {
        DbConnection Connection { get; }
    }
}
