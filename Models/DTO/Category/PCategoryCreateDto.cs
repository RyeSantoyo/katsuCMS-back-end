using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models.DTO.Category
{
    public class PCategoryCreateDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}