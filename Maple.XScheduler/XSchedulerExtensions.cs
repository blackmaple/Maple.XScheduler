using System.Diagnostics.CodeAnalysis;

namespace Maple.XScheduler
{


    public static class XSchedulerExtensions
    {
        public static async ValueTask<bool> XTaskAsync<TService>(
            this IXSchedulerContext<TService> taskScheduler, Action<TService> action)
            where TService : class
        {
            using var taskState = new XSchedulerTaskClosure_Action<TService>(taskScheduler, action);
            if (await taskScheduler.SendAsync(taskState).ConfigureAwait(false))
            {
                return await taskState.GetResultAsync().ConfigureAwait(false);
            }

            return XSchedulerException.Throw<bool>($"METHOD ERROR {nameof(XTaskAsync)}");
        }

        public static async ValueTask<bool> XTaskAsync<TService, TArgs>(
            this IXSchedulerContext<TService> taskScheduler, TArgs args, Action<TService, TArgs> actionArgs)
            where TService : class
            where TArgs : notnull
        {
            using var taskState = new XSchedulerTaskClosure_ActionArgs<TService, TArgs>(taskScheduler, args, actionArgs);
            if (await taskScheduler.SendAsync(taskState).ConfigureAwait(false))
            {
                return await taskState.GetResultAsync().ConfigureAwait(false);
            }

            return XSchedulerException.Throw<bool>($"METHOD ERROR {nameof(XTaskAsync)}");
        }

        public static async ValueTask<TResult?> XTaskAsync<TService, TArgs, TResult>(
            this IXSchedulerContext<TService> taskScheduler, TArgs args, Func<TService, TArgs, TResult> funcArgs)
            where TService : class
            where TArgs : notnull
        {
            using var taskState = new XSchedulerTaskClosure_FuncArgs<TService, TArgs, TResult>(taskScheduler, args, funcArgs);
            if (await taskScheduler.SendAsync(taskState).ConfigureAwait(false))
            {
                return await taskState.GetResultAsync().ConfigureAwait(false);
            }

            return XSchedulerException.Throw<TResult>($"METHOD ERROR {nameof(XTaskAsync)}");
        }

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
}
