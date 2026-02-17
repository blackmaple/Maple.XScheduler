using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Maple.XScheduler.SetTimer
{
    public static class XSchedulerUnmanagedExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection TryAddXScheduler()
            {
                services.TryAddSingleton<IXSchedulerUnmanaged, XSchedulerUnmanagedSetTimer>();
                return services;
            }
        }
    }
}
