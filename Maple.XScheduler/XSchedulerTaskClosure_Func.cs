namespace Maple.XScheduler
{


    internal sealed class XSchedulerTaskClosure_Func<TService, TResult>(TService service, Func<TService, TResult> func)
        : XSchedulerTaskClosure<TService, TResult>(service)
        where TService : IXSchedulerContext
    {
        public Func<TService, TResult> Func { get; } = func;

        protected sealed override TResult ExecuteImp() => Func.Invoke(this.Service);


    }
}
