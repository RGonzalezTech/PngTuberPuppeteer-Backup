﻿using System;
using System.Linq;
using System.Runtime.InteropServices;

// SOURCE:
// https://github.com/baffler/Transparent-Twitch-Chat-Overlay/blob/master/TransparentTwitchChatWPF/Win32Utils.cs
namespace TransparentTwitchChatWPF
{
    public static class WindowHelper
    {
        // SOURCE for these two - https://blog.magnusmontin.net/2015/03/31/implementing-global-hot-keys-in-wpf/
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        [DllImport("user32.dll", SetLastError = true)]
        private static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_TRANSPARENT = 0x20;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;

        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            SetWindowLong(hwnd, GWL_EXSTYLE,
                (IntPtr)(GetWindowLong(hwnd, GWL_EXSTYLE) | WS_EX_LAYERED | WS_EX_TRANSPARENT));
        }

        public static void SetWindowExDefault(IntPtr hwnd)
        {
            SetWindowLong(hwnd, GWL_EXSTYLE,
                (IntPtr)(GetWindowLong(hwnd, GWL_EXSTYLE) & ~(WS_EX_LAYERED | WS_EX_TRANSPARENT)));
        }
    }
}