using System;
using System.Collections.Generic;

namespace ERPSystem.Models
{
    public class OrderHeader
    {
        public string OrderNo { get; set; }
        public string? CustomerNo { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? Status { get; set; }
        public string? Carrier { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerCity { get; set; }
        public string? CustomerCountry { get; set; }
        public int Sequence { get; set; }
        List<OrderItem> Items { get; set; }

    }
}
