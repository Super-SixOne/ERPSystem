using System;

namespace ERPSystem.Models
{
    public class OrderHeader
    {
        public string OrderNo { get; set; }
        public string? CustomerNo { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? Status { get; set; }
        public string? Carrier { get; set; }
        public int Sequence { get; set; }
        public OrderItemCollection Items { get; set; }

        public OrderHeader()
        {
            Items = new OrderItemCollection();
        }
    }
}
