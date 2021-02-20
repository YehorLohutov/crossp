using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class AdLoadsStats
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default;

        public DateTime DateTime { get; set; } = default;

        [ForeignKey("Ad")]
        public int AdId { get; set; } = default;
        public Ad Ad { get; set; } = default;
    }
}
