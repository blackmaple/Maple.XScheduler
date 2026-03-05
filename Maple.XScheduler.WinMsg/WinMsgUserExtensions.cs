using Maple.Hook.WinMsg;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics;

namespace Maple.XScheduler.WinMsg
{
    public static class WinMsgUserExtensions
    {
        extension(IXSchedulerContext @this)
        {
            public IXSchedulerUnmanaged CreateXScheduler(nint hWnd, WinMsgHookItem hookItem)
            { 
                return new WinMsgUserImp(hWnd, hookItem);
            }
 
        }
    }
}
