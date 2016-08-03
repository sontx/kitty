using System.Threading.Tasks;

namespace Kitty.Types.Stash
{
    public interface ITextFactory : IStashProvider
    {
        ITextStash GenerateNew();
    }
}
