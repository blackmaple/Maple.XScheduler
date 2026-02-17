namespace Maple.XScheduler
{
    public interface IXSchedulerContext<TService>
        where TService : class
    {
        TService Service { get; }

        IXSchedulerUnmanaged Unmanaged { get; }

        public ValueTask<bool> SendAsync(XSchedulerTaskClosure closure) => this.Unmanaged.ExecAsync(closure);
    }
}
