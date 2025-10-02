using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public decimal Quantity { get; set; }

    [Display(Name = "Reorder Level")]
    public int ReorderLevel { get; set; } = 0; // Minimum stock before triggering low-stock alert

    [Display(Name = "Last Updated")]
    public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}