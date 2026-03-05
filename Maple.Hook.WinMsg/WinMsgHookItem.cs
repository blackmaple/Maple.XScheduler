using Maple.UnmanagedExtensions;
using Maple.WindowsRuntimes;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maple.Hook.WinMsg
{
    public class WinMsgHookItem(ILogger logger, nint hWnd, WinMsgLoopService loopService) : GCNormalSelf
    {
        ILogger Logger { get; } = logger;
        WinMsgLoopService LoopService { get; } = loopService;
        void PushMsg(nint hWnd, EnumWindowMsgCode msgCode, nint w, nint l)
        {
            if (this.EnabledAsyncCallback)
            {
                this.LoopService.TryWriteMsg(new WindowsMsgInfo<WinMsgHookItem>(this) { WParam = w, LParam = l, Msg = msgCode });
            }
        }
        public bool EnabledAsyncCallback { set; get; }
        public Func<WindowsMsgInfo<WinMsgHookItem>, ValueTask>? AsyncCallback { set; get; }
        internal ValueTask OnAsyncCallback(WindowsMsgInfo<WinMsgHookItem> msgInfo)
        {
            if (this.EnabledAsyncCallback && this.AsyncCallback is not null)
            {
                return this.AsyncCallback(msgInfo);
            }
            return ValueTask.CompletedTask;
        }

        public bool EnabledSyncCallback { set; get; }
        public Func<nint, EnumWindowMsgCode, nint, nint, WinMsgHookItem, bool>? SyncCallback { set; get; }
        internal bool OnSyncCallback(nint hWnd, EnumWindowMsgCode msgCode, nint w, nint l, WinMsgHookItem hookItem)
        {
            if (this.EnabledSyncCallback && this.SyncCallback is not null)
            {
                return this.SyncCallback.Invoke(hWnd, msgCode, w, l, hookItem);
            }
            return false;
        }



        nint MainWindowHandle { get; } = hWnd;

        public bool Start()
        {
            return this.SetHook();
        }

        public bool Stop()
        {
            return this.RemoveHook();
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
        static nint CallbackSubclassProc(nint hWnd, EnumWindowMsgCode msg, nint wParam, nint lParam, nuint uIdSubclass, nuint dwRefData)
        {
            try
            {
                if (WinMsgHookItem.TryGet<WinMsgHookItem>((nint)dwRefData, out var hook))
                {
                    hook.PushMsg(hWnd, msg, wParam, lParam);

                    if (hook.OnSyncCallback(hWnd, msg, wParam, lParam, hook) == true)
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