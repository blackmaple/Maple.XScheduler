using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace Maple.Hook.WinMsg
{

    public static class WinMsgHookExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddWinMsgHookFactory()
            {
                services.TryAddSingleton<WinMsgLoopService>();
                services.AddHostedService(p => p.GetRequiredService<WinMsgLoopService>());
                services.TryAddSingleton<WinMsgHookFactory>();
                return services;
            }


        }
    }
}