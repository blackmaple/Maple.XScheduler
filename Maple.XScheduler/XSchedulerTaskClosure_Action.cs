namespace Maple.XScheduler
{
    internal sealed class XSchedulerTaskClosure_Action<TService>(IXSchedulerContext<TService> schedulerContext, Action<TService> action)
    : XSchedulerTaskClosure<TService, bool>(schedulerContext)
    where TService : class
    {
        public Action<TService> Action { get; } = action;

        protected sealed override bool ExecuteImp()
        {
            Action.Invoke(this.Service);
            return true;
        }


    }
}
