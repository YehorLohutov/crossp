using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class ProjectAccess
    {
        public enum ProjectAccessType { Owner = 0, Editor = 1 }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public ProjectAccessType AccessType { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
