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
        ArgumentNullException.ThrowIfNull(url, nameof(url));

        ArgumentNullException.ThrowIfNull(Client, nameof(Client));

        await Client.ConnectAsync(new Uri(url), CancellationToken.None);

        var waiting = true;

        while (waiting)
        {
            switch (Client.State)
            {
                case WebSocketState.None:
                    waiting = false;
                    break;
                case WebSocketState.Connecting:
                    break;
                case WebSocketState.Open:
                    new Thread(async () => await ReceiveAsync()).Start();
                    waiting = false;
                    break;
                case WebSocketState.CloseSent:
                    waiting = false;
                    break;
                case WebSocketState.CloseReceived:
                    waiting = false;
                    break;
                case WebSocketState.Closed:
                    waiting = false;
                    break;
                case WebSocketState.Aborted:
                    waiting = false;
                    break;
            }
        }

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
