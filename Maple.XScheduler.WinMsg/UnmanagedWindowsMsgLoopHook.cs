using Maple.UnmanagedExtensions;
using Maple.WindowsRuntimes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maple.XScheduler.WinMsg
{
    using unsafe ExecUnmanagedCodeProc = delegate* unmanaged[Stdcall]<nint, void>;
    class UnmanagedWindowsMsgLoopHook : GCNormalSelf
    {
        public ILogger Logger { get; }
        public Func<WindowsMsgInfo, ValueTask> NotifyAsync { set; get; } = static (_) => ValueTask.CompletedTask;
        UnmanagedWindowsMsgChannel Channel { get; }
        nint MainWindowHandle { get; }
        public UnmanagedWindowsMsgLoopHook(ILogger<UnmanagedWindowsMsgLoopHook> logger, nint hWnd)
        {
            this.Logger = logger;
            this.MainWindowHandle = hWnd;
            this.Channel = new UnmanagedWindowsMsgChannel(this);
        }

        public static UnmanagedWindowsMsgLoopHook Create(IServiceProvider serviceProvider, nint hWnd)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<UnmanagedWindowsMsgLoopHook>>();
            return new UnmanagedWindowsMsgLoopHook(logger, hWnd);
        }

        public void Run()
        {
            _ = this.Channel.RunAsync();
            this.SetHook();
        }

        public void Close()
        {
            this.RemoveHook();
            this.Channel.Close();
        }

        private unsafe bool SetHook()
        {
            return RTComCtl32.SetWindowSubclass(this.MainWindowHandle, new RTComCtl32.SubclassProcWrapper(&CallbackSubclassProc), (nuint)this.Handle, (nuint)this.Handle);

        }
        private unsafe bool RemoveHook()
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
                        hook.Close();
                    }
                }
            }
            catch
            {

            }
            return RTComCtl32.DefSubclassProc(hWnd, msg, wParam, lParam);

        }


    }
}
