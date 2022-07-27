using System;
using System.Linq;

// SOURCE:
// https://github.com/baffler/Transparent-Twitch-Chat-Overlay/blob/master/TransparentTwitchChatWPF/Win32Utils.cs
namespace TransparentTwitchChatWPF
{
    public static class KeyConstants
    {
        //Modifiers:
        public const uint MOD_NONE = 0x0000; //[NONE]
        public const uint MOD_ALT = 0x0001; //ALT
        public const uint MOD_CONTROL = 0x0002; //CTRL
        public const uint MOD_SHIFT = 0x0004; //SHIFT
        public const uint MOD_WIN = 0x0008; //WINDOWS
        public const uint MOD_NOREPEAT = 0x4000;
        //CAPS LOCK:
        public const uint VK_CAPITAL = 0x14;

        public const uint VK_F1 = 0x70;
        public const uint VK_F2 = 0x71;
        public const uint VK_F3 = 0x72;
        public const uint VK_F4 = 0x73;
        public const uint VK_F5 = 0x74;
        public const uint VK_F6 = 0x75;
        public const uint VK_F7 = 0x76;
        public const uint VK_F8 = 0x77;
        public const uint VK_F9 = 0x78;
        public const uint VK_F10 = 0x79;
        public const uint VK_F11 = 0x7A;
        public const uint VK_F12 = 0x7B;
    }
}