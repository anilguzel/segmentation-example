using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var Configuration = new ConfigurationBuilder()  
   .AddJsonFile("appsettings.json")
   .Build();

builder.Services.AddConnectors(new connectors.Configuration
{
    ElasticsearchUri = Configuration["Elasticsearch:Uri"],
    RabbitMq = Configuration.GetSection("RabbitMq").Get<connectors.RabbitMq>(),
    RedisEndpoints = new List<RedLockNet.SERedis.Configuration.RedLockEndPoint>
            {
                new System.Net.DnsEndPoint(Configuration["RedLock:HostName"], Convert.ToInt16(Configuration["RedLock:Port"])),
            }
});
builder.Services.AddServices();



var app = builder.Build();


# region rabbitMQ initializing
var rabbitMqConnector = app.Services.GetRequiredService<IRabbitMqConnector>();
var channel = rabbitMqConnector.GetChannel();


channel.ExchangeDeclare(exchange: "orderCreated", type: ExchangeType.Direct);
channel.QueueDeclare(queue: "orderCreated", durable: true, exclusive: false, autoDelete: false, arguments: null);
channel.QueueBind(queue: "orderCreated", exchange: "orderCreated", routingKey: "orderCreated");
#endregion

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

