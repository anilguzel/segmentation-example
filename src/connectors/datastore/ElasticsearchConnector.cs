using Nest;

public class ElasticsearchConnector : IElasticsearchConnector
{
    private readonly ElasticClient _client;

    public ElasticsearchConnector(string uri)
    {
        var settings = new ConnectionSettings(new Uri(uri));
        _client = new ElasticClient(settings);
    }

    public ElasticClient GetClient() => _client;
}
