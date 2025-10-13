using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models.DTO.StockTracker
{
    public class ProductReorderUpdateDto
    {
        public int ProductId { get; set; }
        public decimal? ReorderLevel { get; set; }
        public decimal? PreferredStockLevel { get; set; }
    }
}