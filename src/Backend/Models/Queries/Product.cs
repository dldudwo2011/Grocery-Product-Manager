using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Models.Queries
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Discount { get; set; }
        public string UnitSize { get; set; }
        public int CategoryId { get; set; }
        public bool Taxable { get; set; }
        public byte[] Photo { get; set; }
    }
}
