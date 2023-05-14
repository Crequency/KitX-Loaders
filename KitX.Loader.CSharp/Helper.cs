using KitX.Contract.CSharp;
using KitX.Web.Rules;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace KitX.Loader.CSharp;

public class Helper
{
    public static void Init(string[] args)
    {
        try
        {
            //  初始化加载器
            InitLoader();

            //  处理启动参数
            ProcessStartupArgs(args);

            //  加载插件
            LoadPlugin(pluginPath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);

            Environment.Exit(1);
        }
    }

    /// <summary>
    /// 处理启动参数
    /// </summary>
    /// <param name="args">参数列表</param>
    private static void ProcessStartupArgs(string[] args)
    {
        for (int i = 0; i < args.Length; ++i)
        {
            if (i != args.Length - 1)
            {
                switch (args[i])
                {
                    case "--load":
                        ++i;
                        pluginPath = args[i];
                        break;
                    case "--connect":
                        ++i;
                        string hostname = args[i].Split(':')[0];
                        string port = args[i].Split(':')[1];
                        int portNum;
                        if (int.TryParse(port, out portNum))
                        {
                            try
                            {
                                client = new();

                                client.Connect(hostname, portNum);

                                reciveMessageThread = new(ReciveMessage);

                                reciveMessageThread.Start();
                            }
                            catch (Exception ex)
                            {
                                client?.Dispose();

                                Console.WriteLine($"Connection failed!\n{ex.Message}");
                            }
                        }
                        else Console.WriteLine("Bad port number!");
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 初始化加载器
    /// </summary>
    private static void InitLoader()
    {
        pluginSentCommandsBuffer = new();
    }

    /// <summary>
    /// 注册插件结构
    /// </summary>
    /// <param name="identity">插件结构</param>
    private static void RegisterPluginStruct(IIdentityInterface identity)
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

    /// <summary>
    /// 获取插件结构
    /// </summary>
    /// <returns>插件结构</returns>
    private static PluginStruct GetPluginStruct() => pluginStruct;

    /// <summary>
    /// 获取序列化的插件结构
    /// </summary>
    /// <param name="ps">插件结构</param>
    /// <returns>序列化的 Json 字符串</returns>
    private static string GetPluginStructInJson(PluginStruct ps) => JsonSerializer.Serialize(ps);

    private static string pluginPath = string.Empty;
    private static bool StillReceiving = true;
    private static Thread? reciveMessageThread;
    private static TcpClient? client;
    private static IController? controller;
    private static PluginStruct pluginStruct;

    private static Queue<Command>? pluginSentCommandsBuffer = null;

    /// <summary>
    /// 初始化插件
    /// </summary>
    /// <param name="plugin">插件接口实例</param>
    private static void InitPlugin(IIdentityInterface plugin, string path)
    {
        RegisterPluginStruct(plugin);

        controller = plugin.GetController();

        controller.SetRootPath(Path.GetDirectoryName(path));
        controller.SetCommandsSendBuffer(ref pluginSentCommandsBuffer);

        controller.Start();
    }

    /// <summary>
    /// 加载插件
    /// </summary>
    /// <param name="path">插件路径</param>
    private static void LoadPlugin(string path)
    {
        if (File.Exists(path))
        {
            var dirPath = Path.GetDirectoryName(path);
            var fileName = Path.GetFileName(path);

            if (dirPath is null || fileName is null) throw new Exception("Directory path is null!");

            var catalog = new DirectoryCatalog(dirPath, fileName);

            var container = new CompositionContainer(catalog);

            var sub = container.GetExportedValues<IIdentityInterface>();

            foreach (var item in sub)
            {
                InitPlugin(item, path);
                break;
            }

            SendMessage($"PluginStruct: {GetPluginStructInJson(GetPluginStruct())}");
        }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="content">消息内容</param>
    private static void SendMessage(string content)
    {
        var stream = client?.GetStream();

        if (stream is null) return;

        var data = Encoding.UTF8.GetBytes(content);

        try
        {
            stream.Write(data, 0, data.Length);
            stream.Flush();
        }
        catch
        {
            stream.Close();
            stream.Dispose();
        }
    }

    /// <summary>
    /// 接收消息
    /// </summary>
    private static void ReciveMessage()
    {
        if (client is null) return;

        var stream = client?.GetStream();

        if (stream is null) return;

        var buffer = new byte[1024 * 1024 * 10];  //  Default 10 MB buffer

        try
        {
            while (StillReceiving)
            {

                if (buffer is null) break;

                var length = stream.Read(buffer, 0, buffer.Length);

                if (length > 0)
                {
                    var msg = Encoding.UTF8.GetString(buffer, 0, length);

                    //ToDo: Process `msg`
                }
                else
                {
                    stream.Dispose();
                    break;
                }
            }

            stream.Close();
            stream.Dispose();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);

            stream.Close();
            stream.Dispose();

            client?.Close();
            client?.Dispose();
        }
    }
}

