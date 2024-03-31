using System;
using Elasticsearch;
using Nest;

namespace services.models
{
	public class SegmentationOrder
    {
		public SegmentationOrder(OrderAggregate order)
		{
			CustomerId = order.Customer.Id;
			OrderedDate = order.OrderDate;
			OrderDetail = order;
		}

		public Guid CustomerId { get; }
		public DateTime OrderedDate { get; }
		public OrderAggregate OrderDetail { get; }
	}
}

