using Maple.WindowsRuntimes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Threading.Channels;
using System.Xml.Serialization;

namespace Maple.Hook.WinMsg
{
    public class WinMsgLoopService(ILogger<WinMsgLoopService> logger) : BackgroundService
    {
        Channel<WindowsMsgInfo<WinMsgHookItem>> MsgChannel { get; } = Channel.CreateUnbounded<WindowsMsgInfo<WinMsgHookItem>>();
        ILogger Logger { get; } = logger;

        public bool TryWriteMsg(WindowsMsgInfo<WinMsgHookItem> info) => this.MsgChannel.Writer.TryWrite(info);
        public bool Close() => this.MsgChannel.Writer.TryComplete();
        //      public void Complete() => this.MsgChannel.Writer.Complete();
        private IAsyncEnumerable<WindowsMsgInfo<WinMsgHookItem>> ReadAllAsync(CancellationToken stoppingToken) => this.MsgChannel.Reader.ReadAllAsync(stoppingToken);

        public sealed override Task StopAsync(CancellationToken cancellationToken)
        {
            this.Close();
            return base.StopAsync(cancellationToken);
        }


        protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var dto in this.ReadAllAsync(stoppingToken).ConfigureAwait(false))
            {
                try
                {
                    var hookItem = dto.Data;
                    await hookItem.OnAsyncCallback(dto).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, "");
                }
            }

        }
    }
}
