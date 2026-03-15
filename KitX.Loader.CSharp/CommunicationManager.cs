using System.Net.WebSockets;
using System.Text;

namespace KitX.Loader.CSharp;

public class CommunicationManager
{
    private readonly ClientWebSocket? Client;

    private int receiveBufferSize = 1024 * 1024 * 10; // 10MB

    public Action<string>? OnReceiveMessage { get; set; }

    public CommunicationManager()
    {
        Client = new();

        Client.Options.KeepAliveInterval = TimeSpan.FromSeconds(10);
    }

    public async Task<CommunicationManager> Connect(string? url)
    {
        Console.WriteLine($"[DEBUG] CommunicationManager.Connect: Starting connection to {url}");

        ArgumentNullException.ThrowIfNull(url, nameof(url));

        if (Client is null)
        {
            Console.WriteLine($"[DEBUG] CommunicationManager.Connect: Client is NULL!");
            throw new InvalidOperationException("ClientWebSocket is not initialized");
        }

        Console.WriteLine($"[DEBUG] CommunicationManager.Connect: Client state before connect: {Client.State}");

        try
        {
            Console.WriteLine($"[DEBUG] CommunicationManager.Connect: Calling ConnectAsync...");
            await Client.ConnectAsync(new Uri(url), CancellationToken.None);
            Console.WriteLine($"[DEBUG] CommunicationManager.Connect: ConnectAsync completed, state: {Client.State}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG] CommunicationManager.Connect: ConnectAsync failed: {ex.Message}");
            Console.WriteLine($"[DEBUG] CommunicationManager.Connect: Stack trace: {ex.StackTrace}");
            throw;
        }

        var waiting = true;
        var timeout = DateTime.Now.AddSeconds(30); // 30 second timeout

        Console.WriteLine($"[DEBUG] CommunicationManager.Connect: Waiting for connection open, initial state: {Client.State}");

        while (waiting && DateTime.Now < timeout)
        {
            switch (Client.State)
            {
                case WebSocketState.None:
                    Console.WriteLine($"[DEBUG] CommunicationManager.Connect: State = None");
                    waiting = false;
                    break;
                case WebSocketState.Connecting:
                    Console.WriteLine($"[DEBUG] CommunicationManager.Connect: State = Connecting...");
                    await Task.Delay(10); // Wait a bit before checking again
                    break;
                case WebSocketState.Open:
                    Console.WriteLine($"[DEBUG] CommunicationManager.Connect: State = Open!");
                    _ = ReceiveAsync(); // Start receiving in background
                    waiting = false;
                    break;
                case WebSocketState.CloseSent:
                    Console.WriteLine($"[DEBUG] CommunicationManager.Connect: State = CloseSent");
                    waiting = false;
                    break;
                case WebSocketState.CloseReceived:
                    Console.WriteLine($"[DEBUG] CommunicationManager.Connect: State = CloseReceived");
                    waiting = false;
                    break;
                case WebSocketState.Closed:
                    Console.WriteLine($"[DEBUG] CommunicationManager.Connect: State = Closed");
                    waiting = false;
                    break;
                case WebSocketState.Aborted:
                    Console.WriteLine($"[DEBUG] CommunicationManager.Connect: State = Aborted");
                    waiting = false;
                    break;
            }
        }

        if (Client.State != WebSocketState.Open)
        {
            Console.WriteLine($"[DEBUG] CommunicationManager.Connect: Failed to connect, final state: {Client.State}");
            throw new InvalidOperationException($"WebSocket failed to connect, state: {Client.State}");
        }

        Console.WriteLine($"[DEBUG] CommunicationManager.Connect: Connection established!");

        return this;
    }

    public async Task<CommunicationManager> SendMessageAsync(string message)
    {
        ArgumentNullException.ThrowIfNull(Client, nameof(Client));

        var data = Encoding.UTF8.GetBytes(message);

        var bufferToSend = new ArraySegment<byte>(data);

        await Client.SendAsync(bufferToSend, WebSocketMessageType.Text, true, CancellationToken.None);

        return this;
    }

    private async Task ReceiveAsync()
    {
        ArgumentNullException.ThrowIfNull(Client, nameof(Client));

        var buffer = new byte[receiveBufferSize];

        while (true)
        {
            var receivedBuffer = new ArraySegment<byte>(buffer);

            var result = await Client.ReceiveAsync(
                receivedBuffer,
                CancellationToken.None
            );

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await Client.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    string.Empty,
                    CancellationToken.None
                );

                break;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            OnReceiveMessage?.Invoke(message);

            if (!result.EndOfMessage) continue;
        }
    }

    public CommunicationManager SetBufferSize(int size)
    {
        receiveBufferSize = size;

        return this;
    }

    public async Task<CommunicationManager> Close()
    {
        ArgumentNullException.ThrowIfNull(Client, nameof(Client));

        await Client.CloseAsync(
            WebSocketCloseStatus.NormalClosure,
            string.Empty,
            CancellationToken.None
        );

        return this;
    }
}
