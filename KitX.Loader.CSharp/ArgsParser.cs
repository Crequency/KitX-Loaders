using CommandLine;

namespace KitX.Loader.CSharp;

public class ArgsParser
{
    public static void Parse(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(async option =>
            {
                if (option.PluginPath is null)
                    return;

                if (option.WorkingDirectory is not null)
                    Directory.SetCurrentDirectory(option.WorkingDirectory);

                var communicationManager = new CommunicationManager();

                if (option.ConnectUrl is not null)
                    communicationManager = await communicationManager.Connect(option.ConnectUrl);
                else communicationManager = null;

                var pluginManager = new PluginManager()
                    .OnSendMessage(x => communicationManager?.SendMessageAsync(x))
                    .LoadPlugin(option.PluginPath);

                if (communicationManager is not null)
                    communicationManager.OnReceiveMessage = x => pluginManager.ReceiveMessage(x);
            });
    }
}
