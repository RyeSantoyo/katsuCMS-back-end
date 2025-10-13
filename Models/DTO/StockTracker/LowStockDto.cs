using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models.DTO.StockTracker
{
    public class LowStockDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal? ReorderLevel { get; set; }
        public bool IsLowstock => Quantity < (ReorderLevel ?? 0);
    }
}