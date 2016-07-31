using Microsoft.Win32;
using System;

namespace Kitty.Win32
{
    internal static class SystemInformation
    {
        public static string GetUserName()
        {
            return Environment.UserName;
        }

        public static string GetComputerName()
        {
            return Environment.MachineName;
        }

        public static string GetSystemArchitect()
        {
            return Environment.Is64BitOperatingSystem ? "64 Bits" : "32 Bits";
        }

        private static string GetRegistryString(string path, string key)
        {
            try
            {
                using (RegistryKey hkey = Registry.LocalMachine.OpenSubKey(path))
                {
                    if (hkey == null) return "";
                    return (string)hkey.GetValue(key);
                }
            }
            catch { return ""; }
        }

        public static string GetFriendlyOSName()
        {
            string ProductName = GetRegistryString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            string CSDVersion = GetRegistryString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion");
            if (ProductName != "")
            {
                return (ProductName.StartsWith("Microsoft") ? "" : "Microsoft ") + ProductName +
                            (CSDVersion != "" ? " " + CSDVersion : "");
            }
            return "";
        }
    }
}
