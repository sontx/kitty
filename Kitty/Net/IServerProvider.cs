using Kitty.Stash;
using System;
using System.Threading.Tasks;

namespace Kitty.Net
{
    internal interface IServerProvider : IDisposable
    {
        Task<bool> PrepareAsync();
        Task<bool> SendAsync(BaseStash[] stashs);
    }
}
