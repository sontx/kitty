using Kitty.Net;
using Kitty.Resources;
using Kitty.Stash;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kitty
{
    internal sealed class ApplicationMaintain
    {
        public Task MaintainAsync()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            return Task.Run(() => 
            {
                if (CheckUpdate())
                    UpdateApplication();
            });
        }

        private void UpdateApplication()
        {
            if (DownloadUpdatePack())
            {
                if (InstallUpdatePack())
                    Application.Restart();
            }
        }

        private bool InstallUpdatePack()
        {
            throw new NotImplementedException();
        }

        private bool DownloadUpdatePack()
        {
            throw new NotImplementedException();
        }

        private bool CheckUpdate()
        {
            return false;
        }

        private void UploadErrorLog(Exception exception)
        {
            var factory = new StringFactory();
            using (var stringStash = factory.GenerateNew())
            {
                stringStash.Write(exception.Message);
                stringStash.Write(Environment.NewLine);
                stringStash.Write(exception.StackTrace);
#if DEBUG
                string message = exception.Message + Environment.NewLine + exception.StackTrace;
                MessageBox.Show(message, Application.ProductName);
#endif
            }
            using (var serverProvider = new SmtpEmailProvider())
            {
                serverProvider.MailProviders.Add(new MaintainMailProvider());
                var uploadManager = new UploadManager(factory, serverProvider);
                Task.Run(async () => { var result = await uploadManager.UploadAsync(); });
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;
            if (exception != null)
                UploadErrorLog(exception);
            Application.Restart();
        }
    }
}
