using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Maple.Hook.WinMsg
{
    public sealed class WinMsgHookFactory(IServiceProvider serviceProvider) : IDisposable
    {
        ConcurrentDictionary<nint, WinMsgHookItem> Cache { get; } = new ConcurrentDictionary<nint, WinMsgHookItem>();
        IServiceProvider Provider { get; } = serviceProvider;
        private WinMsgHookItem CreateImp(nint hWnd)
        {
            var loopService = this.Provider.GetRequiredService<WinMsgLoopService>();
            var loggerFactory = this.Provider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger($"{nameof(WinMsgHookItem)}_{hWnd:X8}");
            return new WinMsgHookItem(logger, hWnd, loopService);
        }

        public WinMsgHookItem Create(nint hWnd) => this.Cache.GetOrAdd(hWnd, CreateImp);

        public void Dispose()
        {
            foreach (var c in Cache)
            {
                c.Value.Stop();
                c.Value.Dispose();
            }
        }
    }
}