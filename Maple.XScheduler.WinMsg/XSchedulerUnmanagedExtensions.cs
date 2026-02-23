using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics;

namespace Maple.XScheduler.WinMsg
{
    public static class XSchedulerUnmanagedExtensions
    {
        extension(IServiceCollection @this)
        {
            //public IServiceCollection AddWindowsMsgLoopHook()
            //{
            //    @this.AddSingleton<IUnmanagedWindowsMsgLoopHook, UnmanagedWindowsMsgLoopHook>(p =>);
            //    @this.AddSingleton<UnmanagedWindowsMsgChannel>();
            //    return @this;
            //}

            public IServiceCollection TryAddXScheduler(nint hWnd)
            {
                if (hWnd == nint.Zero)
                {
                    return XSchedulerException.Throw<IServiceCollection>("INVALID WINDOW HANDLE");
                }
                @this.TryAddSingleton<IXSchedulerUnmanaged>(new XSchedulerUnmanagedWindowsMsgLoop(hWnd));
                return @this;
            }
            public IServiceCollection TryAddXScheduler() => @this.TryAddXScheduler(Process.GetCurrentProcess().MainWindowHandle);
        }
    }
}
