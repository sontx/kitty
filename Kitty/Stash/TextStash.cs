﻿namespace Kitty.Stash
{
    internal class TextStash : BaseStash
    {
        public static string GetRawName(string contentType, long createdTime)
        {
            return string.Format("t.{0}.{1}", contentType, createdTime);
        }

        public override string ToString()
        {
            return string.Format("[text.keylog]{0}.{1}", Created.ToString(Properties.Resources.datetime_format), "txt");
        }
    }
}
