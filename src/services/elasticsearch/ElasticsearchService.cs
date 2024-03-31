using connectors.datastore.models;
using Nest;
using services.models;

public class ElasticsearchService : IElasticsearchService
{
    private readonly  IElasticsearchConnector _elasticsearchConnector;
    private readonly ElasticClient _client;

    public ElasticsearchService(IElasticsearchConnector elasticsearchConnector)
    {
        _elasticsearchConnector = elasticsearchConnector;
        _client = _elasticsearchConnector.GetClient();
    }

    public async Task<bool> CheckOrderCountFrequencyAsync(Guid customerId, int frequency = 30, int orderCount = 1)
    {
        var toDate = DateTime.Now;
        var fromDate = toDate.AddDays(-frequency);

        var result = await _client.CountAsync<SegmentationOrder>(s => s
            .Index(nameof(SegmentationOrder).ToLower())
            .Query(q =>
                 q.Term(te => te
                    .Field(f => f.CustomerId.Suffix("keyword"))
                    .Value(customerId)
                 ) &&
                 q.DateRange(r => r
                      .Field(f => f.OrderedDate)
                      .GreaterThanOrEquals(fromDate)
                      .LessThanOrEquals(toDate))));


        return result.Count == orderCount;
    }

    public async Task<bool> IndexDocumentAsync<T>(T document) where T : class
    {
        var response = await _client.IndexAsync(document, idx => idx.Index(document.GetType().Name.ToLower()));
        return response.IsValid;
    }

    public async Task<IEnumerable<T>> SearchAsync<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query, string indexName) where T : class
    {
        var response = await _client.SearchAsync<T>(s => s
            .Index(indexName)
            .Query(query));

        return response.Documents;
    }

    public async Task<List<T>> GetFilteredDocuments<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query) where T : class
    {
        var searchResponse = await _client.SearchAsync<T>(s => s
            .Index(nameof(T))
            .Query(query));

        return searchResponse.Documents.ToList();
    }

    public async Task<List<Campaign>> GetActiveCampaignsAsync() 
    {
        var searchResponse = await _client.SearchAsync<Campaign>(s => s
            .Index(nameof(Campaign).ToLower())
            .Query(q => q
                .Bool(b => b
                    .Filter(f => f
                        .Term(t => t
                            .Field(ff => ff.IsActive)
                            .Value(true))))));

        return searchResponse.Documents.ToList();

    }

    public async Task<bool> CheckIsSegmentedCustomerAsync(Guid customerId, int campaignId)
    {
        var searchResponse = await _client.SearchAsync<SegmentedCustomer>(s => s
            .Index(nameof(SegmentedCustomer).ToLower())
            .Query(q =>
                 q.Term(cst => cst
                    .Field(f => f.CustomerId.Suffix("keyword"))
                    .Value(customerId)
                 ) &&
                 q.Term(c => c
                      .Field(f => f.CampaignId)
                      .Value(campaignId))));

        // If the relevant campaign can be re-eligible, comparison can be made here with recursive count.
        return searchResponse.Total > default(int);
    }
}
