using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Maple.Hook.WinMsg
{
    public static partial class WindowLongNativeMethods
    {
        [LibraryImport("user32.dll", EntryPoint = "GetWindowLongW")]
        private static partial int GetWindowLong32(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex);

        [LibraryImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
        private static partial nint GetWindowLongPtr64(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex);

        [LibraryImport("user32.dll", EntryPoint = "SetWindowLongW")]
        private static partial int SetWindowLong32(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex, int dwNewLong);

        [LibraryImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
        private static partial nint SetWindowLongPtr64(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex, nint dwNewLong);

        internal static nint GetWindowLongPtr(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex)
            => IntPtr.Size == 8 ? GetWindowLongPtr64(hWnd, nIndex) : GetWindowLong32(hWnd, nIndex);

        internal static nint SetWindowLongPtr(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex, nint dwNewLong)
            => IntPtr.Size == 8 ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong) : SetWindowLong32(hWnd, nIndex, (int)dwNewLong);

        internal static bool SetHook(HWND hWnd, nint newProc, out nint oldProc)
        {
            oldProc = SetWindowLongPtr(hWnd, WINDOW_LONG_PTR_INDEX.GWLP_WNDPROC, newProc);
            return oldProc != nint.Zero;
        }
        internal static bool RemoveHook(HWND hWnd, nint oldProc)
        {
            return SetWindowLongPtr(hWnd, WINDOW_LONG_PTR_INDEX.GWLP_WNDPROC, oldProc) != nint.Zero;
        }

        internal static void SetPrivateData(HWND hWnd, nint data)
        {
            SetWindowLongPtr(hWnd, WINDOW_LONG_PTR_INDEX.GWLP_USERDATA, data);
        }
        internal static nint GetPrivateData(HWND hWnd)
        {
            return GetWindowLongPtr(hWnd, WINDOW_LONG_PTR_INDEX.GWLP_USERDATA);
        }

        //[System.Diagnostics.Conditional("DEBUG")]
        //public static void GetCurrentThread(string name)
        //{ 
        //    var id= PInvoke.GetCurrentThreadId();
        //    Debug.WriteLine($"[{name} Thread Id]:{id}");
        //}
    }
}
