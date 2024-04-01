using System.Text;
using connectors.datastore.models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using services.locking;
using services.models;

namespace after_order;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IRabbitMqConnector _rabbitMqConnector;
    private readonly IElasticsearchService _elasticsearchService;
    private readonly IDistributedLockService _distributedLockService;


    public Worker(ILogger<Worker> logger, IRabbitMqConnector rabbitMqConnector, IElasticsearchService elasticsearchService, IDistributedLockService distributedLockService)
    {
        _logger = logger;
        _rabbitMqConnector = rabbitMqConnector;
        _elasticsearchService = elasticsearchService;
        _distributedLockService = distributedLockService;
    }

    // improvements:
    // 1. Requests should be wrapped with sessionId attached to enable tracking session-based logs via Kibana.
    // 2. A basic exception handling has been implemented with try/catch; the code base can be improved.
    // 3. Exception models can be customized.
    // 4. In case of a possible exception situation (line 93), requeue is applied. The segmentationOrder will be duplicated; this can be resolved in the CheckOrderCountFrequencyAsync method with orderId grouping.
    // 5. InteractWithCustomerAsync should not be the responsibility of this service; the operation can be split.
    // 6. Information/exception logging should be detailed.
    // 7. Integration & unit tests.
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = _rabbitMqConnector.GetChannel();
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (_, ea) =>
        {
            try
            {
                await HandleEventAsync(channel, ea);
                channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                channel.BasicReject(ea.DeliveryTag, true);
                _logger.LogError("Event is requeued cause " + ex.Message);
            }
        };
        channel.BasicConsume(queue: "orderCreated", autoAck: false, consumer: consumer);
    }

    private async Task HandleEventAsync(IModel channel, BasicDeliverEventArgs ea)
    {
        var order = GetOrderMessage(ea);
        if (order is null) return;

        using (var redLock = await _distributedLockService.CreateLockAsync($"cst-segmentation-lock-{order.Customer.Id}"))
        {
            if (!redLock.IsAcquired)
            {
                throw new Exception("The customer is already being checked now...");
            }
            else
            {
                var segmentationOrder = new SegmentationOrder(order);
                var result = await _elasticsearchService.IndexDocumentAsync<SegmentationOrder>(segmentationOrder);
                if (!result) throw new Exception($"Error indexing document: {segmentationOrder}");

                var campaigns = await _elasticsearchService.GetActiveCampaignsAsync();

                foreach (var campaign in campaigns)
                {
                    // Check if the customer is already included in the potential campaign.
                    bool isInSegment = await _elasticsearchService.CheckIsSegmentedCustomerAsync(order.Customer.Id, campaign.Id);
                    if (isInSegment) return;

                    bool isCustomerEligibleForSegment = await _elasticsearchService.CheckOrderCountFrequencyAsync(order.Customer.Id, campaign.Frequency, campaign.TotalOrderCount); ;
                    if (!isCustomerEligibleForSegment) return;

                    // Add the customer to the segment
                    var isSuccess = await SegmentCustomerAsync(order.Customer.Id, campaign.Id);
                    if (isSuccess)
                    {
                        await InteractWithCustomerAsync();
                    }
                    else
                    {
                        throw new Exception("An error occurred when indexing the segmented customer.");
                    }
                }

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
        }
    }
    private async Task<bool> SegmentCustomerAsync(Guid customerId, int campaignId)
    {
        var segmentedCustomer = new SegmentedCustomer(customerId, campaignId);
        return await _elasticsearchService.IndexDocumentAsync<SegmentedCustomer>(segmentedCustomer);
    }

    private OrderAggregate GetOrderMessage(BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        _logger.LogInformation("Received message: {0}", message);

        return Newtonsoft.Json.JsonConvert.DeserializeObject<OrderAggregate>(message);
    }

    private async Task InteractWithCustomerAsync()
    {
        // send email or sth through related campaign
        await Task.CompletedTask;
    }
}

