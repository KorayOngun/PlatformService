using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using PlatformService.Settings;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;
        var _rabbimMQSettings = _configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>() ?? throw new ArgumentNullException(nameof(RabbitMQSettings));
        var factory = new ConnectionFactory()
        {
            HostName = _rabbimMQSettings.RabbitMQHost,
            Port = _rabbimMQSettings.RabbitMQPort
        };
        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            Console.WriteLine("--> Connected to MessageBus");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"--> Could not connect to the message bus {exception.Message}");
        }
    }

    public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
        var message = JsonSerializer.Serialize(platformPublishedDto);
        if (_connection.IsOpen)
        {
            Console.WriteLine("--> RabbitMQ Connection Open, Sending Mesaage...");
            SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RabbitMQ connection closed, not sending");
        }
    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);
        Console.WriteLine($"--> We have sent {message}");
    }

    public void Dispose()
    {
        Console.WriteLine("MessageBusDisposed");
        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }

    }
    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> RabbitMQ Connection Shutdown");
    }
}