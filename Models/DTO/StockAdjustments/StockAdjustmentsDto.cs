using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models.DTO.StockAdjustments
{
    public class StockAdjustmentsDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public decimal PreviousQuantity { get; set; }
        public decimal AdjustedQuantity { get; set; }
        public string AdjustmentType { get; set; } = string.Empty; 
        public string Reason { get; set; } = string.Empty;
        public DateTime AdjustmentDate { get; set; } = DateTime.Now;
    }
}