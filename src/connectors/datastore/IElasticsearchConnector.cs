using Nest;

public interface IElasticsearchConnector
{
    ElasticClient GetClient();
}