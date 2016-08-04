namespace Kitty.Types.Stash
{
    public class HistoryStash : BaseStash
    {
        public static string GetRawName(string browserName, long createdTime)
        {
            return string.Format("h.{0}.{1}", browserName, createdTime);
        }

        public override string ToString()
        {
            string browserName = Tag as string;
            return string.Format("[history.{0}]{1}.sqlite", browserName, Created.ToString(Conf.Properties.Resources.datetime_format));
        }
    }
}
