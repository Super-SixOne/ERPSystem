namespace ERPSystem.Models
{
    public class OrderItem
    {
        public string OrderNo { get; set; }
        public string OrderPos { get; set; }
        public string? Material { get; set; }
        public string? Status { get; set; }
        public int? TargetQuantity { get; set; }
        public int? NOKQuantity { get; set; }
    }
}
