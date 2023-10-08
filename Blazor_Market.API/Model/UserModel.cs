using Blazor_Market.API.Model.ProductModel;
using Microsoft.AspNetCore.Identity;

namespace Blazor_Market.API.Model
{
    public class UserModel:IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Navigation property to represent the products added by the user
        public ICollection<Product>? Products { get; set; }
    }
}
