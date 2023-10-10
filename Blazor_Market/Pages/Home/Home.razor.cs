using Blazor_Market.API.Model.ProductModel;
using Blazor_Market.Services.ProductService;
using Microsoft.AspNetCore.Components;

namespace Blazor_Market.Pages.Home
{
    public partial class Home
    {
        private List<ProductGetDto> Products { get; set; } = new List<ProductGetDto>();
        [Inject]
        private IProductService? ProductService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var products = await ProductService!.GetAllProducts();
            Products = products;
        }

    }
}
