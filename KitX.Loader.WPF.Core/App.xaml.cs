using System;
using System.Threading.Tasks;
using System.Windows;
using KitX.Loader.CSharp;

namespace KitX.Loader.WPF.Core;

public partial class App : Application
{
    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        try
        {
            await ArgsParser.ParseAsync(e.Args);
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
}
