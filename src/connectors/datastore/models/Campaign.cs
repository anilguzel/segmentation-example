namespace connectors.datastore.models
{
    public class Campaign
    {
        public int Id { get; set; }
        public int Frequency { get; set; }
        public int TotalOrderCount { get; set; }
        public bool IsActive { get; set; }
    }
}

