using KitX.Contract.CSharp;
using KitX.Web.Rules;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Text.Json;

namespace KitX.Loader.CSharp;

public class PluginManager
{
    private PluginStruct? pluginStruct;

    private IController? controller;

    private Queue<Command>? pluginSentCommandsBuffer;

    private Action<string>? sendMessageAction;

    public PluginManager()
    {
        pluginSentCommandsBuffer = new();
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

        foreach (var item in sub)
        {
            InitPlugin(item, path);
            break;
        }

        SendMessage($"PluginStruct: {JsonSerializer.Serialize(pluginStruct)}");

        return this;
    }

    private void InitPlugin(IIdentityInterface plugin, string path)
    {
        RegisterPluginStruct(plugin);

        controller = plugin.GetController();

        controller.SetRootPath(Path.GetDirectoryName(path));
        controller.SetCommandsSendBuffer(ref pluginSentCommandsBuffer);

        controller.Start();
    }

    private void RegisterPluginStruct(IIdentityInterface identity)
    {
        pluginStruct = new()
        {
            Name = identity.GetName(),
            Version = identity.GetVersion(),
            DisplayName = identity.GetDisplayName(),
            AuthorName = identity.GetAuthorName(),
            PublisherName = identity.GetPublisherName(),
            AuthorLink = identity.GetAuthorLink(),
            PublisherLink = identity.GetPublisherLink(),
            SimpleDescription = identity.GetSimpleDescription(),
            ComplexDescription = identity.GetComplexDescription(),
            TotalDescriptionInMarkdown = identity.GetTotalDescriptionInMarkdown(),
            IconInBase64 = identity.GetIconInBase64(),
            PublishDate = identity.GetPublishDate(),
            LastUpdateDate = identity.GetLastUpdateDate(),
            IsMarketVersion = identity.IsMarketVersion(),
            Tags = new(),
            Functions = identity.GetController().GetFunctions(),
            RootStartupFileName = identity.GetRootStartupFileName(),
        };
    }

    private void SendMessage(string message) => sendMessageAction?.Invoke(message);

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
