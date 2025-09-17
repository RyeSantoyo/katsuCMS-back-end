using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}