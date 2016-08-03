using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Kitty.Win32
{
    public static class Win32API
    {
        public delegate long HookProc(int ncode, UIntPtr wParam, IntPtr lParam);
        private const int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        private const int FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
        private const int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
        private const int WM_GETTEXTLENGTH = 0x000E;
        private const int WM_GETTEXT = 0x000D;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int CallNextHookEx(IntPtr hHook, int ncode, UIntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", SetLastError = true)]
        private static extern short GetKeyState(int nVirtKey);
        [DllImport("user32.dll", EntryPoint = "GetKeyNameTextW", CharSet = CharSet.Unicode)]
        private static extern int GetKeyNameText(int lParam, [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder str, int size);
        [DllImport("user32.dll", EntryPoint = "SetWindowsHookExA", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hmod, uint dwThreadId);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int UnhookWindowsHookEx(IntPtr hHook);
        [DllImport("kernel32.dll", EntryPoint = "FormatMessageA")]
        private static extern int FormatMessage(int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, ref IntPtr lpBuffer, int nSize, IntPtr Arguments);
        [DllImport("kernel32.dll")]
        private static extern int LocalFree(IntPtr hMem);
        [DllImport("user32.dll", EntryPoint = "GetWindowTextA", SetLastError = true)]
        private static extern int GetWindowText(int hwnd, string lpString, int cch);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wparam, int lparam);
        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        private static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

        public static int CallNextHookEx_(IntPtr hHook, int ncode, UIntPtr wParam, IntPtr lParam)
        {
            return CallNextHookEx(hHook, ncode, wParam, lParam);
        }

        public static IntPtr GetForegroundWindow_()
        {
            return GetForegroundWindow();
        }

        public static short GetKeyState_(int nVirtKey)
        {
            return GetKeyState(nVirtKey);
        }

        public static string GetKeyNameText(int lParam)
        {
            StringBuilder builder = new StringBuilder(16);
            GetKeyNameText((int)lParam, builder, builder.Capacity);
            return builder.ToString();
        }

        public static IntPtr SetWindowsHookEx_(int idHook, HookProc lpfn, IntPtr hmod, uint dwThreadId)
        {
            return SetWindowsHookEx(idHook, lpfn, hmod, dwThreadId);
        }

        public static int UnhookWindowsHookEx_(IntPtr hHook)
        {
            return UnhookWindowsHookEx(hHook);
        }

        public static string GetWindowText(IntPtr hwnd)
        {
            int length = SendMessage(hwnd, WM_GETTEXTLENGTH, 0, 0).ToInt32();
            if (length > 0)
            {
                StringBuilder builder = new StringBuilder(length + 1);
                SendMessage(hwnd, WM_GETTEXT, builder.Capacity, builder);
                return builder.ToString();
            }
            return null;
        }

        public static int GetLastError_()
        {
            return Marshal.GetLastWin32Error();
        }

        public static string GetLastErrorAsMessage()
        {
            int errorCode = GetLastError_();
            IntPtr lpMsgBuf = IntPtr.Zero;
            int dwChars = FormatMessage(
                FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
                IntPtr.Zero,
                errorCode,
                0, // Default language
                ref lpMsgBuf,
                0,
                IntPtr.Zero);
            if (dwChars == 0)
            {
                GetLastError_();
                return null;
            }

            string sRet = Marshal.PtrToStringAnsi(lpMsgBuf);
            LocalFree(lpMsgBuf);
            return sRet;
        }
    }
}
