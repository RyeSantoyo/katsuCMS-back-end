using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;

        //public ICollection<Product> Products { get; set; } = new List<Product>();

        public ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>();
        public ICollection<InventoryStock>? InventoryStocks { get; set; }
        // public ICollection<PurchaseOrder> PurchaseOrders { get; set; };
    }
}