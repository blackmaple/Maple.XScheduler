namespace Maple.XScheduler
{
    public interface IXSchedulerContext 
    {
        

        IXSchedulerUnmanaged Unmanaged { get; }

      //  public ValueTask<bool> SendAsync(XSchedulerTaskClosure closure) => this.Unmanaged.ExecAsync(closure);
    }
}
