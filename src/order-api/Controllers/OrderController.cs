using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace order_api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IElasticsearchService _elasticsearchService;

    public OrderController(IElasticsearchService elasticsearchService, IRabbitMqService rabbitMqService)
    {
        _elasticsearchService = elasticsearchService;
        _rabbitMqService = rabbitMqService;
    }

    /// <summary>
    /// create a dummy order to feed rabbitmq
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public ActionResult Order()
    {
        var testOrder = new OrderAggregate();
        var orderAsJson = JsonConvert.SerializeObject(testOrder);
        _rabbitMqService.SendMessage("orderCreated", orderAsJson);

        return Ok(orderAsJson);
    }



    [HttpGet]
    public ActionResult Segmentation(Guid customerId)
    {
        // kullanici segmenti karsiliyor mu diye gorebiliyoruz.
        return Ok(_elasticsearchService.CheckOrderCountFrequencyAsync(customerId, 60, 2));
    }
}

