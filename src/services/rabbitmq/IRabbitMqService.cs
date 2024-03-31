public interface IRabbitMqService
{
    void SendMessage(string queueName, string message);
}