    public class OrderAggregate
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderLineValueObject> OrderLines { get; set; }
        public CustomerEntity Customer { get; set; }

        public OrderAggregate()
        {
            Id = Guid.NewGuid();
            OrderDate = DateTime.Now;
            Customer = new CustomerEntity();
        }
    }

    public class CustomerEntity
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }


        public CustomerEntity() => Id = Guid.NewGuid();
    }

    public class OrderLineValueObject
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

