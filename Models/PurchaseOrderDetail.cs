using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models
{
    public class PurchaseOrderDetail
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public string PONumber { get; set; } = string.Empty;
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
        public InventoryStock InventoryStock {get;set;} = null!;
        [Required]
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string PurchaseOrderNumber { get; set; } = string.Empty;
        public Product Product { get; set; } = null!;
        public int UnitId { get; set; }
        public Unit Unit { get; set; } = null!;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}