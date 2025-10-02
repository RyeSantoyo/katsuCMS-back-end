using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models
{
    public class StockPullOut
    {
        public int Id { get; set; }
        public int? StoreId { get; set; }
        public Store? Store { get; set; }
        [Required]
        public string Reason { get; set; } = string.Empty;
        public DateTime PullOutDate { get; set; }
         public ICollection<StockPullOutDetail> StockPullOutDetails { get; set; } = new List<StockPullOutDetail>();
    }
}