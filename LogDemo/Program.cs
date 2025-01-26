// See https://aka.ms/new-console-template for more information
using Serilog;
using Serilog.Events;

// 配置Serilog
Log.Logger = new LoggerConfiguration()
    // 最小日志级别
    .MinimumLevel.Debug()
    // 输出到控制台
    .WriteTo.Console()
    // Error日志单独写入文件
    .WriteTo.File("logs\\error-.txt",
        restrictedToMinimumLevel: LogEventLevel.Error,
        rollingInterval: RollingInterval.Day)
    // Warning日志单独写入文件
    .WriteTo.File("logs\\warning-.txt",
        restrictedToMinimumLevel: LogEventLevel.Warning,
        rollingInterval: RollingInterval.Day)
    // Information日志单独写入文件
    .WriteTo.File("logs\\info-.txt",
        restrictedToMinimumLevel: LogEventLevel.Information,
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting up the application...");

    // 测试不同级别的日志
    Log.Debug("这是一条调试信息");
    Log.Information("这是一条信息消息");
    Log.Warning("这是一条警告消息");
    Log.Error("这是一条错误消息");
    Log.Fatal("这是一条致命错误消息");

    // 模拟一个异常
    try
    {
        throw new Exception("这是一个测试异常");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "程序运行时发生错误");
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "程序意外终止");
}
finally
{
    Log.CloseAndFlush();
}
