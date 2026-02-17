using Maple.WindowsRuntimes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maple.XScheduler.SetTimer
{
    public class SetTimerSchedulerService : IXSchedulerService
    {
        public ValueTask<bool> ExecAsync(XSchedulerTask taskExecuter)
        {
            var hwnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            if (hwnd == nint.Zero)
            {
                return XSchedulerException.Throw<ValueTask<bool>>("NOT FOUND MainWindowHandle");
            }
            var b = SetTimer(hwnd, (nuint)taskExecuter.Handle);
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
            if (MPinned<XSchedulerTask>.TryGet((nint)nIDEvent, out var taskState))
            {
                taskState.TryExecute();
            }
        }
    }
}
