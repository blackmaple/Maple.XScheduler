namespace Maple.XScheduler
{
    public interface IXSchedulerUnmanaged
    {
        nint MainWindowHandle { get; }
        ValueTask<bool> ExecAsync(XSchedulerTaskClosure closure);
    }
}
