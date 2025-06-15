namespace POC.OrderingService.Query.Abstraction.Dtos.Orders
{
    public class OrderItemReadModelDto
    {
        public Guid Id { get;  set; }
        public string ProductName { get;  set; }
        public double Count { get;  set; }
        public string Unit { get;  set; }
        public decimal Amount { get;  set; }
        public string Currency { get;  set; }
    }
}