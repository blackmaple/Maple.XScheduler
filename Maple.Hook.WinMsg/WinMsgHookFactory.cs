using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Maple.Hook.WinMsg
{
    public sealed class WinMsgHookFactory(WinMsgLoopService msgLoopService) : IDisposable
    {
        ConcurrentDictionary<nint, WinMsgHookItem> Cache { get; } = new ConcurrentDictionary<nint, WinMsgHookItem>();
        ConcurrentBag<WinMsgHookItem> Bag { get; } = [];

        WinMsgLoopService WinMsgLoop { get; } = msgLoopService;

        private WinMsgHookItem CreateImp(nint hWnd)
        {
            // var loopService = this.Provider.GetRequiredService<WinMsgLoopService>();
            return new WinMsgHookItem(hWnd, WinMsgLoop);
        }

        public WinMsgHookItem Create(nint hWnd) => this.Cache.GetOrAdd(hWnd, CreateImp);
        public WinMsgHookItem CreateRequiresNew(nint hWnd)
        {
            var hook = CreateImp(hWnd);
            this.Bag.Add(hook);
            return hook;
        }

        public void Dispose()
        {
            foreach (var c in Cache)
            {
        //        c.Value.Stop();
                c.Value.Dispose();
            }
            Cache.Clear();
            foreach (var c in Bag)
            {
       //         c.Stop();
                c.Dispose();
            }
            Bag.Clear();
        }
    }
}