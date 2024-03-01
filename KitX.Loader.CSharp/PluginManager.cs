using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Text;
using System.Text.Json;
using KitX.Contract.CSharp;
using KitX.Shared.CSharp.Plugin;
using KitX.Shared.CSharp.WebCommand;
using KitX.Shared.CSharp.WebCommand.Details;
using KitX.Shared.CSharp.WebCommand.Infos;

namespace KitX.Loader.CSharp;

public class PluginManager
{
    private PluginInfo? pluginInfo;

    private IController? controller;

    private Action<string>? sendMessageAction;

    private readonly Connector Connector = Connector.Instance;

    private static readonly JsonSerializerOptions serializerOptions = new()
    {
        WriteIndented = true,
        IncludeFields = true,
        PropertyNameCaseInsensitive = true,
    };

    public PluginManager()
    {
        Connector = Connector
            .SetSender(
                x => SendMessage(
                    JsonSerializer.Serialize(x, serializerOptions)
                )
            )
            .SetSerializer(
                x => JsonSerializer.Serialize(x, serializerOptions)
            );
        ;
    }

    public PluginManager OnSendMessage(Action<string> action)
    {
        sendMessageAction = action;

        return this;
    }

    public PluginManager LoadPlugin(string path)
    {
        if (!File.Exists(path))
            throw new ArgumentException("File not exist.", nameof(path));

        var dirPath = Path.GetDirectoryName(path);

        var fileName = Path.GetFileName(path);

        if (dirPath is null) throw new Exception("Can't get directory path of plugin file.");

        AddDllResolveHandler(dirPath);

        var catalog = new DirectoryCatalog(dirPath, fileName);

        var container = new CompositionContainer(catalog);

        var sub = container.GetExportedValues<IIdentityInterface>();

        InitPlugin(sub.First());

        return this;
    }

    private void InitPlugin(IIdentityInterface plugin)
    {
        pluginInfo = plugin.GetPluginInfo();

        var pluginInfoToSend = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(pluginInfo, serializerOptions)
        );

        Connector.Request().RegisterPlugin(pluginInfoToSend, pluginInfoToSend.Length).Send();

        controller = plugin.GetController();

        controller.SetSendCommandAction(
            x => SendMessage(JsonSerializer.Serialize(x, serializerOptions))
        );

        controller.Start();
    }

    private void SendMessage(string message) => sendMessageAction?.Invoke(message);

    public void ReceiveMessage(string message)
    {
        var kwc = JsonSerializer.Deserialize<Request>(message, serializerOptions);

        var command = JsonSerializer.Deserialize<Command>(kwc.Content, serializerOptions);

        switch (command.Request)
        {
            case CommandRequestInfo.ReceiveWorkingDetail:

                var workingDetailJson = Encoding.UTF8.GetString(command.Body);

                var workingDetail = JsonSerializer.Deserialize<PluginWorkingDetail>(workingDetailJson);

                if (workingDetail is not null)
                    controller?.SetWorkingDetail(workingDetail);

                break;

            case CommandRequestInfo.ReceiveCommand:

                controller?.Execute(command);

                break;
        }
    }

    private static void AddDllResolveHandler(string appendPath)
    {
        var domain = AppDomain.CurrentDomain;

        domain.AssemblyResolve += (sender, args) =>
        {
            var assemblyFolder = Path.GetFullPath(appendPath);
            var assemblyPath = Path.Combine(assemblyFolder, new AssemblyName(args.Name).Name + ".dll");

            if (!File.Exists(assemblyPath)) return null;

            var assembly = Assembly.LoadFrom(assemblyPath);

            return assembly;
        };
    }
}
