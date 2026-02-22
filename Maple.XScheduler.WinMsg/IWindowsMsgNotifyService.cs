using Maple.WindowsRuntimes;

namespace Maple.XScheduler.WinMsg
{
    public interface IWindowsMsgNotifyService
    {
        ValueTask NotifyAsync(WindowsMsgInfo info);
    }
    //  public class T
}
