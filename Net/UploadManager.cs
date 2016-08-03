using Kitty.Types.Stash;
using System.Threading.Tasks;

namespace Kitty.Net
{
    public sealed class UploadManager
    {
        private IStashProvider stashProvider;
        private IServerProvider serverProvider;

        public UploadManager(IStashProvider stashProvider, IServerProvider serverProvider)
        {
            this.stashProvider = stashProvider;
            this.serverProvider = serverProvider;
        }

        private void ClearStashs(BaseStash[] stashs)
        {
            foreach (var stash in stashs)
            {
                stashProvider.DeleteStash(stash);
            }
        }

        public Task<bool> UploadAsync()
        {
            return Task.Run(async () => 
            {
                bool serverReady = await serverProvider.PrepareAsync();
                if (serverReady)
                {
                    var stashs = await stashProvider.GetStashsAsync();
                    if (stashs != null && stashs.Length > 0)
                    {
                        bool uploadSussess = await serverProvider.SendAsync(stashs);
                        if (uploadSussess)
                            ClearStashs(stashs);
                        return uploadSussess;
                    }
                }
                return false;
            });
        }
    }
}
