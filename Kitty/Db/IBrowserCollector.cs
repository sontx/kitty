using System;
using System.Threading.Tasks;

namespace Kitty.Db
{
    internal interface IBrowserCollector : IDisposable
    {
        Task<bool> CollectAsync();
    }
}
