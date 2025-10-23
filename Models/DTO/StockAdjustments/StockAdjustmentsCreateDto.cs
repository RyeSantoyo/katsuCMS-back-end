using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models.DTO.StockAdjustments
{
    public class StockAdjustmentsCreateDto
    {
        public int InventoryStockId { get; set; }
        public decimal AdjustedQuantity { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}