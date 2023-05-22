using System.Net.Sockets;
using System.Text;

namespace KitX.Loader.CSharp;

public class CommunicationManager
{
    private readonly TcpClient? client;

    private readonly Thread? receiveThread;

    private bool stillReceiving = true;

    private int receiveBufferSize = 1024 * 1024 * 10; // 10MB

    public CommunicationManager()
    {
        client = new();

        receiveThread = new(ReceiveMessage);
    }

    public CommunicationManager Connect(string address)
    {
        var splited = address.Split(':');

        var ipv4 = splited[0];

        if (!int.TryParse(splited[1], out var port))
            throw new ArgumentException("Bad port number.", nameof(address));

        client?.Connect(ipv4, port);

        receiveThread?.Start();

        return this;
    }

    public CommunicationManager SendMessage(string message)
    {
        var stream = client?.GetStream();

        if (stream is null) return this;

        var data = Encoding.UTF8.GetBytes(message);

        try
        {
            stream?.Write(data, 0, data.Length);

            stream?.Flush();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            Console.WriteLine(ex.StackTrace);

            stream?.Close();

            stream?.Dispose();
        }

        return this;
    }

    private void ReceiveMessage()
    {
        if (client is null) return;

        var stream = client?.GetStream();

        if (stream is null) return;

        var buffer = new byte[receiveBufferSize];  //  Default 10 MB buffer

        try
        {
            while (stillReceiving)
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
                    stream?.Dispose();

                    break;
                }
            }

            stream?.Close();

            stream?.Dispose();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);

            stream?.Close();
            stream?.Dispose();

            client?.Close();
            client?.Dispose();
        }
    }

    public CommunicationManager SetBufferSize(int size)
    {
        receiveBufferSize = size;

        return this;
    }

    public CommunicationManager Stop()
    {
        stillReceiving = false;

        return this;
    }
}
