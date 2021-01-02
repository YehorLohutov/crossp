using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class Ad
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Url { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }  
        public Project Project { get; set; }

        [ForeignKey("File")]
        public int FileId { get; set; }
        public Models.File File { get; set; }
    }
}
