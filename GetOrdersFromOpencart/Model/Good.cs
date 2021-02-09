using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetOrdersFromOpencart.Model
{
    public class Good
    {
        public int GoodId { get; set; }
        public string Articul { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string ForWho { get; set; }
        public string Large { get; set; }
    }
}
