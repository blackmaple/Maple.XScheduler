using Maple.WindowsRuntimes;
using Microsoft.Extensions.Logging;

namespace Maple.XScheduler.WinMsg
{
    public interface IUnmanagedWindowsMsgLoopHook
    {
        ILogger Logger { get; }
        Func<WindowsMsgInfo, ValueTask> NotifyAsync { set; get; }
    }
    //  public class T
}
