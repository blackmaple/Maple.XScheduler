using Maple.WindowsRuntimes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading.Channels;

namespace Maple.XScheduler.WinMsg
{
    public class XSchedulerUnmanagedWindowsMsgLoop : IXSchedulerUnmanaged
    {
        public ValueTask<bool> ExecAsync(XSchedulerTaskClosure closure)
        {
            throw new NotImplementedException();
        }
    }


    public class UnmanagedWindowsMsgLoopHook(ILogger<UnmanagedWindowsMsgLoopHook> logger )
    {
        public ILogger Logger { get; } = logger;
        
        public Func<WindowsMsgInfo,ValueTask>? NotifyAsync { set; get; }



    }

    class UnmanagedWindowsMsgChannelService(UnmanagedWindowsMsgLoopHook hook)
    {
        Channel<WindowsMsgInfo> MsgChannel { get; } = Channel.CreateUnbounded<WindowsMsgInfo>();
        UnmanagedWindowsMsgLoopHook Hook { get; } = hook;
        ILogger Logger => Hook.Logger;
        IWindowsMsgNotifyService MsgNotifyService => Hook.MsgNotifyService;

        public ValueTask WriteMsgAsync(WindowsMsgInfo info) => this.MsgChannel.Writer.WriteAsync(info);
        public bool TryWriteMsg(WindowsMsgInfo info) => this.MsgChannel.Writer.TryWrite(info);
        public bool TryComplete() => this.MsgChannel.Writer.TryComplete();
        public void Complete() => this.MsgChannel.Writer.Complete();
        private IAsyncEnumerable<WindowsMsgInfo> ReadAllAsync()=> this.MsgChannel.Reader.ReadAllAsync();

        public Task RunAsync() => NotifyMsgLoopAsync();

        protected async Task NotifyMsgLoopAsync()
        {
            await foreach (var dto in this.ReadAllAsync().ConfigureAwait(false))
            {
               
                try
                {
                    await MsgNotifyService.NotifyAsync(dto).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, "");
                }
            }

        }
    }
    //  public class T


    public static class XSchedulerUnmanagedExtensions
    {
        public static IServiceCollection AddWindowsMsgLoopHook<[DynamicallyAccessedMembers]T>(this IServiceCollection @this)
        {
            return @this;
        }
    }
}
