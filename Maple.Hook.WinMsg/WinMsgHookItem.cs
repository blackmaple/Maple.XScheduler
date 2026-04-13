using Maple.UnmanagedExtensions;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Maple.Hook.WinMsg
{
    using unsafe WindowProc = delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, LRESULT>;
    public class WinMsgHookItem : IDisposable
    {

        WinMsgLoopService LoopService { get; }
        nint MainWindowHandle { get; }
        nint _OldWindowsProc = nint.Zero;
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
            return this.SetSubClass();
        }

        public bool Stop()
        {
            return this.RemoveSubClass();
        }

        private unsafe bool SetHook()
        {
            if (_OldWindowsProc != nint.Zero)
            {
                return true;
            }
            WindowProc proc = &CallbackWindowProc;
            var hWnd = new HWND(this.MainWindowHandle);
            if (WindowLongNativeMethods.SetHook(hWnd, (nint)proc, out _OldWindowsProc))
            {
                WindowLongNativeMethods.SetPrivateData(hWnd, this.Handle);
                return true;
            }
            return false;

        }
        private unsafe bool RemoveHook()
        {
            var oldWindowsProc = _OldWindowsProc;
            if (oldWindowsProc == nint.Zero)
            {
                return true;
            }
            _OldWindowsProc = nint.Zero;
            var hWnd = new HWND(this.MainWindowHandle);
            WindowLongNativeMethods.SetPrivateData(hWnd, nint.Zero);
            return WindowLongNativeMethods.RemoveHook(hWnd, oldWindowsProc);
            //  return PInvoke.RemoveWindowSubclass(new HWND(this.MainWindowHandle), &CallbackSubclassProc, (nuint)this.Handle);

        }

        private unsafe bool SetSubClass()
        {
            var b = PInvoke.SetTimer(new HWND(this.MainWindowHandle), (nuint)this.Handle, PInvoke.USER_TIMER_MINIMUM, &TimerProc);
            return b != nuint.Zero;

            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
            static unsafe void TimerProc(HWND hwnd, uint message, nuint nIDEvent, uint dwTime)
            {
               
                PInvoke.KillTimer(hwnd, nIDEvent);
                var b = PInvoke.SetWindowSubclass(hwnd, &CallbackSubclassProc, (nuint)nIDEvent, (nuint)nIDEvent);
               
            }
        }
        private unsafe bool RemoveSubClass()
        {
            var b = PInvoke.SetTimer(new HWND(this.MainWindowHandle), (nuint)this.Handle, PInvoke.USER_TIMER_MINIMUM, &TimerProc);
            return b != nuint.Zero;
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
            static unsafe void TimerProc(HWND hwnd, uint message, nuint nIDEvent, uint dwTime)
            {

                PInvoke.KillTimer(hwnd, nIDEvent);
                var b= PInvoke.RemoveWindowSubclass(hwnd, &CallbackSubclassProc, nIDEvent);
           //     Debug.WriteLine(b.ToString());
            }
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        static unsafe LRESULT CallbackWindowProc(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam)
        {
            try
            {
                var dwRefData = WindowLongNativeMethods.GetPrivateData(hWnd);
                if (GCNormalObject<WinMsgHookItem>.TryGet(dwRefData, out var hook))
                {
                    hook.PushMsg(hWnd, msg, wParam, lParam);
                    if (hook.OnSyncCallback(hWnd, msg, wParam, lParam, hook) == true)
                    {
                        return new LRESULT(1);
                    }
                    return PInvoke.CallWindowProc((WindowProc)hook._OldWindowsProc, hWnd, msg, wParam, lParam);
                }
            }
            catch
            {

            }
            return PInvoke.DefWindowProc(hWnd, msg, wParam, lParam);

        }
        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        static unsafe LRESULT CallbackSubclassProc(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
        {
            try
            {
                if (GCNormalObject<WinMsgHookItem>.TryGet((nint)dwRefData, out var hook))
                {
                    hook.PushMsg(hWnd, msg, wParam, lParam);
                    if (hook.OnSyncCallback(hWnd, msg, wParam, lParam, hook) == true)
                    {
                        return new LRESULT(1);
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