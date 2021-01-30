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
        public int Id { get; set; } = default;

        public string Name { get; set; } = ApplicationDBContext.DEFAULT_AD_NAME;

        public string Url { get; set; } = ApplicationDBContext.DEFAULT_AD_URL;

        [ForeignKey("Project")]
        public int ProjectId { get; set; } = default;
        public Project Project { get; set; } = default;

        [ForeignKey("File")]
        public int? FileId { get; set; } = default;
        public Models.File? File { get; set; } = default;

        public Ad() { }

        public Ad(string name, string url, Project project)
        {
            Name = name;
            Url = url;
            ProjectId = project.Id;
        }

        public Ad(int id, string name, string url, Project project) : this(name, url, project)
        {
            Id = id;
        }

        public Ad(string name, string url, Project project, File file) : this(name, url, project)
        {
            FileId = file.Id;
        }
    }
}
