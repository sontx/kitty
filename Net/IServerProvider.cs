using Kitty.Types.Stash;
using System;
using System.Threading.Tasks;

namespace Kitty.Net
{
    public interface IServerProvider : IDisposable
    {
        Task<bool> PrepareAsync();
        Task<bool> SendAsync(BaseStash[] stashs);
    }
}
