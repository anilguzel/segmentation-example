using System;
using RabbitMQ.Client;

public interface IRabbitMqConnector
{
    IModel GetChannel();
}