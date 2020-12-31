using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class File
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public int UserId { get; set; }
    }
}
