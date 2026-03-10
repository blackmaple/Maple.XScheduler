namespace Maple.XScheduler
{
    public interface IXSchedulerFactory
    {
        IXSchedulerUnmanaged Create( );
        IXSchedulerUnmanaged Create(nint hWnd);

    }


    public interface IXSchedulerFactory<T> : IXSchedulerFactory
    {
        IXSchedulerUnmanaged Create(nint hWnd, T raw);

    }

}
