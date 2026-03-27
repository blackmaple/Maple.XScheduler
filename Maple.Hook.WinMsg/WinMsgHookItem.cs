using Maple.UnmanagedExtensions;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace Maple.Hook.WinMsg
{
    public class WinMsgHookItem : IDisposable
    {

        WinMsgLoopService LoopService { get; }
        nint MainWindowHandle { get; }
        GCNormalObject<WinMsgHookItem> GCNormalObject { get; }
        nint Handle => this.GCNormalObject.Handle;

        public AdditionalContentManager AdditionalContent { get; } = new();

       



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


        public WinMsgHookItem(nint hWnd, WinMsgLoopService loopService)
        {
            this.MainWindowHandle = hWnd;
            this.LoopService = loopService;
            this.GCNormalObject = new GCNormalObject<WinMsgHookItem>(this);
        }


    

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
                if (GCNormalObject<WinMsgHookItem>.TryGet((nint)dwRefData, out var hook))
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

        public void Dispose()
        {
            this.Stop();
            this.EnabledSyncCallback = false;
            this.EnabledAsyncCallback = false;
            this.SyncCallback = default;
            this.AsyncCallback = default;
           
            this.GCNormalObject.Dispose();
            this.AdditionalContent.Clear();
            GC.SuppressFinalize(this);
        }
    }
}