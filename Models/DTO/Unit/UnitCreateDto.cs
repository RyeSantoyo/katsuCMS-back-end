using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models.DTO.Unit
{
    public class UnitCreateDto
    {
        public int Id { get; set; }
        public string UnitName { get; set; } = string.Empty;

    }
}