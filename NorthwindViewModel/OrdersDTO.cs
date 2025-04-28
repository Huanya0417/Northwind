namespace NorthwindViewModel
{
    public class OrdersDTO
    {
        public int OrderID { get; set; }

        public string? CustomerID { get; set; }

        public string? CustomerName { get; set; }

        public int? EmployeeID { get; set; }

        public string? EmployeeName { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? RequiredDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public int? ShipVia { get; set; }

        public string? ShipCompany { get; set; }

        public decimal? Freight { get; set; }

        public string? ShipName { get; set; }

        public string? ShipAddress { get; set; }

        public string? ShipCity { get; set; }

        public string? ShipRegion { get; set; }

        public string? ShipPostalCode { get; set; }

        public string? ShipCountry { get; set; }

        public List<OrderDetailsDTO>? orderDetails { get; set; }
    }
}
