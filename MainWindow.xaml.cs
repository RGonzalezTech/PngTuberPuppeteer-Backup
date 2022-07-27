using Microsoft.Win32;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TransparentTwitchChatWPF;

namespace PngTuber.Pupper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HwndSource source;
        private IntPtr handle;

        private const int HotKeyBase = 0x9000;

        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = new RootViewModel();

            this.VM.Initialize();
            this.VM.WebViewPreview = this.previewWindow;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.source.RemoveHook(this.HwndHookMethod);

            for (var i = 0; i < 12; i++)
            {
                WindowHelper.UnregisterHotKey(
                    this.handle, HotKeyBase + i);
            }

            this.VM.Shutdown();
            base.OnClosing(e);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            this.handle = new WindowInteropHelper(this).Handle;
            this.source = HwndSource.FromHwnd(this.handle);
            this.source.AddHook(this.HwndHookMethod);

            for(uint i = 0; i < 12; i++)
            {
                // Set up a hook on Ctrl + Shift + F1-F12
                WindowHelper.RegisterHotKey(
                    this.handle, HotKeyBase + (int)i,
                    KeyConstants.MOD_CONTROL | KeyConstants.MOD_SHIFT | KeyConstants.MOD_NOREPEAT,
                    KeyConstants.VK_F1 + i);
            }
        }

        private IntPtr HwndHookMethod(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (!this.VM.ListenGlobalHotkeys)
            {
                return IntPtr.Zero;
            }

            if(msg != 0x0312)
            {
                return IntPtr.Zero;
            }

            var wParamInt = wParam.ToInt32();
            if(wParamInt < HotKeyBase || wParamInt >= HotKeyBase + 12)
            {
                return IntPtr.Zero;
            }

            var vKey = (uint)(((int)lParam >> 16) & 0xFFFF);

            if(vKey >= KeyConstants.VK_F1 && vKey <= KeyConstants.VK_F10)
            {
                int fKeyPressed = (int)(vKey - KeyConstants.VK_F1);

                this.VM.UpdateExpressionIndex(fKeyPressed);
            }
            else if(vKey == KeyConstants.VK_F11)
            {
                this.VM.UpdateExpressionIndex(-1, true);
            }
            else if (vKey == KeyConstants.VK_F12)
            {
                this.VM.UpdateExpressionIndex(1, true);
            }

            return IntPtr.Zero;
        }

        public RootViewModel VM => this.DataContext as RootViewModel;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.VM.SendSignalRDebugMessage();
        }
    }
}
