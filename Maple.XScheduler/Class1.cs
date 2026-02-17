using System.Diagnostics.CodeAnalysis;

namespace Maple.XScheduler
{

    public interface IXSchedulerContext<TService>
        where TService : class
    {
        TService Service { get; }

        IXSchedulerService Scheduler { get; }

        public ValueTask<bool> SendAsync(XSchedulerTaskClosure closure) => this.Scheduler.ExecAsync(closure);
    }


    public static class XSchedulerExtensions
    {
        public static async ValueTask<TResult?> XTaskAsync<TService, TResult>(
            this IXSchedulerContext<TService> taskScheduler, Func<TService, TResult> func)
            where TService : class
        {
            using var taskState = new XSchedulerTaskClosure_Func<TService, TResult>(taskScheduler, func);
            if (await taskScheduler.SendAsync(taskState).ConfigureAwait(false))
            {
                return await taskState.GetResultAsync().ConfigureAwait(false);
            }
            return XSchedulerException.Throw<TResult>($"METHOD ERROR {nameof(XTaskAsync)}");
        }

    }


    internal sealed class XSchedulerTaskClosure_Func<TService, TResult>(IXSchedulerContext<TService> schedulerContext, Func<TService, TResult> func)
        : XSchedulerTaskClosure<TService, TResult>(schedulerContext)
        where TService : class
    {
        public Func<TService, TResult?> Func { get; } = func;

        protected sealed override TResult? ExecuteImp() => Func.Invoke(this.Service);


    }
}
