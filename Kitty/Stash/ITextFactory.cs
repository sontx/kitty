namespace Kitty.Stash
{
    internal interface ITextFactory : IStashProvider
    {
        ITextStash GenerateNew();
    }
}