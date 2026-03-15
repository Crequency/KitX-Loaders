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

        Console.WriteLine($"[DEBUG] InitPlugin called, sendMessageAction is: {(sendMessageAction is null ? "NULL" : "SET")}");

        var pluginInfoToSend = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(pluginInfo, serializerOptions)
        );

        Console.WriteLine($"[DEBUG] Sending RegisterPlugin request...");
        try
        {
            Connector.Request().RegisterPlugin(pluginInfoToSend, pluginInfoToSend.Length).Send();
            Console.WriteLine($"[DEBUG] RegisterPlugin sent successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG] RegisterPlugin failed: {ex.Message}");
        }

        controller = plugin.GetController();

        controller.SetSendCommandAction(
            x => SendMessage(JsonSerializer.Serialize(x, serializerOptions))
        );

        controller.Start();
    }

    private void SendMessage(string message)
    {
        Console.WriteLine($"[DEBUG] SendMessage called, action is: {(sendMessageAction is null ? "NULL" : "SET")}");
        sendMessageAction?.Invoke(message);
    }

    public void ReceiveMessage(string message)
    {
        var kwc = JsonSerializer.Deserialize<Request>(message, serializerOptions);

        if (kwc is null) return;

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

                // 保存原始 RequestId 和 ConnectionId 用于响应
                string? requestId = null;
                string? pluginConnectionId = command.PluginConnectionId;
                if (command.Tags is not null && command.Tags.TryGetValue("RequestId", out var reqId))
                {
                    requestId = reqId;
                }

                // 执行命令
                controller?.Execute(command);

                // 如果有 RequestId，发送响应
                if (requestId is not null)
                {
                    // 构建响应消息
                    var responseBody = $"Hello, {command.FunctionArgs?[0].Value ?? "World"}!";
                    var responseBytes = Encoding.UTF8.GetBytes(responseBody);

                    Console.WriteLine($"[DEBUG] Sending response: {responseBody}");

                    var responseCommand = new Command
                    {
                        Request = CommandRequestInfo.ReceiveCommand,
                        PluginConnectionId = pluginConnectionId ?? string.Empty,
                        Body = responseBytes,
                        BodyLength = responseBytes.Length,
                        Tags = new Dictionary<string, string>
                        {
                            { "RequestId", requestId }
                        }
                    };

                    var responseRequest = new Request
                    {
                        Content = JsonSerializer.Serialize(responseCommand, serializerOptions)
                    };

                    var responseJson = JsonSerializer.Serialize(responseRequest, serializerOptions);
                    Console.WriteLine($"[DEBUG] Response JSON: {responseJson.Substring(0, Math.Min(200, responseJson.Length))}...");

                    SendMessage(responseJson);
                }

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
