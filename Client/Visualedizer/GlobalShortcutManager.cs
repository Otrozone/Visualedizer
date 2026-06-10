using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Ledqualizer
{
    internal sealed class GlobalShortcutEventArgs : EventArgs
    {
        public GlobalShortcutEventArgs(KeyboardShortcutConfig shortcut, bool isRepeat)
        {
            Shortcut = shortcut;
            IsRepeat = isRepeat;
        }

        public KeyboardShortcutConfig Shortcut { get; }
        public bool IsRepeat { get; }
    }

    internal sealed class GlobalShortcutManager : IDisposable
    {
        private const int WhKeyboardLl = 13;
        private const int WmKeyDown = 0x0100;
        private const int WmKeyUp = 0x0101;
        private const int WmSysKeyDown = 0x0104;
        private const int WmSysKeyUp = 0x0105;
        private const int KeyPressedMask = 0x8000;

        private readonly LowLevelKeyboardProc hookCallback;
        private readonly HashSet<string> pressedShortcuts = new(StringComparer.Ordinal);
        private IntPtr hookId;
        private bool disposed;

        public GlobalShortcutManager()
        {
            hookCallback = HookCallback;
            hookId = SetHook(hookCallback);
            if (hookId == IntPtr.Zero)
            {
                throw new InvalidOperationException("Unable to install the global keyboard hook.");
            }
        }

        public event EventHandler<GlobalShortcutEventArgs>? ShortcutKeyDown;
        public event EventHandler<GlobalShortcutEventArgs>? ShortcutKeyUp;

        public bool IsShortcutPressed(KeyboardShortcutConfig shortcut)
        {
            if (shortcut.IsEmpty || !shortcut.IsUsable)
            {
                return false;
            }

            return IsKeyPressed(shortcut.Key)
                && shortcut.Control == IsAnyKeyPressed(Keys.ControlKey, Keys.LControlKey, Keys.RControlKey)
                && shortcut.Shift == IsAnyKeyPressed(Keys.ShiftKey, Keys.LShiftKey, Keys.RShiftKey)
                && shortcut.Alt == IsAnyKeyPressed(Keys.Menu, Keys.LMenu, Keys.RMenu);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            if (hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookId);
                hookId = IntPtr.Zero;
            }
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using Process currentProcess = Process.GetCurrentProcess();
            using ProcessModule? currentModule = currentProcess.MainModule;
            IntPtr moduleHandle = currentModule == null
                ? IntPtr.Zero
                : GetModuleHandle(currentModule.ModuleName);
            return SetWindowsHookEx(WhKeyboardLl, proc, moduleHandle, 0);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int message = wParam.ToInt32();
                bool isKeyDown = message is WmKeyDown or WmSysKeyDown;
                bool isKeyUp = message is WmKeyUp or WmSysKeyUp;
                if (isKeyDown || isKeyUp)
                {
                    var hookData = Marshal.PtrToStructure<KbdLlHookStruct>(lParam);
                    Keys key = (Keys)hookData.VkCode;
                    if (!KeyboardShortcutConfig.IsModifierKey(key))
                    {
                        KeyboardShortcutConfig shortcut = BuildShortcut(key);
                        if (shortcut.IsUsable)
                        {
                            string signature = shortcut.GetSignature();
                            if (isKeyDown)
                            {
                                bool repeat = !pressedShortcuts.Add(signature);
                                ShortcutKeyDown?.Invoke(this, new GlobalShortcutEventArgs(shortcut, repeat));
                            }
                            else
                            {
                                pressedShortcuts.Remove(signature);
                                ShortcutKeyUp?.Invoke(this, new GlobalShortcutEventArgs(shortcut, false));
                            }
                        }
                    }
                    else if (isKeyUp)
                    {
                        ShortcutKeyUp?.Invoke(this, new GlobalShortcutEventArgs(KeyboardShortcutConfig.Empty(), false));
                    }
                }
            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        private static KeyboardShortcutConfig BuildShortcut(Keys key)
        {
            return new KeyboardShortcutConfig
            {
                Control = IsAnyKeyPressed(Keys.ControlKey, Keys.LControlKey, Keys.RControlKey),
                Shift = IsAnyKeyPressed(Keys.ShiftKey, Keys.LShiftKey, Keys.RShiftKey),
                Alt = IsAnyKeyPressed(Keys.Menu, Keys.LMenu, Keys.RMenu),
                Key = key
            };
        }

        private static bool IsAnyKeyPressed(params Keys[] keys)
        {
            return keys.Any(IsKeyPressed);
        }

        private static bool IsKeyPressed(Keys key)
        {
            return (GetAsyncKeyState((int)key) & KeyPressedMask) != 0;
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct KbdLlHookStruct
        {
            public uint VkCode;
            public uint ScanCode;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
    }
}
