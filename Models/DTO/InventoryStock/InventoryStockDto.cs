using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models.DTO.InventoryStock
{
    public class InventoryStockDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal ReorderLevel { get; set; }
        public decimal PreferredStockLevel { get; set; }
        public bool IsLowstock { get; set; } 
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public List<string> SupplierNames { get; set; } = new();
    }
}