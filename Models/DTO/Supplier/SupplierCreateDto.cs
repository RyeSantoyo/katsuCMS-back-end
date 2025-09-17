using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models.DTO.Supplier
{
    public class SupplierCreateDto
    {
        public string SupplierName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
    }
} 