using Maple.WindowsRuntimes;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Maple.XScheduler.WinMsg
{
    class UnmanagedWindowsMsgChannel(UnmanagedWindowsMsgLoopHook hook)
    {
        Channel<WindowsMsgInfo> MsgChannel { get; } = Channel.CreateUnbounded<WindowsMsgInfo>();
        UnmanagedWindowsMsgLoopHook Hook { get; } = hook;
        ILogger Logger => Hook.Logger;
        Func<WindowsMsgInfo, ValueTask> NotifyAsync => Hook.NotifyAsync;

        //      public ValueTask WriteMsgAsync(WindowsMsgInfo info) => this.MsgChannel.Writer.WriteAsync(info);
        public bool TryWriteMsg(WindowsMsgInfo info) => this.MsgChannel.Writer.TryWrite(info);
        public bool Close() => this.MsgChannel.Writer.TryComplete();
        //      public void Complete() => this.MsgChannel.Writer.Complete();
        private IAsyncEnumerable<WindowsMsgInfo> ReadAllAsync() => this.MsgChannel.Reader.ReadAllAsync();

        public Task RunAsync() => NotifyMsgLoopAsync();

        protected async Task NotifyMsgLoopAsync()
        {
            await foreach (var dto in this.ReadAllAsync().ConfigureAwait(false))
            {

                try
                {
                    await NotifyAsync.Invoke(dto).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, "");
                }
            }

        }
    }
}
