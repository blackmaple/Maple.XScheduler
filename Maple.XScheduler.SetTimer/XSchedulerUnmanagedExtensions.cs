using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics;

namespace Maple.XScheduler.SetTimer
{
    public static class XSchedulerUnmanagedExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection TryAddXScheduler(nint hWnd)
            {
                if (hWnd == nint.Zero)
                {
                    return XSchedulerException.Throw<IServiceCollection>("INVALID WINDOW HANDLE");
                }
                services.TryAddSingleton<IXSchedulerUnmanaged>(new XSchedulerUnmanagedSetTimer(hWnd));
                return services;
            }
            public IServiceCollection TryAddXScheduler() => services.TryAddXScheduler(Process.GetCurrentProcess().MainWindowHandle);


        }
    }
}
