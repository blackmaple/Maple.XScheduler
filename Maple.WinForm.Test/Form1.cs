using Maple.Hook.WinMsg;
using Maple.WindowsRuntimes;
using Maple.XScheduler;
using Maple.XScheduler.SetTimer;
using Maple.XScheduler.WinMsg;

namespace Maple.WinForm.Test
{
    public partial class Form1 : Form, IXSchedulerContext
    {
        WinMsgHookItem HookItem { get; }
        public IXSchedulerUnmanaged Scheduler { get; }
        public Form1(WinMsgHookFactory hookFactory)
        {
            InitializeComponent();

            System.Diagnostics.Debug.WriteLine($"Form1:{Environment.CurrentManagedThreadId:X8}");
            this.HookItem = hookFactory.Create(this.Handle);
            this.HookItem.SyncCallback += (hWnd, msgCode, wParam, lParam, hookItem) =>
            {
                System.Diagnostics.Debug.WriteLine(
                    $"SyncCallback:{Environment.CurrentManagedThreadId:X8}=>{msgCode}");
                return false;
            };
            this.HookItem.EnabledSyncCallback = true;

            this.HookItem.AsyncCallback += (data) =>
            {

                System.Diagnostics.Debug.WriteLine(
                    $"AsyncCallback:{Environment.CurrentManagedThreadId:X8}=>{data.Msg}");
                return ValueTask.CompletedTask;
            };
            this.HookItem.EnabledAsyncCallback = true;

            this.HookItem.Start();

            //    this.Scheduler = WinMsgUserExtensions.CreateXScheduler(this, this.Handle, this.HookItem);
            this.Scheduler = WinRTSetTimerExtensions.CreateXScheduler(this, this.Handle);


        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await this.XTaskAsync(p => MessageBox.Show(p, "test"));
        }
    }
}
