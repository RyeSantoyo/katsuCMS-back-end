using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace katsuCMS_backend.Models
{
    public class ProductSupplier
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int SupplierId { get; set; }

        [JsonIgnore] // Prevent serialization issues
        public Product Product { get; set; } = null!;

        [JsonIgnore]
        public Supplier Supplier { get; set; } = null!;
    }
}