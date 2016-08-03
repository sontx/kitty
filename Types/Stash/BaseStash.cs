using System;

namespace Kitty.Types.Stash
{
    public abstract class BaseStash
    {
        public object Tag { get; set; }
        public byte[] Content { get; set; }
        public DateTime Created { get; set; }

        public abstract override string ToString();
    }
}
