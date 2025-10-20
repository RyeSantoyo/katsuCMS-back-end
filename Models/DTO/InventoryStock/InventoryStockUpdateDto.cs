using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models.DTO.InventoryStock
{
    public class InventoryStockUpdateDto
    {
        public decimal? Quantity { get; set; }
        public decimal? ReorderLevel { get; set; }
        public decimal? PreferredStockLevel { get; set; }
    }
}