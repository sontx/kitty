using System;

namespace Kitty.Types.Stash
{
    public interface ITextStash : IDisposable
    {
        bool IsEmpty { get; }

        bool IsFull { get; }

        void Write(string st);
    }
}
