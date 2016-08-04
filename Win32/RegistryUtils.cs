using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Kitty.Win32
{
    public static class RegistryUtils
    {
        private static bool CheckProgramIsInstalled(RegistryKey hkey, string displayNamePattern)
        {
            displayNamePattern = displayNamePattern.ToUpper();
            var subkeys = hkey.GetSubKeyNames();
            foreach (var key in subkeys)
            {
                if (key.ToUpper().Contains(displayNamePattern))
                    return true;
                using (RegistryKey subHKey = hkey.OpenSubKey(key, false))
                {
                    string displayName = subHKey.GetValue("DisplayName", "").ToString().ToUpper();
                    if (displayName.Contains(displayNamePattern))
                        return true;
                }
            }
            return false;
        }

        private static bool CheckProgramIsInstalled(string uninstallKeyPath, string displayNamePattern)
        {
            try
            {
                using (RegistryKey hkey = Registry.LocalMachine.OpenSubKey(uninstallKeyPath, false))
                {
                    if (CheckProgramIsInstalled(hkey, displayNamePattern))
                        return true;
                }
                using (RegistryKey hkey = Registry.CurrentUser.OpenSubKey(uninstallKeyPath, false))
                {
                    return CheckProgramIsInstalled(hkey, displayNamePattern);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
            }
            return false;
        }

        public static bool CheckProgramIsInstalled(string displayNamePattern)
        {
            return CheckProgramIsInstalled("SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall", displayNamePattern) ||
                   CheckProgramIsInstalled("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall", displayNamePattern);
        }

        public static Task<bool> CheckProgramIsInstalledAsync(string displayNamePattern)
        {
            return Task.Run(() => { return CheckProgramIsInstalled(displayNamePattern); });
        }
    }
}
