using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System;

namespace WebApplication.Models
{
    public class File
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default;
        public string Name { get; set; } = default;
        public string Path { get; set; } = default;
        public string Extension { get; set; } = default;

        [ForeignKey("User")]
        public int UserId { get; set; } = default;
        public User User { get; set; } = default;

        public List<Ad> Ads { get; set; } = new List<Ad>();
        public File() { }
        public File(string path, User user)
        {
            Path = path;
            UserId = user.Id;
            SetFileNameAndExtensionFrom(path);
        }
        public File(int id, string path, User user) : this(path, user)
        {
            Id = id;
        }

        public void SetFileNameAndExtensionFrom(string path)
        {
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            Extension = System.IO.Path.GetExtension(path);
        }
    }
}
