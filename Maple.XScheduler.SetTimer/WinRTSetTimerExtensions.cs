using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Maple.XScheduler.SetTimer
{
    public static class WinRTSetTimerExtensions
    {
        extension(IServiceCollection @this)
        {

            public IServiceCollection AddWinRTSetTimerFactory()
            {
                @this.TryAddSingleton<IXSchedulerFactory, WinRTSetTimerFactory>();
 
                return @this;
            }

        }
    }



}
