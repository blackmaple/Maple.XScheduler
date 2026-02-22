using Maple.UnmanagedExtensions;

namespace Maple.XScheduler
{

    public abstract class XSchedulerTaskClosure() : GCPinnedSelf
    {
        public abstract void TryExecute();

        public static implicit operator nint(XSchedulerTaskClosure closure) => closure.Handle;
    }

    public abstract class XSchedulerTaskClosure<TResult>
        : XSchedulerTaskClosure
    {
        /// <summary>
        /// 开始执行
        /// </summary>
        protected TaskCompletionSource<bool> Executing { get; } = new TaskCompletionSource<bool>();

        /// <summary>
        /// 执行完成
        /// </summary>
        protected TaskCompletionSource<TResult?> Executed { get; } = new TaskCompletionSource<TResult?>();

        public sealed override void TryExecute()
        {
            try
            {
                this.Executing.SetResult(true);
                var v = this.ExecuteImp();
                this.Executed.SetResult(v);
            }
            catch (Exception ex)
            {
                this.Executed.SetException(ex);
            }
        }

        protected abstract TResult? ExecuteImp();

        public async Task<TResult?> GetResultAsync(TimeSpan timeSpan)
        {
            //5秒内未执行 则丢出time out
            if (await this.Executing.Task.WaitAsync(timeSpan).ConfigureAwait(false))
            {
                //等待执行完毕 不设置time out
                return await this.Executed.Task.ConfigureAwait(false);
            }
            return XSchedulerException.Throw<TResult>($"METHOD ERROR {nameof(GetResultAsync)}");
        }
        public Task<TResult?> GetResultAsync(long seconds = 5L) => GetResultAsync(TimeSpan.FromSeconds(seconds));
    }

    public abstract class XSchedulerTaskClosure<TService, TResult>(IXSchedulerContext<TService> schedulerContext)
        : XSchedulerTaskClosure<TResult>
        where TService : class
    {
        protected IXSchedulerContext<TService> SchedulerContext { get; } = schedulerContext;
        protected TService Service => this.SchedulerContext.Service;


    }
}
