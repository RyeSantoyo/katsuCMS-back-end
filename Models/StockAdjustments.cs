using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models
{
    public class StockAdjustments
    {
        public int Id { get; set; }
        public int InventoryStockId { get; set; }
        public InventoryStock? InventoryStock { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public Product Product { get; set; } = null!;
        public decimal PreviousQuantity { get; set; }
        [Display(Name = "Reorder Level")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ReorderLevel { get; set; } = 0;

        [Display(Name = "Preferred Stock Level")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PreferredStockLevel { get; set; } = 0;
        public decimal AdjustedQuantity { get; set; }
        public string AdjustmentType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime AdjustmentDate { get; set; } = DateTime.Now;
    }
}