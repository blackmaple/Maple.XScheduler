namespace Maple.XScheduler
{
    internal sealed class XSchedulerTaskClosure_FuncArgs<TService, TArgs, TResult>(TService service, TArgs args, Func<TService, TArgs, TResult> func)
    : XSchedulerTaskClosure<TService, TResult>(service)
    where TService : IXSchedulerContext
    where TArgs : notnull

    {
        public Func<TService, TArgs, TResult?> Func { get; } = func;
        public TArgs Args { get; } = args;
        protected sealed override TResult? ExecuteImp() => Func.Invoke(this.Service, this.Args);


    }
}
