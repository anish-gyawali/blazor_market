using Blazor_Market.API.Model.ProductModel;
using Blazor_Market.Services.ProductService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.Security.Claims;

namespace Blazor_Market.Pages.Product
{
    [Authorize]
    public partial class Products
    {
        private string? errorMessage;
        private string? selectedImageFile ;
        [Inject]
        private AuthenticationStateProvider? authenticationStateProvider { get; set; }

        [Inject]
        private NavigationManager? NavigationManager { get; set; }
        [Inject]
        private IProductService? ProductService { get; set; }
        private ProductGetDto? selectedProduct;
        private CreateProductDto createProductModel = new CreateProductDto(); 
        private ProductUpdateDto updateProductModel = new ProductUpdateDto();

        private async Task HandleImageUpload(InputFileChangeEventArgs e)
        {
            var fileList = e.GetMultipleFiles().FirstOrDefault();
            if (fileList != null)
            {
                var buffer = new byte[fileList.Size];
                using (var stram=fileList.OpenReadStream())
                {
                    await stram.ReadAsync(buffer);
                }
                selectedImageFile=Convert.ToBase64String(buffer);
            }
        }
        private void ValidatePrice(ChangeEventArgs e)
        {
            if (decimal.TryParse(e.Value?.ToString(), out var price))
            {
                createProductModel.ProductPrice = price;
            }
            else
            {
                errorMessage = "Please enter a valid price.";
            }
        }

        private bool IsBase64String(string base64)
        {
            try
            {
                Convert.FromBase64String(base64);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        protected override async Task OnInitializedAsync()
        {
            await ProductService!.GetAllProducts();
        }
        private async Task GetProduct(int productId)
        {
            selectedProduct = await ProductService!.GetProductById(productId);
        }
        private async Task CreateProduct()
        {
            var authenticationState = await authenticationStateProvider!.GetAuthenticationStateAsync();
            var user = authenticationState.User;

            // Perform validation checks before creating the product
            if (string.IsNullOrWhiteSpace(createProductModel.ProductName))
            {
                errorMessage = "Product Name is required.";
                return;
            }
            if (createProductModel.ProductPrice <= 0)
            {
                errorMessage = "Product price must be positive.";
                return;
            }
            errorMessage = null;

            // Handle the image file
            byte[]? imageBytes = null;

            // Check if selectedImageFile is not null and contains a base64 string
            if (!string.IsNullOrEmpty(selectedImageFile) && IsBase64String(selectedImageFile))
    {
                // Convert the base64 string back to bytes
                imageBytes = Convert.FromBase64String(selectedImageFile);
            }


            try
            {
                if (user.Identity!.IsAuthenticated)
                {
                    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                    // If all checks passed, create the product
                    await ProductService!.CreateProduct(new CreateProductDto
                    {
                        ProductName = createProductModel.ProductName,
                        ProductDescription = createProductModel.ProductDescription,
                        ProductPrice = createProductModel.ProductPrice,
                        ProductImageFileName = imageBytes,
                        ProductAddedDate = DateTime.Now, // Set the current date
                        ProductStatus = createProductModel.ProductStatus,
                        UserId = userId
                    });

                    await ProductService!.GetAllProducts();
                    createProductModel = new CreateProductDto();

                    NavigationManager!.NavigateTo("/");
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"An error occurred: {ex.Message}";
            }
        }

        private async Task UpdateProduct()
        {
            await ProductService!.UpdateProduct(updateProductModel);
            await ProductService!.GetAllProducts();
            updateProductModel = new ProductUpdateDto();
        }
        
    }
}
