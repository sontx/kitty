using System.Threading.Tasks;

namespace Kitty.Stash
{
    internal interface IStashProvider
    {
        Task<BaseStash[]> GetStashsAsync();
        void DeleteStash(BaseStash stash);
    }
}
