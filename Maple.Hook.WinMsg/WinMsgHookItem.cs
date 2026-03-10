using Maple.UnmanagedExtensions;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace Maple.Hook.WinMsg
{
    public class WinMsgHookItem(nint hWnd, WinMsgLoopService loopService) : GCNormalSelf
    {

        WinMsgLoopService LoopService { get; } = loopService;
        void PushMsg(nint _, uint msgCode, nuint w, nint l)
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
        public Func<nint, uint, nuint, nint, WinMsgHookItem, bool>? SyncCallback { set; get; }
        internal bool OnSyncCallback(nint hWnd, uint msgCode, nuint w, nint l, WinMsgHookItem hookItem)
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
            return PInvoke.SetWindowSubclass(new HWND(this.MainWindowHandle), &CallbackSubclassProc, (nuint)this.Handle, (nuint)this.Handle);

        }
        private unsafe bool RemoveHook()
        {
            return PInvoke.RemoveWindowSubclass(new HWND(this.MainWindowHandle), &CallbackSubclassProc, (nuint)this.Handle);

        }
        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        static LRESULT CallbackSubclassProc(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
        {
            try
            {
                if (WinMsgHookItem.TryGet<WinMsgHookItem>((nint)dwRefData, out var hook))
                {
                    hook.PushMsg(hWnd, msg, wParam, lParam);

                    if (hook.OnSyncCallback(hWnd, msg, wParam, lParam, hook) == true)
                    {
                        return new LRESULT(nint.Zero);
                    }
                }
            }
            catch
            {

            }
            return PInvoke.DefSubclassProc(hWnd, msg, wParam, lParam);

        }


    }
}