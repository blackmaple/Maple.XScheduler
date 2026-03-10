using System.Diagnostics;

namespace Maple.XScheduler.SetTimer
{
    public class WinRTSetTimerFactory : IXSchedulerFactory
    {
        public IXSchedulerUnmanaged Create(nint hWnd)
        {
            if (hWnd == nint.Zero)
            {
                return XSchedulerException.Throw<IXSchedulerUnmanaged>("INVALID MAINWINDOW HANDLE");
            }
            return new WinRTSetTimerImp(hWnd);
        }

        public IXSchedulerUnmanaged Create()
        {
            return Create(Process.GetCurrentProcess().MainWindowHandle);
        }
    }



}
