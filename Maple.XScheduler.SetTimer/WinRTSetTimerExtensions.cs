using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics;

namespace Maple.XScheduler.SetTimer
{
    public static class WinRTSetTimerExtensions
    {
        extension(IXSchedulerContext @this)
        {
            public IXSchedulerUnmanaged CreateXScheduler(nint hWnd)
            {
                return new WinRTSetTimerImp(hWnd);
            }
        }
    }


     
}
