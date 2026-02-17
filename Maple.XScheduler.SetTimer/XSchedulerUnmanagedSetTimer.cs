using Maple.WindowsRuntimes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maple.XScheduler.SetTimer
{
    internal class XSchedulerUnmanagedSetTimer : IXSchedulerUnmanaged
    {
        public ValueTask<bool> ExecAsync(XSchedulerTaskClosure taskClosure)
        {
            var hwnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            if (hwnd == nint.Zero)
            {
                return XSchedulerException.Throw<ValueTask<bool>>("NOT FOUND MainWindowHandle");
            }
            var b = SetTimer(hwnd, (nuint)taskClosure.Handle);
            return new ValueTask<bool>(b);
        }


        unsafe static bool SetTimer(nint hwnd, nuint nIDEvent)
        {
            const uint USER_TIMER_MINIMUM = 0xA;
            //  const uint USER_TIMER_MAXIMUM = 0x7FFFFFFF;

            var b = RTUser32.SetTimer(hwnd, nIDEvent, USER_TIMER_MINIMUM, new RTUser32.TimerProcWrapper(&TimerProc));
            return b != nuint.Zero;
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        static void TimerProc(nint hwnd, uint message, nuint nIDEvent, uint dwTime)
        {
            RTUser32.KillTimer(hwnd, nIDEvent);
            if (MPinned<XSchedulerTaskClosure>.TryGet((nint)nIDEvent, out var taskClosure))
            {
                taskClosure.TryExecute();
            }
        }
    }
}
