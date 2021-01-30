using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class Project
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default;
        public string Name { get; set; } = ApplicationDBContext.DEFAULT_PROJECT_NAME;
        public string ExternalId { get; set; } = default;

        [ForeignKey("User")]
        public int UserId { get; set; } = default;
        public User User { get; set; } = default;

        public List<Ad> Ads { get; set; } = new List<Ad>();

        public Project() {
            ExternalId = Guid.NewGuid().ToString();
        }
        public Project(User user) : this()
        {
            UserId = user.Id;
        }
        public Project(int id, string name, User user) : this(user)
        {
            Id = id;
            Name = name;
        }
    }
}
