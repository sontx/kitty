using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Kitty.Win32
{
    public static class ApplicationInformation
    {
        public static string GetVersion()
        {
            return Application.ProductVersion;
        }

        public static string GetLocation()
        {
            return Application.ExecutablePath;
        }

        public static string GetHowLong()
        {
            return (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString();
        }

        public static string GetInstalledTime()
        {
            try
            {
                using (RegistryKey hkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\" + Application.ProductName))
                {
                    if (hkey == null) return "";
                    string installed = hkey.GetValue("installed", string.Empty).ToString();
                    return string.IsNullOrEmpty(installed) ? DateTime.Now.ToString(Properties.Resources.datetime_format) : installed;
                }
            }
            catch { return DateTime.Now.ToString(Properties.Resources.datetime_format); }
        }

        public static void VerifyApplition()
        {
            CheckInstalledTime();
            CheckAutoRun();
        }

        private static void CheckInstalledTime()
        {
            try
            {
                using (RegistryKey hkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft", true))
                {
                    hkey.CreateSubKey(Application.ProductName);
                }
                using (RegistryKey hkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\" + Application.ProductName, true))
                {
                    if (string.IsNullOrEmpty(hkey.GetValue("installed", "").ToString()))
                    {
                        hkey.SetValue("installed", DateTime.Now.ToString(Properties.Resources.datetime_format), RegistryValueKind.String);
                    }
                }
            }
            catch { }
        }

        private static void CheckAutoRun()
        {
            try
            {
                using (RegistryKey hkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (string.IsNullOrEmpty(hkey.GetValue("kitty", "").ToString()))
                    {
                        hkey.SetValue("kitty", Application.ExecutablePath, RegistryValueKind.String);
                    }
                }
            }
            catch { }
        }
    }
}
