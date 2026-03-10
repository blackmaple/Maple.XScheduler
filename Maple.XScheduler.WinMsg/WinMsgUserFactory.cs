using Maple.Hook.WinMsg;
using System.Diagnostics;

namespace Maple.XScheduler.WinMsg
{
    public class WinMsgUserFactory(WinMsgHookFactory hookFactory) : IXSchedulerFactory<WinMsgHookItem>
    {

        WinMsgHookFactory HookFactory { get; } = hookFactory;

        public IXSchedulerUnmanaged Create(nint hWnd, WinMsgHookItem raw)
        {
            if (hWnd == nint.Zero)
            {
                return XSchedulerException.Throw<IXSchedulerUnmanaged>("INVALID MAINWINDOW HANDLE");
            }
            return new WinMsgUserImp(hWnd, raw);
        }

        public IXSchedulerUnmanaged Create(nint hWnd)
        {
            if (hWnd == nint.Zero)
            {
                return XSchedulerException.Throw<IXSchedulerUnmanaged>("INVALID MAINWINDOW HANDLE");
            }
            var hookItem = HookFactory.Create(hWnd);
            return new WinMsgUserImp(hWnd, hookItem);
        }

        public IXSchedulerUnmanaged Create() => Create(Process.GetCurrentProcess().MainWindowHandle);


    }
}
