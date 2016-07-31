using System.Text;

namespace Kitty.Resources
{
    internal abstract class MailProvider
    {
        public abstract string FromAddress { get; }
        public abstract string FromPassword { get; }
        public abstract string ToAddress { get; }

        protected static string GetStringFromBytes(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
