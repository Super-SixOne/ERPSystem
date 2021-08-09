using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSystem.Models
{
    public class Customer
    {
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string Streetaddress { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public bool VIP { get; set; }
    }
}
