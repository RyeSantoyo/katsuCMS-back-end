using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Product SKU")]
        public string ProductCode { get; set; } = string.Empty;
        [Display(Name = "Product Name")]
        public string? ProductName { get; set; }
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
        [Display(Name = "Price")]
        [Column(TypeName = "decimal(18,2)")]
         public decimal Price { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }


        /* FKS */
        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        public ProductCategory Category { get; set; } = null!;

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;

        [ForeignKey("UnitId")]
        public int UnitId { get; set; }
        public Unit Unit { get; set; } = null!;

        // public List<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>();

        // public ICollection<PurchaseOrderDetail>? PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();

        // public int? UnitConversionId { get; set; }
        // public UnitConversion? UnitConversion { get; set; }

        // public int ConvertedQuantity => UnitConversion != null ? ConvertedQuantity * UnitConversion.Factor : (int?)null;
    }
}