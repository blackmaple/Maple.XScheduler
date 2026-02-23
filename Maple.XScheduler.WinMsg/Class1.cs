using Maple.UnmanagedExtensions;
using Maple.WindowsRuntimes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Channels;

namespace Maple.XScheduler.WinMsg
{
    using unsafe ExecUnmanagedCodeProc = delegate* unmanaged[Stdcall]<nint, void>;

    public class XSchedulerUnmanagedWindowsMsgLoop(nint hWnd) : IXSchedulerUnmanaged
    {
        public nint MainWindowHandle { get; } = hWnd;

        public ValueTask<bool> ExecAsync(XSchedulerTaskClosure taskClosure)
        {
            if (MainWindowHandle == nint.Zero)
            {
                return XSchedulerException.Throw<ValueTask<bool>>("ERROR:MainWindowHandle");
            }
            var b = PostMessage(MainWindowHandle, taskClosure.Handle);
            return new ValueTask<bool>(b);
        }

        unsafe static bool PostMessage(nint hwnd, nint userData)
        {
            ExecUnmanagedCodeProc procPtr = &UserExecCodeProc;
            return RTUser32.PostMessage(hwnd, EnumWindowMsgCode.USER_EXEC_CODE, (nint)procPtr, userData);
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        static void UserExecCodeProc(nint lParam)
        {
            if (XSchedulerTaskClosure.TryGet<XSchedulerTaskClosure>(lParam, out var taskClosure))
            {
                taskClosure.TryExecute();
            }
        }
    }
1
    public class UnmanagedWindowsMsgLoopHook : GCNormalSelf 
    {
        public ILogger Logger { get; }
        public Func<WindowsMsgInfo, ValueTask> NotifyAsync { set; get; } = static (_) => ValueTask.CompletedTask;
        UnmanagedWindowsMsgChannel Channel { get; }
        nint MainWindowHandle { get; }
        public UnmanagedWindowsMsgLoopHook(ILogger<UnmanagedWindowsMsgLoopHook> logger ,nint hWnd)
        {
            this.Logger = logger;
            this.MainWindowHandle = hWnd;
            this.Channel = new UnmanagedWindowsMsgChannel(this);
         }

        public static UnmanagedWindowsMsgLoopHook Create(IServiceProvider serviceProvider)
        {
            var hWnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            if (hWnd == nint.Zero)
            {
                return XSchedulerException.Throw<UnmanagedWindowsMsgLoopHook>("NOT FOUND MainWindowHandle");
            }
            var logger = serviceProvider.GetRequiredService<ILogger<UnmanagedWindowsMsgLoopHook>>();
            return new UnmanagedWindowsMsgLoopHook(logger, hWnd);
        }

        public void Run()
        {
            _=this.Channel.RunAsync();
            this.SetHook();
        }

        public void Close()
        {
            this.RemoveHook();
            this.Channel.Close();
        }

        private unsafe bool SetHook( )
        {
            return RTComCtl32.SetWindowSubclass(this.MainWindowHandle, new RTComCtl32.SubclassProcWrapper(&CallbackSubclassProc), (nuint)this.Handle, (nuint)this.Handle);

        }
        private unsafe bool RemoveHook( )
        {
            return RTComCtl32.RemoveWindowSubclass(this.MainWindowHandle, new RTComCtl32.SubclassProcWrapper(&CallbackSubclassProc), (nuint)this.Handle);

        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        static unsafe nint CallbackSubclassProc(nint hWnd, EnumWindowMsgCode msg, nint wParam, nint lParam, nuint uIdSubclass, nuint dwRefData)
        {
            try
            {
                if (UnmanagedWindowsMsgLoopHook.TryGet<UnmanagedWindowsMsgLoopHook>((nint)dwRefData, out var hook))
                {
                    if (msg == EnumWindowMsgCode.USER_EXEC_CODE)
                    {
                        if (lParam != nint.Zero)
                        {
                            var procPtr = (ExecUnmanagedCodeProc)lParam;
                            procPtr(lParam);
                        }
                        return nint.Zero;
                    }
                    else if (msg == EnumWindowMsgCode.WM_CLOSE)
                    {

                    }
                }
            }
            catch
            {

            }
            return RTComCtl32.DefSubclassProc(hWnd, msg, wParam, lParam);

        }


    }

    class UnmanagedWindowsMsgChannel(UnmanagedWindowsMsgLoopHook hook)
    {
        Channel<WindowsMsgInfo> MsgChannel { get; } = Channel.CreateUnbounded<WindowsMsgInfo>();
        UnmanagedWindowsMsgLoopHook Hook { get; } = hook;
        ILogger Logger => Hook.Logger;
        Func<WindowsMsgInfo, ValueTask> NotifyAsync => Hook.NotifyAsync;

  //      public ValueTask WriteMsgAsync(WindowsMsgInfo info) => this.MsgChannel.Writer.WriteAsync(info);
        public bool TryWriteMsg(WindowsMsgInfo info) => this.MsgChannel.Writer.TryWrite(info);
        public bool Close() =>  this.MsgChannel.Writer.TryComplete();
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
    //  public class T


    public static class XSchedulerUnmanagedExtensions
    {
        public static IServiceCollection AddWindowsMsgLoopHook(this IServiceCollection @this)
        {
            @this.AddSingleton<IUnmanagedWindowsMsgLoopHook, UnmanagedWindowsMsgLoopHook>(p =>);
            @this.AddSingleton<UnmanagedWindowsMsgChannel>();
            return @this;
        }
    }
}
