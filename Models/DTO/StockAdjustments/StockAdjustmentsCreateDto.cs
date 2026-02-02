using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models.DTO.StockAdjustments
{
    public class StockAdjustmentsCreateDto
    {
        public int InventoryStockId { get; set; }
        public string AdjustmentType { get; set; } = string.Empty;
        public decimal AdjustedQuantity { get; set; }
        public decimal ReorderLevel { get; set; }
        public decimal PreferredStockLevel { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}