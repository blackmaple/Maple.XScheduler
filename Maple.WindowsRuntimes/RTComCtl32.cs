using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace Maple.WindowsRuntimes
{
    using UINT_PTR = nuint;
    using HWND = nint;
    using DWORD_PTR = nuint;
    using unsafe SubclassProc = delegate* unmanaged[Stdcall]<nint, EnumWindowMsgCode, nint, nint, nuint, nuint, nint>;
    using WPARAM = nint;
    using LPARAM = nint;
    using LRESULT = nint;
    public static partial class RTComCtl32
    {

        const string LibraryComCtl32 = "ComCtl32.dll";

        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        [LibraryImport(LibraryComCtl32, EntryPoint = "SetWindowSubclass", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetWindowSubclass(HWND hWnd, SubclassProcWrapper pfnSubclass, UINT_PTR uIdSubclass, DWORD_PTR dwRefData);

        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        [LibraryImport(LibraryComCtl32, EntryPoint = "RemoveWindowSubclass", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool RemoveWindowSubclass(HWND hWnd, SubclassProcWrapper pfnSubclass, UINT_PTR uIdSubclass);

        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        [LibraryImport(LibraryComCtl32, EntryPoint = "DefSubclassProc", SetLastError = false)]
        public static partial LRESULT DefSubclassProc(HWND hWnd, EnumWindowMsgCode msg, WPARAM wParam, LPARAM lParam);

        /// <summary>
        /// delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nuint, nuint, nint>
        /// </summary>
        /// <param name="v"></param>
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct SubclassProcWrapper(SubclassProc v)
        {
           // [MarshalAs(UnmanagedType.SysInt)]
            public SubclassProc Value = v;
        }
    }
}
