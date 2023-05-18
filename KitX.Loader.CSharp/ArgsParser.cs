using CommandLine;

namespace KitX.Loader.CSharp;

public class ArgsParser
{
    public static void Parse(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(option =>
            {
                var communicationManager = new CommunicationManager();

                if (option.DashboardIpAddress is not null)
                    communicationManager = communicationManager
                        .Connect(option.DashboardIpAddress ?? "");
                else communicationManager = null;

                var pluginManager = new PluginManager()
                    .OnSendMessage(x => communicationManager?.SendMessage(x))
                    .LoadPlugin(option.PluginPath ?? "");
            });
    }
}
