namespace Maple.XScheduler
{
    internal sealed class XSchedulerTaskClosure_Action<TService>(TService service, Action<TService> action)
    : XSchedulerTaskClosure<TService, bool>(service)
    where TService : IXSchedulerContext
    {
        public Action<TService> Action { get; } = action;

        protected sealed override bool ExecuteImp()
        {
            Action.Invoke(this.Service);
            return true;
        }


    }
}
