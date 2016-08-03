using System;
using System.Threading.Tasks;

namespace Kitty.Db
{
    public interface IBrowserCollector
    {
        Task<bool> CollectAsync();
    }
}
