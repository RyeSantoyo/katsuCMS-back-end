using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models.DTO.PurchaseOrderDetailDto
{
    public class PurchaseOrderDetailResponseDTO
    {
        public string ProductName { get; set; } = string.Empty; // human-readable
        public string UnitName { get; set; } = string.Empty;    // human-readable
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
    }
}