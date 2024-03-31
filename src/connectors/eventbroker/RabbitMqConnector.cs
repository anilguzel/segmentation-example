using System;
using RabbitMQ.Client;

public class RabbitMqConnector : IRabbitMqConnector
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqConnector(string hostName, string userName, string password, int port)
    {
        var factory = new ConnectionFactory() { HostName = hostName, UserName = userName, Password = password, Port = port };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public IModel GetChannel()
    {
        return _channel;
    }
}
