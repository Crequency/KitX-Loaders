using CommandLine;

namespace KitX.Loader.CSharp;

public class Options
{
    [Option('l', "load", Required = true, HelpText = "Path to plugin.")]
    public string? PluginPath { get; set; }

    [Option('c', "connect", Required = false, HelpText = "Dashboard IPv4 address.")]
    public string? DashboardIpAddress { get; set; }
}
