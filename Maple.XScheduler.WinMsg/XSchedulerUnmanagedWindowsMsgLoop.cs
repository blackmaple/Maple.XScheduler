using Maple.WindowsRuntimes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maple.XScheduler.WinMsg
{
    using unsafe ExecUnmanagedCodeProc = delegate* unmanaged[Stdcall]<nint, void>;

    public class XSchedulerUnmanagedWindowsMsgLoop(nint hWnd) : IXSchedulerUnmanaged
    {
        public nint MainWindowHandle { get; } = hWnd;

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

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        static void UserExecCodeProc(nint lParam)
        {
            if (XSchedulerTaskClosure.TryGet<XSchedulerTaskClosure>(lParam, out var taskClosure))
            {
                taskClosure.TryExecute();
            }
        }
    }
1
}
