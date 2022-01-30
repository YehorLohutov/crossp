using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projects.Models
{
    public class Project
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default;
        public string ExternalId { get; set; } = default;

        public string Name { get; set; } = default;
        public string UserExternalId { get; set; } = default;

        public Project() { }
        public Project(string userExternalId)
        {
            if (string.IsNullOrWhiteSpace(userExternalId))
                throw new NullReferenceException($"Parameter {nameof(userExternalId)} passed into {nameof(Project)} constructor is null or empty.");
            
            UserExternalId = userExternalId;
            ExternalId = Guid.NewGuid().ToString();
        }

        public Project(int id, string name, string userExternalId) : this(userExternalId)
        {
            if (name == default || name.Length == 0)
                throw new ArgumentException($"Parameter {nameof(name)} passed into {nameof(Project)} constructor is null or empty.");

            Id = id;
            Name = name;
        }
    }
}
