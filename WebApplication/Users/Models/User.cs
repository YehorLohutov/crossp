using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Users.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default;
        public string ExternalId { get; set; } = default;
        public string Login { get; set; } = default;
        public string Password { get; set; } = default;

        public User(string login, string password)
        {
            if (!IsLoginValid(login))
                throw new ArgumentException($"Parameter {nameof(login)} passed into {nameof(User)} constructor is null or empty.");

            if (!IsPasswordValid(password))
                throw new ArgumentException($"Parameter {nameof(password)} passed into {nameof(User)} constructor is null or empty.");

            Login = login;
            Password = password;
            ExternalId = Guid.NewGuid().ToString();
        }

        public User(string login, string password, string externalId) : this(login, password)
        {
            if (!IsExternalIdValid(externalId))
                throw new ArgumentException($"Parameter {nameof(externalId)} passed into {nameof(User)} constructor is null or empty.");

            ExternalId = externalId;
        }

        public User(int id, string login, string password, string externalId) :this(login, password, externalId)
        {
            Id = id;
        }

        private static bool IsLoginValid(string login) => !string.IsNullOrWhiteSpace(login);
        private static bool IsPasswordValid(string password) => !string.IsNullOrWhiteSpace(password);
        private static bool IsExternalIdValid(string externalId) => !string.IsNullOrWhiteSpace(externalId);

    }
}
