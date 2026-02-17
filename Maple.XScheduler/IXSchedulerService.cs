namespace Maple.XScheduler
{
    public interface IXSchedulerService
    {
        ValueTask<bool> ExecAsync(XSchedulerTaskClosure closure);
    }
}
