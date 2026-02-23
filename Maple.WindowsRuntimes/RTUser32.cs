using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
namespace Maple.WindowsRuntimes
{
    using UINT_PTR = nuint;
    using HWND = nint;
    using unsafe TimerProc = delegate* unmanaged[Stdcall]<nint, uint, nuint, uint, void>;

    public static partial class RTUser32
    {
        //[UnmanagedFunctionPointer(CallingConvention.StdCall)]
        //public delegate void TimerProc(IntPtr hWnd, uint uMsg, UIntPtr nIDEvent, uint dwTime);

        const string LibraryUser32 = "User32.dll";

        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        [LibraryImport(LibraryUser32, EntryPoint = "SetTimer", SetLastError = false)]
        public static partial UINT_PTR SetTimer(HWND hWnd, UINT_PTR nIDEvent, uint uElapse, TimerProcWrapper lpTimerFunc);

        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        [return: MarshalAs(UnmanagedType.Bool)]
        [LibraryImport(LibraryUser32, EntryPoint = "KillTimer", SetLastError = false)]
        public static partial bool KillTimer(HWND hWnd, UINT_PTR nIDEvent);

        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        [LibraryImport(LibraryUser32, EntryPoint = "CallWindowProcW", SetLastError = false)]
        public static partial nint CallWindowProc(nint lpPrevWndFunc, HWND hWnd, EnumWindowMsgCode Msg, nint wParam, nint lParam);

        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        [LibraryImport(LibraryUser32, EntryPoint = "DefWindowProcW", SetLastError = false)]
        public static partial nint DefWindowProc(HWND hWnd, EnumWindowMsgCode Msg, nint wParam, nint lParam);

        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        [LibraryImport(LibraryUser32, EntryPoint = "SetWindowLongPtrW", SetLastError = false)]
        public static partial nint SetWindowLongPtr(HWND hWnd, EnumGWLP nIndex, nint dwNewLong);


        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        [LibraryImport(LibraryUser32, EntryPoint = "PostMessageW", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostMessage(HWND hWnd, EnumWindowMsgCode Msg, nint wParam, nint lParam);


        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct TimerProcWrapper(TimerProc v)
        {
            [MarshalAs(UnmanagedType.SysInt)]
            public TimerProc Value = v;
        }


    }

}
