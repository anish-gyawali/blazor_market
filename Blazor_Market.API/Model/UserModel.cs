using Microsoft.AspNetCore.Identity;

namespace Blazor_Market.API.Model
{
    public class UserModel:IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
