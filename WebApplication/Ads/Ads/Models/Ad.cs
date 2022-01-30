using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ads.Models
{
    public class Ad
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default;
        public string ExternalId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string ProjectExternalId { get; set; } = default;
        
        public string FileExternalId { get; set; } = default;

        public Ad() { }
        public Ad(string projectExternalId, string fileExternalId)
        {
            if (string.IsNullOrWhiteSpace(projectExternalId))
                throw new NullReferenceException($"Parameter {nameof(projectExternalId)} passed into {nameof(Ad)} constructor is null or empty.");

            if (string.IsNullOrWhiteSpace(fileExternalId))
                throw new NullReferenceException($"Parameter {nameof(fileExternalId)} passed into {nameof(Ad)} constructor is null or empty.");

            ProjectExternalId = projectExternalId;
            FileExternalId = fileExternalId;
            ExternalId = Guid.NewGuid().ToString();
            Name = "New ad";
            Url = "Empty";
        }
    }
}
