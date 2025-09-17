using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models
{
    public class Unit
    {
        public int Id { get; set; }

        public string UnitName { get; set; } = string.Empty;
        // public int BaseUnitId { get; set; }
        // public Unit? BaseUnit { get; set; }
        // public double? ConversionFactor { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
        //public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
    }
}