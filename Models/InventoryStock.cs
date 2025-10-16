using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models
{
    public class InventoryStock
    {
        
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    [Required]
    public int UnitId { get; set; }
    public Unit Unit { get; set; } = null!;

    [Display(Name = "Current Quantity")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Quantity { get; set; } = 0;

    [Display(Name = "Reorder Level")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ReorderLevel { get; set; } = 0;

    [Display(Name = "Preferred Stock Level")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PreferredStockLevel { get; set; } = 0;

        // 🕓 Timestamp
    [Display(Name = "Last Updated")]
    public DateTime LastUpdated { get; set; } = DateTime.Now;
    public ICollection<StockLogs>? StockLogs { get; set; }

    }
}