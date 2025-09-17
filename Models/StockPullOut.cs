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

        [Required]
        public int StoreId { get; set; }
        public Store? Store { get; set; }
        public string? Reason { get; set; }

        public DateTime PullOutDate { get; set; } = DateTime.Now;
       // public ICollection<StockPullOutDetail> StockPullOutDetails { get; set; } = new List<StockPullOutDetail>();

    }
}