using Maple.Hook.WinMsg;
using Maple.WinForm.Test;
using Maple.XScheduler.SetTimer;
using Maple.XScheduler.WinMsg;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole();
builder.Services.AddTransient<Form1>();
builder.Services.AddHostedService<WindowsFormsLifetime<Form1>>();
builder.Services.AddWinMsgHookFactory();
builder.Services.AddWinRTSetTimerProvider();
//Maple.XScheduler.SetTimer.WinRTSetTimerExtensions.TryAddXScheduler(builder.Services);
//Maple.XScheduler.WinMsg.XSchedulerUnmanagedExtensions.TryAddXScheduler(builder.Services);
var app = builder.Build();
await app.RunAsync().ConfigureAwait(false);