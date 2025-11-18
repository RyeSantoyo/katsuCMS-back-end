using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using katsuCMS_backend.Models.DTO.PurchaseOrderDetailDto;

namespace katsuCMS_backend.Models.DTO.PurchaseOrder
{
    public class PurchaseOrderCreateDto
    {
        public string PONumber { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public PurchaseOrderStatus Status  { get; set; } = PurchaseOrderStatus.Pending;
        public decimal TotalAmount { get; set; }
        public List<PurchaseOrderDetailDTO> PurchaseOrderDetails { get; set; } = new();
    }
    
   public class PurchaseOrderUpdateDto : PurchaseOrderCreateDto
    {
        public new PurchaseOrderStatus Status  { get; set; } = PurchaseOrderStatus.Pending;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

}