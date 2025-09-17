using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models
{
    public class StockLogs
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int QuantityChange { get; set; }
        public decimal UnitPrice { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime DateLogged { get; set; }

        public int? UnitsId { get; set; }
        public Unit? Units { get; set; }

        public string? Receiver { get; set; }

        public int? StockPullOutId { get; set; }
        public StockPullOut? StockPullOut { get; set; }

        public StockType StockType { get; set; }
    }

    public enum StockType
    {
        In,
        Out,
        Adjustment
    }
}