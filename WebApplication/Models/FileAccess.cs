using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class FileAccess
    {
        public enum FileAccessType { Owner = 0, Reader = 1 }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public FileAccessType AccessType { get; set; }

        [ForeignKey("File")]
        public int FileId { get; set; }
        public Models.File File { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
