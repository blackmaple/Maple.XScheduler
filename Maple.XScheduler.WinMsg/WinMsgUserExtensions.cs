using Maple.Hook.WinMsg;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Maple.XScheduler.WinMsg
{
    public static class WinMsgUserExtensions
    {
        extension(IServiceCollection @this)
        {

            public IServiceCollection AddWinMsgUserFactory()
            {
                @this.TryAddSingleton<WinMsgUserFactory>();
                @this.TryAddSingleton<IXSchedulerFactory<WinMsgHookItem>>(p => p.GetRequiredService<WinMsgUserFactory>());
                @this.TryAddSingleton<IXSchedulerFactory>(p => p.GetRequiredService<IXSchedulerFactory<WinMsgHookItem>>());
                return @this;
            }

        }
    }
}
