using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32;

namespace Maple.XScheduler.SetTimer
{
    internal class WinRTSetTimerImp(nint hWnd) : IXSchedulerUnmanaged
    {
        public nint MainWindowHandle { get; } = hWnd;

        public ValueTask<bool> ExecAsync(XSchedulerTaskClosure taskClosure)
        {

            var b = SetTimer(this.MainWindowHandle, (nuint)taskClosure.Handle);
            return new ValueTask<bool>(b);
        }


        unsafe static bool SetTimer(nint hwnd, nuint nIDEvent)
        {
          //  const uint USER_TIMER_MINIMUM = 0xA;
            //  const uint USER_TIMER_MAXIMUM = 0x7FFFFFFF;
            
            var b = PInvoke.SetTimer(new Windows.Win32.Foundation.HWND(hwnd), nIDEvent, PInvoke.USER_TIMER_MINIMUM, &TimerProc);
            return b != nuint.Zero;
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        static void TimerProc(Windows.Win32.Foundation.HWND hwnd, uint message, nuint nIDEvent, uint dwTime)
        {
            PInvoke.KillTimer(hwnd, nIDEvent);
            if (XSchedulerTaskClosure.TryGet<XSchedulerTaskClosure>((nint)nIDEvent, out var taskClosure))
            {
                taskClosure.TryExecute();
            }
        }
    }
}
