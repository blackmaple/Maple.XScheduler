using Maple.Hook.WinMsg;
using Maple.WindowsRuntimes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maple.XScheduler.WinMsg
{
    using unsafe ExecUnmanagedCodeProc = delegate* unmanaged[Stdcall]<nint, void>;

    public class WinMsgUserImp : IXSchedulerUnmanaged
    {
        public nint MainWindowHandle { get; }
        WinMsgHookItem HookItem { get; }

        public WinMsgUserImp(nint hWnd, WinMsgHookItem hookItem)
        {

            this.MainWindowHandle = hWnd;
            this.HookItem = hookItem;
            this.HookItem.SyncCallback += OnSyncCallback;
            this.HookItem.EnabledSyncCallback = true;
            this.HookItem.Start();
        }

        public ValueTask<bool> ExecAsync(XSchedulerTaskClosure taskClosure)
        {
            var b = PostMessage(this.MainWindowHandle, taskClosure.Handle);
            return new ValueTask<bool>(b);
        }

        unsafe static bool PostMessage(nint hwnd, nint userData)
        {
            ExecUnmanagedCodeProc procPtr = &UserExecCodeProc;
            return RTUser32.PostMessage(hwnd, EnumWindowMsgCode.USER_EXEC_CODE, (nint)procPtr, userData);
        }


        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        static void UserExecCodeProc(nint lParam)
        {
            if (XSchedulerTaskClosure.TryGet<XSchedulerTaskClosure>(lParam, out var taskClosure))
            {
                taskClosure.TryExecute();
            }
        }

        unsafe static bool OnSyncCallback(nint hwnd, EnumWindowMsgCode msgCode, nint wParam, nint lParam, WinMsgHookItem _)
        {
            if (msgCode == EnumWindowMsgCode.USER_EXEC_CODE)
            {
                ExecUnmanagedCodeProc procPtr = (ExecUnmanagedCodeProc)wParam;
                procPtr(lParam);
                return true;
            }
            return false;
        }
    }

}
