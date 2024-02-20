using KitX.Loader.CSharp;
using System;
using System.Windows;

namespace KitX.Loader.WPF.Core;

public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        try
        {
            ArgsParser.Parse(e.Args);
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
