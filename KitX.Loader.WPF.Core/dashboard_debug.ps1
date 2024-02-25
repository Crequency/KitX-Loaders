param(
    [int]
    $From = 1,

    [int]
    $To = $(throw "`To` not provide"),

    [int]
    $Port = $(throw "`Port` not provide"),

    [double]
    $Sleep = 0.7
)

$From..$To | ForEach-Object {
    .\bin\Debug\net8.0-windows\KitX.Loader.WPF.Core.exe --load '..\..\KitX Plugins\TestPlugin.WPF.Core\bin\Debug\net8.0-windows\TestPlugin.WPF.Core.dll' --connect "ws://127.0.0.1:$Port/connectionId_$_";
    Start-Sleep $Sleep;
}
