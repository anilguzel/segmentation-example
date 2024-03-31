using RabbitMQ.Client;

public class RabbitMqService : IRabbitMqService
{
    private readonly IRabbitMqConnector _connector;
    private readonly IModel _channel;

    public RabbitMqService(IRabbitMqConnector connector)
    {
        _connector = connector;
        _channel = _connector.GetChannel();
    }

    public void SendMessage(string queueName, string message)
    {
        var body = System.Text.Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: queueName,
                              routingKey: queueName,
                              basicProperties: null,
                              body: body);
    }
}
