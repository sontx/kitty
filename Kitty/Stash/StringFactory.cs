using System;
using System.Text;
using System.Threading.Tasks;

namespace Kitty.Stash
{
    internal sealed class StringFactory : ITextFactory
    {
        private ITextStash currentStash = null;

        public void DeleteStash(BaseStash stash)
        {
        }

        public ITextStash GenerateNew()
        {
            if (currentStash == null)
                currentStash = new StringStash();
            return currentStash;
        }

        public Task<BaseStash[]> GetStashsAsync()
        {
            return Task.Run(() =>
            {
                var textStash = new TextStash()
                {
                    Content = Encoding.UTF8.GetBytes(currentStash.ToString()),
                    Created = (currentStash as StringStash).Created,
                    Tag = null
                };
                return new BaseStash[] { textStash };
            });
        }

        private class StringStash : ITextStash
        {
            private StringBuilder builder;

            public DateTime Created { get; private set; }

            public bool IsEmpty { get; private set; } = true;

            public bool IsFull { get; private set; } = false;

            public void Dispose()
            {
            }

            public void Write(string st)
            {
                builder.Append(st);
                IsEmpty = false;
            }

            public StringStash()
            {
                builder = new StringBuilder();
                Created = DateTime.Now;
            }

            public override string ToString()
            {
                return builder.ToString();
            }
        }
    }
}
