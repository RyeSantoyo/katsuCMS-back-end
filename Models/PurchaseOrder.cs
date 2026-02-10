using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models
{
    public class PurchaseOrder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string PONumber { get; set; } = string.Empty;
        [Required]
        public int SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; } = null!;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        [Required]
        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Pending;

        public decimal TotalAmount { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();
    }

    public enum PurchaseOrderStatus
    {
        Pending,
        Approved,
        Completed,
        Cancelled
    }
}