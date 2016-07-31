namespace Kitty.Stash
{
    internal class TextStash : BaseStash
    {
        public override string ToString()
        {
            return string.Format("{0}.{1}", Created.ToString(Properties.Resources.datetime_format), "txt");
        }
    }
}
