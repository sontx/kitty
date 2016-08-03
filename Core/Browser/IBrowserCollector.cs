using System.Threading.Tasks;

namespace Kitty.Core.Browser
{
    public interface IBrowserCollector
    {
        Task<bool> CollectAsync();
    }
}
