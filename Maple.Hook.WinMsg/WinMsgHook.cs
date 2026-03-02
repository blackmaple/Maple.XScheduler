using Maple.UnmanagedExtensions;
using Maple.WindowsRuntimes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maple.Hook.WinMsg
{
    using unsafe ExecUnmanagedCodeProc = delegate* unmanaged[Stdcall]<nint, void>;
    class WinMsgHook : GCNormalSelf
    {
        public ILogger Logger { get; }
        public Func<WindowsMsgInfo<WinMsgHook>, ValueTask> AsyncCallback { set; get; } = static (_) => ValueTask.CompletedTask;
        public Func<nint, EnumWindowMsgCode, nint, nint, WinMsgHook, bool> SyncCallback { set; get; } = static (_, _, _, _, _) => false;
        WinMsgChannel Channel { get; }
        nint MainWindowHandle { get; }
        public WinMsgHook(ILogger<WinMsgHook> logger, nint hWnd)
        {
            this.Logger = logger;
            this.MainWindowHandle = hWnd;
            this.Channel = new WinMsgChannel(this);
        }

        public static WinMsgHook Create(IServiceProvider serviceProvider, nint hWnd)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<WinMsgHook>>();
            return new WinMsgHook(logger, hWnd);
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
                if (WinMsgHook.TryGet<WinMsgHook>((nint)dwRefData, out var hook))
                {
                    hook.Channel.TryWriteMsg(new WindowsMsgInfo<WinMsgHook>(hook) { WParam = wParam, LParam = lParam, Msg = msg  });
                    if(hook.SyncCallback(hWnd, msg, wParam, lParam, hook))
                    {
                        return nint.Zero;
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
