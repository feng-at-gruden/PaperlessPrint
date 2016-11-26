using System;
using System.Windows;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Diagnostics;
using System.Windows.Interop;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Common.Utiles
{
    public static class WinHookerHelper
    {

        #region " Disable Special Keys"

        private delegate IntPtr LowLevelKeyboardProcDelegate(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProcDelegate callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);
        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //private static extern short GetAsyncKeyState(Keys key);


        const int WH_KEYBOARD_LL = 13;
        private static IntPtr intLLKey;
        //private static KBDLLHOOKSTRUCT lParam;


        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr extra;
        }

        public static void EnableSpecialKeyboardHook()
        {
            ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
            intLLKey = SetWindowsHookEx(WH_KEYBOARD_LL,
                new LowLevelKeyboardProcDelegate(LowLevelKeyboardProc),
                GetModuleHandle(objCurrentModule.ModuleName), 0);
        }

        public static void ReleaseSpecialKeyboardHook()
        {
            if (intLLKey != IntPtr.Zero)
            {
                UnhookWindowsHookEx(intLLKey);
                intLLKey = IntPtr.Zero;
            }
        }

        private static IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Disabling Windows keys
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                if (objKeyInfo.key == Keys.RWin ||                     
                    objKeyInfo.key == Keys.LWin ||
                    ((objKeyInfo.key == Keys.Tab) && (objKeyInfo.flags == 0x20)) ||         // Alt +Tab
                    ((objKeyInfo.key == Keys.Escape) && (objKeyInfo.flags == 0x20)) ||      // Alt+Esc
                    ((objKeyInfo.key == Keys.Escape) && (objKeyInfo.flags == 0x00)) ||      // Ctrl+Esc
                    ((objKeyInfo.key == Keys.F4) && (objKeyInfo.flags == 0x20)) ||          // Alt+F4
                    ((objKeyInfo.key == Keys.Space) && (objKeyInfo.flags == 0x20)))         // Alt+Space
                {   
                    return (IntPtr)1;   
                }
            }

            return CallNextHookEx(intLLKey, nCode, wParam, lParam);
        }


        /*code needed to disable start menu*/
        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;

        public static void HideTaskBar()  
        {
            int hwnd = FindWindow("Shell_TrayWnd", "");
            ShowWindow(hwnd, SW_HIDE);
        }

        public static void ShowTaskBar()  
        {
            int hwnd = FindWindow("Shell_TrayWnd", "");
            ShowWindow(hwnd, SW_SHOW);
        }


        public static void DisableTaskManager()  
        {
            RegistryKey regkey;
            string keyValueInt = "1";
            string subKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";

            try
            {
                regkey = Registry.CurrentUser.CreateSubKey(subKey);
                regkey.SetValue("DisableTaskMgr", keyValueInt);
                regkey.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        public static void EnableTaskManager()  
        {
            try
            {
                string subKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
                RegistryKey rk = Registry.CurrentUser;
                RegistryKey sk1 = rk.OpenSubKey(subKey);
                if (sk1 != null)
                    rk.DeleteSubKeyTree(subKey);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        #region HotKey

        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(
            [In] IntPtr hWnd,
            [In] int id,
            [In] uint fsModifiers,
            [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);

        private static HwndSource _source;
        private const int HOTKEY_ID = 9000;

        public static void RegisterHotKey(Window window)
        {
            var helper = new WindowInteropHelper(window);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            const uint VK_F10 = 0x79;
            const uint MOD_CTRL = 0x0002;
            const uint VK_TAB = 0x09;
            const uint MOD_ALT = 0x0001;
            if (!RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_ALT, VK_F10))
            {
                // handle error
            }
        }

        public static void UnregisterHotKey(Window window)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            var helper = new WindowInteropHelper(window);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
        }

        private static IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            OnHotKeyPressed();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private static void OnHotKeyPressed()
        {
            // do nothing
        }
        
        #endregion



    }

}
