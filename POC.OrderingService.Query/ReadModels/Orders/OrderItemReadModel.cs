namespace POC.OrderingService.Query.ReadModels.Orders
{
    public class OrderItemReadModel
    {
        public Guid Id { get; private set; }
        public OrderItemReadModel Order { get; private set; }
        public Guid OrderId { get; private set; }
        public string ProductName { get; private set; }
        public double Count { get; private set; }
        public string Unit { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
    }
}