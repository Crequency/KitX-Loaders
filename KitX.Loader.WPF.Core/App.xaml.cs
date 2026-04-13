using System;
using System.Windows;
using KitX.Loader.CSharp;

namespace KitX.Loader.WPF.Core;

public partial class App : Application
{
    private CommunicationManager? _communicationManager;

    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        try
        {
            // 使用非阻塞方式加载插件，WPF 自身的消息循环管理进程生命周期
            _communicationManager = await ArgsParser.LoadWithoutBlockingAsync(e.Args);
        }
        catch (Exception o)
        {
            MessageBox.Show(
                o.Message,
                "Loader Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            Console.WriteLine(o.Message);

            Environment.Exit(1);
        }
    }

    private async void Application_Exit(object sender, ExitEventArgs e)
    {
        // 优雅退出：关闭 WebSocket 连接
        if (_communicationManager is not null)
        {
            try
            {
                await _communicationManager.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Error closing communication: {ex.Message}");
            }
        }
    }
}
