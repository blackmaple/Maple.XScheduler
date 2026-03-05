namespace Maple.XScheduler
{
    public interface IXSchedulerContext 
    {
        

        IXSchedulerUnmanaged Scheduler { get; }

      //  public ValueTask<bool> SendAsync(XSchedulerTaskClosure closure) => this.Unmanaged.ExecAsync(closure);
    }
}
