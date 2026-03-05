using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class WindowsFormsLifetime<TForm>(IHostApplicationLifetime hostLifetime, IServiceProvider services) : BackgroundService
    where TForm : Form
{
    private readonly IHostApplicationLifetime _hostLifetime = hostLifetime;
    private readonly IServiceProvider _services = services;
    private TForm? _mainForm;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 在单独的线程上启动 WinForms 消息循环，因为 StartAsync 通常运行在非 UI 线程
        var thread = new Thread(() =>
        {
            // 1. 设置 WinForms 同步上下文
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 2. 从 DI 容器解析主窗体（其依赖会被自动注入）
            _mainForm = _services.GetRequiredService<TForm>();

            // 3. 注册主窗体关闭事件：当主窗体关闭时，停止主机
            //_mainForm.FormClosed += (sender, args) =>
            //{
            //    // 触发主机停止
            //};

            // 4. 启动 WinForms 消息循环（这会阻塞线程，直到主窗体关闭）
            Application.Run(_mainForm);
            _hostLifetime.StopApplication();
        });
        thread.SetApartmentState(ApartmentState.STA); // WinForms 必须运行在 STA 线程
        thread.Start();
        return Task.CompletedTask;
    }
}