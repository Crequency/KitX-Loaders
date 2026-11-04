using KitX.Loader.CSharp;
using System.Threading.Tasks;

// 全局异常处理
AppDomain.CurrentDomain.UnhandledException += (_, e) =>
{
    Console.WriteLine($"[FATAL] Unhandled exception: {e.ExceptionObject}");
    Console.WriteLine($"[FATAL] Stack trace: {(e.ExceptionObject as Exception)?.StackTrace}");
    Environment.Exit(1);
};

TaskScheduler.UnobservedTaskException += (_, e) =>
{
    Console.WriteLine($"[ERROR] Unobserved task exception: {e.Exception.Message}");
    e.SetObserved();
};

// 使用同步方式阻塞等待异步解析完成
ArgsParser.ParseAsync(args).GetAwaiter().GetResult();
