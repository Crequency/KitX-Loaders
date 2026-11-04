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
        if (option.PluginPath is null)
            return;

        var communicationManager = await PrepareAsync(option);

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
    /// <returns>用于优雅关闭的 CommunicationManager 实例；无连接或解析失败时为 null</returns>
    public static async Task<CommunicationManager?> LoadWithoutBlockingAsync(string[] args)
    {
        // 通过 TCS 等待 WithParsed 的异步回调真正完成，避免在插件加载前就返回（竞态）。
        var tcs = new TaskCompletionSource<CommunicationManager?>();

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(async option =>
            {
                try
                {
                    tcs.SetResult(await PrepareAsync(option));
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            })
            .WithNotParsed(errors =>
            {
                tcs.SetResult(null);
            });

        return await tcs.Task;
    }

    /// <summary>
    /// 公共加载流程：切换工作目录、建立连接、加载插件并接线消息回调。
    /// 连接失败时回退为无连接模式，仅加载插件。
    /// </summary>
    private static async Task<CommunicationManager?> PrepareAsync(Options option)
    {
        if (option.PluginPath is null)
            return null;

        if (option.WorkingDirectory is not null)
            Directory.SetCurrentDirectory(option.WorkingDirectory);

        CommunicationManager? communicationManager = null;

        if (option.ConnectUrl is not null)
        {
            try
            {
                communicationManager = await new CommunicationManager().Connect(option.ConnectUrl);
            }
            catch
            {
                // 连接失败：回退为无连接模式，插件仍可本地运行
            }
        }

        var pluginManager = new PluginManager()
            .OnSendMessage(x => communicationManager?.SendMessageAsync(x))
            .LoadPlugin(option.PluginPath);

        if (communicationManager is not null)
            communicationManager.OnReceiveMessage = x => pluginManager.ReceiveMessage(x);

        return communicationManager;
    }
}
