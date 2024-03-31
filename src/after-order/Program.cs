using System.Reflection;
using after_order;
using connectors.datastore.models;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        #region configurations
        var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        var configurationBuilder = new ConfigurationBuilder();

        if (environmentName == "Development")
            configurationBuilder.AddJsonFile("appsettings.Development.json", optional: false);
        else
            configurationBuilder.AddJsonFile("appsettings.json", optional: false);
        var Configuration = configurationBuilder.Build();
        #endregion

        #region logging
        var elasticSinkConfiguration = new ElasticsearchSinkOptions(new Uri(Configuration["Elasticsearch:Uri"]))
        {
            AutoRegisterTemplate = true,
            IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
        };

        Log.Logger = new LoggerConfiguration()
		.Enrich.FromLogContext()
        .Enrich.WithExceptionDetails()
        .Enrich.WithMachineName()
		.WriteTo.Debug()
		.WriteTo.Console()
		.WriteTo.Elasticsearch(elasticSinkConfiguration)
		.Enrich.WithProperty("Environment", environmentName)
		.ReadFrom.Configuration(Configuration)
		.CreateLogger();
        #endregion

        #region solution dependencies
        services.AddConnectors(new connectors.Configuration
        {
            ElasticsearchUri = Configuration["Elasticsearch:Uri"],
            RabbitMq = Configuration.GetSection("RabbitMq").Get<connectors.RabbitMq>(),
            RedisEndpoints = new List<RedLockNet.SERedis.Configuration.RedLockEndPoint>
            {
                new System.Net.DnsEndPoint(Configuration["RedLock:HostName"], Convert.ToInt16(Configuration["RedLock:Port"])),
            }
        });

        services.AddServices();
        #endregion

        services.AddHostedService<Worker>();
    })
    .UseSerilog()
    .Build();

#region campaign feed
var elasticsearchService = host.Services.GetRequiredService<IElasticsearchService>();
var result = await elasticsearchService.IndexDocumentAsync<Campaign>(new Campaign { Id = 1, Frequency = 60, IsActive = true, TotalOrderCount = 2 });
if (!result) throw new Exception();
#endregion

host.Run();

