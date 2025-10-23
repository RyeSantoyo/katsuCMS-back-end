using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models
{
    public class StockAdjustments
    {
        public int Id { get; set; }

        public int InventoryStockId { get; set; }
        public InventoryStock InventoryStock { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public decimal PreviousQuantity { get; set; }
        public decimal AdjustedQuantity { get; set; }
        public string AdjustmentType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime AdjustmentDate { get; set; } = DateTime.Now;
    }
}