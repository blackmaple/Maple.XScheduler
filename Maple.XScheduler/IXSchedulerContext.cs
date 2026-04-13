namespace Maple.XScheduler
{
    public interface IXSchedulerContext 
    {
        

        IXSchedulerUnmanaged XScheduler { get; }

      //  public ValueTask<bool> SendAsync(XSchedulerTaskClosure closure) => this.Unmanaged.ExecAsync(closure);
    }

 
}
