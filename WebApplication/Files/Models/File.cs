using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Files.Models
{
    public class File
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default;
        public string ExternalId { get; set; } = default;
        public string Name { get; set; } = default;
        public string Path { get; set; } = default;
        public string Extension { get; set; } = default;

        public string UserExternalId { get; set; } = default;

        public File() { }

        public File(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException($"Parameter {nameof(path)} passed into {nameof(File)} constructor is null or empty.");

            ExternalId = Guid.NewGuid().ToString();
            Path = path;
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            Extension = System.IO.Path.GetExtension(path);
        }
        public File(int id, string path) : this(path)
        {
            Id = id;
        }
    }
}
