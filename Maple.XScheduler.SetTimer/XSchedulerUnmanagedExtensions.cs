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
                services.TryAddSingleton<IXSchedulerUnmanaged>((_) => new XSchedulerUnmanagedSetTimer(hWnd));
                return services;
            }
            public IServiceCollection TryAddXScheduler()
            {
                services.TryAddSingleton<IXSchedulerUnmanaged>(static (_) => new XSchedulerUnmanagedSetTimer(Process.GetCurrentProcess().MainWindowHandle));
                return services;
            }
        }
    }
}
