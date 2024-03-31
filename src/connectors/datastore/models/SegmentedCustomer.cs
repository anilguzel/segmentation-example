using System;
namespace connectors.datastore.models
{
	public class SegmentedCustomer
	{
		public SegmentedCustomer(Guid customerId, int campaignId, List<Guid> orderIds = null)
		{
			CustomerId = customerId;
			CampaignId = campaignId;
			OrderIds = orderIds;
			CreatedDateTime = DateTime.Now;
		}
		public Guid CustomerId { get; }
		public int CampaignId { get; }
		public DateTime CreatedDateTime { get; }
		public List<Guid> OrderIds { get; }
	}
}

