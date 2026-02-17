namespace Maple.XScheduler
{


    internal sealed class XSchedulerTaskClosure_Func<TService, TResult>(IXSchedulerContext<TService> schedulerContext, Func<TService, TResult> func)
        : XSchedulerTaskClosure<TService, TResult>(schedulerContext)
        where TService : class
    {
        public Func<TService, TResult?> Func { get; } = func;

        protected sealed override TResult? ExecuteImp() => Func.Invoke(this.Service);


    }
}
