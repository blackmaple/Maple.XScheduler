using Maple.Hook.WinMsg;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Maple.XScheduler.WinMsg
{
    public static class WinMsgUserExtensions
    {
        extension(IServiceCollection @this)
        {

            public IServiceCollection AddWinMsgUserProvider()
            {
                @this.TryAddSingleton<WinMsgUserProvider>();
                @this.TryAddSingleton<IXSchedulerProvider<WinMsgHookItem>>(p => p.GetRequiredService<WinMsgUserProvider>());
                @this.TryAddSingleton<IXSchedulerProvider>(p => p.GetRequiredService<IXSchedulerProvider<WinMsgHookItem>>());
                return @this;
            }

        }
    }
}
