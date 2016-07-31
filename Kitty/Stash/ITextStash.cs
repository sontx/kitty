using System;

namespace Kitty.Stash
{
    internal interface ITextStash : IDisposable
    {
        bool IsEmpty { get; }

        bool IsFull { get; }

        void Write(string st);
    }
}
