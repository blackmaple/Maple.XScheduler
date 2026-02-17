namespace Maple.XScheduler
{
    public interface IXSchedulerUnmanaged
    {
        ValueTask<bool> ExecAsync(XSchedulerTaskClosure closure);
    }
}
