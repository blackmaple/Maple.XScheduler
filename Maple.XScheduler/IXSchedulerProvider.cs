namespace Maple.XScheduler
{
    public interface IXSchedulerProvider
    {
        IXSchedulerUnmanaged Create( );
        IXSchedulerUnmanaged Create(nint hWnd);

    }
    public interface IXSchedulerProvider<T> : IXSchedulerProvider
    {
        IXSchedulerUnmanaged Create(nint hWnd, T raw);

    }

}
