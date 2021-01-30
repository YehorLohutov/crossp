using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default;
        public string Login { get; set; } = default;
        public string Password { get; set; } = default;
        public List<File> Files { get; set; } = new List<File>();
        public List<Project> Projects { get; set; } = new List<Project>();

        public User() { }
        public User(string login, string password)
        {
            Login = login;
            Password = password;
        }
        public User(int id, string login, string password) : this(login, password)
        {
            Id = id;
        }
    }
}
