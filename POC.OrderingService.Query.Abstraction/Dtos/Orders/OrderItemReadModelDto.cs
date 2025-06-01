namespace POC.OrderingService.Query.Abstraction.Dtos.Orders
{
    public class OrderItemReadModelDto
    {
        public Guid OrderId { get; private set; }
        public string ProductName { get; private set; }
        public double Count { get; private set; }
        public string Unit { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
    }
}