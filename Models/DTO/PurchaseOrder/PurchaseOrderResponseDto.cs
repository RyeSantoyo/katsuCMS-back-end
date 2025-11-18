using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using katsuCMS_backend.Models.DTO.PurchaseOrderDetailDto;

namespace katsuCMS_backend.Models.DTO.PurchaseOrder
{
    public class PurchaseOrderResponseDTO
    {
        public string PONumber { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public PurchaseOrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<PurchaseOrderDetailResponseDTO> OrderDetails { get; set; } = new();
    }
}