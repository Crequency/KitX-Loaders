using CommandLine;

namespace KitX.Loader.CSharp;

public class Options
{
    [Option('l', "load", Required = true, HelpText = "Path to plugin.")]
    public string? PluginPath { get; set; }

    [Option('c', "connect", Required = false, HelpText = "Connect url.")]
    public string? ConnectUrl { get; set; }

    [Option("working-directory", Required = false, HelpText = "Set working directory.")]
    public string? WorkingDirectory { get; set; }
}
