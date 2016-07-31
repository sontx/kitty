using System;

namespace Kitty.Stash
{
    internal abstract class BaseStash
    {
        public object Tag { get; set; }
        public byte[] Content { get; set; }
        public DateTime Created { get; set; }

        public abstract override string ToString();
    }
}
