namespace POC.OrderingService.Query.Contracts.ReadModels.Orders
{
    public class OrderItemReadModel
    {
        public Guid Id { get; set; }
        public OrderReadModel Order { get; set; }
        public Guid OrderId { get; set; }
        public string ProductName { get; set; }
        public double Count { get; set; }
        public string Unit { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}