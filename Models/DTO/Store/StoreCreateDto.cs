using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models.DTO.Store
{
    public class StoreCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
    }
}