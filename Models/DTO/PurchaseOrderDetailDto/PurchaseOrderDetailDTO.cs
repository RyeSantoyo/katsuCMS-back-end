using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using katsuCMS_backend.Models.DTO.PurchaseOrder;

namespace katsuCMS_backend.Models.DTO.PurchaseOrderDetailDto
{
    public class PurchaseOrderDetailDTO
    {
        public string ProductName { get; set; } = string.Empty;
        public string PONumber { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public int UnitId { get; set; }
    }
}