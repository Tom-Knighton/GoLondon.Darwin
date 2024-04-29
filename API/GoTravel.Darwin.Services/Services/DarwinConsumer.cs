using System.IO.Compression;
using System.Xml.Serialization;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using GoTravel.Darwin.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RttiPPT;

namespace GoTravel.Darwin.Services.Services;

public class DarwinConsumer(IConfiguration config, ILogger<DarwinConsumer> log) : IDarwinConsumer
{
    public DateTime LastReceived { get; set; }
    
    private bool _running;
    private Task _runLoop;
    private CancellationTokenSource _cts;

    private string _url;
    private string _user, _pass, _topic;
    
    public async Task Start()
    {
        var darwinConfig = config.GetSection("Darwin").GetSection("PushPort");
        _url = $"{darwinConfig["Host"]}:{darwinConfig["Port"]}?connection.watchTopicAdvisories=false";
        _user = darwinConfig["Username"];
        _pass = darwinConfig["Pass"];
        _topic = darwinConfig["Topic"];

        if (_running) throw new ApplicationException("Push Port receiver tried to start twice");

        _cts = new CancellationTokenSource();
        _runLoop = Task.Run((Func<Task>)Run);
        _runLoop.ConfigureAwait(false);
        log.LogDebug("Darwin PP Consumer Started");
    }

    private async Task Run()
    {
        var ct = _cts.Token;

        IMessageConsumer consumer = null;
        try
        {
            var connectionFactory = new ConnectionFactory(_url);
            var connection = await connectionFactory.CreateConnectionAsync(_user, _pass);
            var session = await connection.CreateSessionAsync();
            var topic = await session.GetTopicAsync(_topic);
            consumer = await session.CreateConsumerAsync(topic, "MessageType = 'TS' OR MessageType = 'SC'");
            consumer.Listener += async (message) => await Consume(message, ct);
            await connection.StartAsync();
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Failed to connect to PP");
            consumer?.CloseAsync();
        }
    }
//dqSq4kkGAsKJK98LwAha
    private async Task Consume(IMessage message, CancellationToken ct)
    {
        try
        {
            if (message is null) throw new NullReferenceException(nameof(message));
            if (message is not ActiveMQBytesMessage byteMessage) throw new Exception("Message is not a byte message");
            
            using var compressedStream = new MemoryStream(byteMessage.Content);
            await using var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            if (gzipStream is null) throw new NullReferenceException(nameof(gzipStream));
            
            var serializer = new XmlSerializer(typeof(Pport));
            var data = serializer.Deserialize(gzipStream);
            if (data is not Pport pportData) throw new NullReferenceException(nameof(data));

            if (pportData.Item is not PportUR uR) throw new Exception("Timetable received");

            // var rids = uR.TS.Select(u => u.rid).ToList();
            // rids.AddRange(uR.deactivated?.Select(u => u.rid));
            
            LastReceived = pportData.ts;
            log.LogDebug("Received PP message with timestamp: {Timestamp}", LastReceived);
            GC.Collect();
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Failed to consume message: {@Message}", message);
        }
        finally
        {
            await message?.AcknowledgeAsync();
        }
    }
}