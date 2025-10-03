using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models.DTO.Product
{
    public class ProductCreateDto
    {
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int UnitId { get; set; }
        public int SupplierId { get; set; }
        public int CategoryId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<int> SupplierIds { get; set; } = new();
        public List<string> SupplierNames { get; set; } = new();
    }
    

    public class ProductUpdateDto : ProductCreateDto
    {
        // Inherits all properties from ProductCreateDto
    }
}