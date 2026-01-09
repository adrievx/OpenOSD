using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using OpenOSD.Util;

namespace OpenOSD {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public static IntPtr hookId = IntPtr.Zero;

        public static event Func<int, IntPtr, IntPtr, IntPtr> KeyboardHook;

        private static NativeFunctions.LowLevelKeyboardProc _proc = HookCallback;

        protected override void OnStartup(StartupEventArgs e) {
            hookId = SetHook(_proc);
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e) {
            NativeFunctions.UnhookWindowsHookEx(hookId);
            base.OnExit(e);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
            if (KeyboardHook != null) {
                foreach (Func<int, IntPtr, IntPtr, IntPtr> handler in KeyboardHook.GetInvocationList()) {
                    handler(nCode, wParam, lParam);
                }
            }

            return NativeFunctions.CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        private static IntPtr SetHook(NativeFunctions.LowLevelKeyboardProc proc) {
            using (var curProcess = Process.GetCurrentProcess()) {
                var curModule = curProcess.MainModule;
                return NativeFunctions.SetWindowsHookEx(NativeStatics.WH_KEYBOARD_LL, proc, NativeFunctions.GetModuleHandle(curModule.ModuleName), 0);
            }
        }
    }
}
