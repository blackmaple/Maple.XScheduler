namespace Maple.XScheduler
{
    internal sealed class XSchedulerTaskClosure_ActionArgs<TService, TArgs>(TService service, TArgs args, Action<TService, TArgs> action)
    : XSchedulerTaskClosure<TService, bool>(service)
        where TService : IXSchedulerContext
        where TArgs : notnull

    {
        public Action<TService, TArgs> Action { get; } = action;
        public TArgs Args { get; } = args;
        protected sealed override bool ExecuteImp()
        {
            Action.Invoke(this.Service, this.Args);
            return true;
        }


    }
}
