using System.ComponentModel.DataAnnotations;

namespace Blazor_Market.API.Model.AccountModel
{
    public class RegisterModel
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [Phone]
        public string? PhoneNumber { get; set; }

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        
        [Required]
        public string? Role { get; set; }
    }
}
