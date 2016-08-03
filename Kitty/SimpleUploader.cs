using Kitty.Net;
using Kitty.Resources;
using Kitty.Types.Stash;
using System.Threading.Tasks;

namespace Kitty
{
    internal sealed class SimpleUploader
    {
        private IStashProvider stashProvider;

        public SimpleUploader(IStashProvider stashProvider)
        {
            this.stashProvider = stashProvider;
        }

        public Task UploadAsync()
        {
            return Task.Run(async () => 
            {
                using (var serverProvider = new SmtpEmailProvider())
                {
                    serverProvider.MailProviders.Add(new MainMailProvider());
                    serverProvider.MailProviders.Add(new SubMailProvider());
                    var uploadManager = new UploadManager(stashProvider, serverProvider);
                    var result = await uploadManager.UploadAsync();
                }
            });
        }
    }
}
