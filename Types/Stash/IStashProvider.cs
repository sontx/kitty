using System.Threading.Tasks;

namespace Kitty.Types.Stash
{
    public interface IStashProvider
    {
        Task<BaseStash[]> GetStashsAsync();
        void DeleteStash(BaseStash stash);
    }
}
