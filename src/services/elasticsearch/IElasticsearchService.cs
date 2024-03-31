using Nest;
using System.Reflection.Metadata;
using services.models;
using connectors.datastore.models;

public interface IElasticsearchService
{
    Task<bool> CheckOrderCountFrequencyAsync(Guid customerId, int frequency = 30, int orderCount = 1);
    Task<bool> IndexDocumentAsync<T>(T document) where T : class;
    Task<List<T>> GetFilteredDocuments<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query) where T : class;
    Task<List<Campaign>> GetActiveCampaignsAsync();

    Task<bool> CheckIsSegmentedCustomerAsync(Guid customerId, int campaignId);
}