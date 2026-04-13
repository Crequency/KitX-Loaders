using System.Threading;
using System.Threading.Tasks;
using CommandLine;

namespace KitX.Loader.CSharp;

public class ArgsParser
{
    public static void Parse(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(async option =>
            {
                await ParseOptionAsync(option);
            });
    }

    public static Task ParseAsync(string[] args)
    {
        var tcs = new TaskCompletionSource<bool>();

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(async option =>
            {
                try
                {
                    await ParseOptionAsync(option);
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            })
            .WithNotParsed(errors =>
            {
                tcs.SetResult(false);
            });

        return tcs.Task;
    }

    private static async Task ParseOptionAsync(Options option)
    {
        Console.WriteLine($"[DEBUG] ArgsParser: PluginPath = {option.PluginPath}");
        Console.WriteLine($"[DEBUG] ArgsParser: ConnectUrl = {option.ConnectUrl}");
        Console.WriteLine($"[DEBUG] ArgsParser: WorkingDirectory = {option.WorkingDirectory}");

        if (option.PluginPath is null)
            return;

        if (option.WorkingDirectory is not null)
            Directory.SetCurrentDirectory(option.WorkingDirectory);

        var communicationManager = new CommunicationManager();

        if (option.ConnectUrl is not null)
        {
            Console.WriteLine($"[DEBUG] Connecting to {option.ConnectUrl}...");
            try
            {
                communicationManager = await communicationManager.Connect(option.ConnectUrl);
                Console.WriteLine($"[DEBUG] Connected successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Connection failed: {ex.Message}");
                Console.WriteLine($"[DEBUG] Stack trace: {ex.StackTrace}");
            }
        }
        else
        {
            Console.WriteLine("[DEBUG] No ConnectUrl provided, running without connection");
            communicationManager = null;
        }

        var pluginManager = new PluginManager()
            .OnSendMessage(x => communicationManager?.SendMessageAsync(x))
            .LoadPlugin(option.PluginPath);

        if (communicationManager is not null)
            communicationManager.OnReceiveMessage = x => pluginManager.ReceiveMessage(x);

        // === 进程保活：等待退出信号 ===
        var exitSignal = new ManualResetEventSlim(false);

        // 响应 Ctrl+C / Ctrl+Break
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            exitSignal.Set();
        };

        // 响应进程终止事件
        AppDomain.CurrentDomain.ProcessExit += (_, _) => exitSignal.Set();

        // 等待退出信号
        exitSignal.Wait();

        // 优雅退出：关闭 WebSocket 连接
        if (communicationManager is not null)
            await communicationManager.Close();
    }

    /// <summary>
    /// 仅执行插件加载和连接，不阻塞等待退出信号。
    /// 适用于有自身消息循环的宿主（如 WPF），由宿主管理进程生命周期。
    /// </summary>
    /// <param name="args">命令行参数</param>
    /// <returns>用于优雅关闭的 CommunicationManager 实例</returns>
    public static async Task<CommunicationManager?> LoadWithoutBlockingAsync(string[] args)
    {
        CommunicationManager? communicationManager = null;

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(async option =>
            {
                Console.WriteLine($"[DEBUG] ArgsParser: PluginPath = {option.PluginPath}");
                Console.WriteLine($"[DEBUG] ArgsParser: ConnectUrl = {option.ConnectUrl}");
                Console.WriteLine($"[DEBUG] ArgsParser: WorkingDirectory = {option.WorkingDirectory}");

                if (option.PluginPath is null)
                    return;

                if (option.WorkingDirectory is not null)
                    Directory.SetCurrentDirectory(option.WorkingDirectory);

                communicationManager = new CommunicationManager();

                if (option.ConnectUrl is not null)
                {
                    Console.WriteLine($"[DEBUG] Connecting to {option.ConnectUrl}...");
                    try
                    {
                        communicationManager = await communicationManager.Connect(option.ConnectUrl);
                        Console.WriteLine($"[DEBUG] Connected successfully!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[DEBUG] Connection failed: {ex.Message}");
                        communicationManager = null;
                    }
                }
                else
                {
                    Console.WriteLine("[DEBUG] No ConnectUrl provided, running without connection");
                    communicationManager = null;
                }

                var pluginManager = new PluginManager()
                    .OnSendMessage(x => communicationManager?.SendMessageAsync(x))
                    .LoadPlugin(option.PluginPath);

                if (communicationManager is not null)
                    communicationManager.OnReceiveMessage = x => pluginManager.ReceiveMessage(x);
            });

        return communicationManager;
    }
}
