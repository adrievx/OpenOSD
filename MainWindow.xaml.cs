using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenOSD.Util;

namespace OpenOSD {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private bool _isActive = false;
        private CancellationTokenSource _fadeCts;

        public MainWindow() {
            InitializeComponent();
            doWindowSetup();

            SourceInitialized += (_, __) => hideWnd();
            App.KeyboardHook += HookCallback;
        }

        private void doWindowSetup() {
            this.Top = 32;
            this.Left = 32;
            this.Opacity = 0;
            this.ShowInTaskbar = false;
        }

        private void hideWnd() {
            var hwnd = new WindowInteropHelper(this).Handle;
            int exStyle = NativeFunctions.GetWindowLong(hwnd, NativeStatics.GWL_EXSTYLE);

            exStyle |= NativeStatics.WS_EX_TOOLWINDOW;
            exStyle &= ~NativeStatics.WS_EX_APPWINDOW;

            NativeFunctions.SetWindowLong(hwnd, NativeStatics.GWL_EXSTYLE, exStyle);
        }

        protected override void OnSourceInitialized(EventArgs e) {
            base.OnSourceInitialized(e);

            var source = PresentationSource.FromVisual(this) as HwndSource;
            source?.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            if (msg == NativeStatics.WM_SYSCOMMAND && ((int)wParam & 0xFFF0) == NativeStatics.SC_CLOSE) {
                handled = true; // swallow alt f4
                return IntPtr.Zero;
            }

            return IntPtr.Zero;
        }


        protected override void OnClosed(EventArgs e) {
            App.KeyboardHook -= HookCallback;
            base.OnClosed(e);
        }

        public IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
            if (nCode < 0 || wParam != (IntPtr)NativeStatics.WM_KEYDOWN) {
                return IntPtr.Zero;
            }

            int vkCode = Marshal.ReadInt32(lParam);
            if (!AppStatics.LockKeys.TryGetValue(vkCode, out var info)) {
                return IntPtr.Zero;
            }

            bool isOn = (NativeFunctions.GetKeyState(vkCode) & 1) != 0;
            showLockState(info.label, isOn, info.icon);

            return IntPtr.Zero;
        }

        private async void showLockState(string label, bool state, string imagePath) {
            lblMainText.Content = $"{label} {(!state ? "ON" : "OFF")}"; // not sure why it needs to be inverted
            imgIcon.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));

            _fadeCts?.Cancel();
            _fadeCts = new CancellationTokenSource();

            await showWithFadeAsync(_fadeCts.Token);
        }


        private Task fadeWindowAsync(double from, double to, double durationSeconds) {
            var tcs = new TaskCompletionSource<bool>();

            var animation = new DoubleAnimation {
                From = from,
                To = to,
                Duration = TimeSpan.FromSeconds(durationSeconds),
                FillBehavior = FillBehavior.Stop
            };

            animation.Completed += (s, e) => {
                this.Opacity = to;
                tcs.SetResult(true);
            };

            this.BeginAnimation(Window.OpacityProperty, animation);
            return tcs.Task;
        }

        private async Task showWithFadeAsync(CancellationToken token, double fadeInSeconds = 0.2, double visibleSeconds = 2.0, double fadeOutSeconds = 0.2) {
            try {
                Dispatcher.Invoke(() => {
                    Show();
                    BeginAnimation(Window.OpacityProperty, null);
                });

                _isActive = true;

                double currentOpacity = Opacity;
                await fadeWindowAsync(currentOpacity, 1, fadeInSeconds);

                await Task.Delay(TimeSpan.FromSeconds(visibleSeconds), token);

                await fadeWindowAsync(1, 0, fadeOutSeconds);

                _isActive = false;
                Hide();
            }
            catch (TaskCanceledException) {
                // expected when key is pressed again
            }
        }
    }
}