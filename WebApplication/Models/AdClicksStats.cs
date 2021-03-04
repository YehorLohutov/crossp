using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class AdClicksStats
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default;

        public DateTime Date { get; set; } = default;

        public int Number { get; set; } = default;

        [ForeignKey("Ad")]
        public int AdId { get; set; } = default;
        public Ad Ad { get; set; } = default;

        public AdClicksStats() { }
        public AdClicksStats(DateTime date, Ad ad) 
        {
            Date = date.Date;
            AdId = ad.Id;
        }
        public AdClicksStats(int id, DateTime date, Ad ad) : this(date, ad)
        {
            Id = id;
        }
    }
}
