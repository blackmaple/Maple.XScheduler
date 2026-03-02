using System.Diagnostics.CodeAnalysis;

namespace Maple.XScheduler
{


    public static class XSchedulerExtensions
    {
        extension<TService>(TService service) where TService : IXSchedulerContext
        {
            public async Task<bool> XTaskAsync(Action<TService> action)
            {
                using var taskClosure_Action = new XSchedulerTaskClosure_Action<TService>(service, action);
                return await taskClosure_Action.ExecAsync().ConfigureAwait(false);
            }

            public async Task<bool> XTaskAsync<TArgs>(TArgs args, Action<TService, TArgs> actionArgs)
                where TArgs : notnull
            {
                using var taskClosure_ActionArgs = new XSchedulerTaskClosure_ActionArgs<TService, TArgs>(service, args, actionArgs);
                return await taskClosure_ActionArgs.ExecAsync().ConfigureAwait(false);
            }

            public async Task<TResult?> XTaskAsync<TArgs, TResult>(TArgs args, Func<TService, TArgs, TResult> funcArgs)
                where TArgs : notnull
            {
                using var taskClosure_FuncArgs = new XSchedulerTaskClosure_FuncArgs<TService, TArgs, TResult>(service, args, funcArgs);
                return await taskClosure_FuncArgs.ExecAsync().ConfigureAwait(false);
            }

            public async Task<TResult?> XTaskAsync<TResult>(Func<TService, TResult> func)
            {
                using var taskClosure_Func = new XSchedulerTaskClosure_Func<TService, TResult>(service, func);
                return await taskClosure_Func.ExecAsync().ConfigureAwait(false);
            }
        }
    }
}
