using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MAUI.Models
{
    class LoginModel : IValidatableObject
    {
        public string Login { get; set; } = default;
        public string Password { get; set; } = default;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(Login))
                results.Add(new ValidationResult($"{nameof(Login)} is null, empty or consists white-space characters."));
            if (string.IsNullOrWhiteSpace(Password))
                results.Add(new ValidationResult($"{nameof(Password)} is null, empty or consists white-space characters."));

            return results;
        }
    }
}
