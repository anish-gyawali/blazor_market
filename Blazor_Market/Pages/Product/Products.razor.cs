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
        [Parameter] public int? productId { get; set; }
        private string? errorMessage;
        private string? selectedImageFile ;
        private bool IsUpdatingProduct = false;
        private int? productIdToUpdate;

        [Inject]
        private AuthenticationStateProvider? authenticationStateProvider { get; set; }

        [Inject]
        private NavigationManager? NavigationManager { get; set; }
        [Inject]
        private IProductService? ProductService { get; set; }
        private CreateProductDto createProductModel = new CreateProductDto(); 
        private ProductUpdateDto updateProductModel = new ProductUpdateDto();
        private string ProductName
        {
            get => IsUpdatingProduct ? updateProductModel.ProductName! : createProductModel.ProductName!;
            set
            {
                if (IsUpdatingProduct)
                {
                    updateProductModel.ProductName = value;
                }
                else
                {
                    createProductModel.ProductName = value;
                }
            }
        }

        private string ProductDescription
        {
            get => IsUpdatingProduct ? updateProductModel.ProductDescription! : createProductModel.ProductDescription!;
            set
            {
                if (IsUpdatingProduct)
                {
                    updateProductModel.ProductDescription = value;
                }
                else
                {
                    createProductModel.ProductDescription = value;
                }
            }
        }

        private decimal ProductPrice
        {
            get => IsUpdatingProduct ? updateProductModel.ProductPrice : createProductModel.ProductPrice;
            set
            {
                if (IsUpdatingProduct)
                {
                    updateProductModel.ProductPrice = value;
                }
                else
                {
                    createProductModel.ProductPrice = value;
                }
            }
        }

        private ProductStatus ProductStatus
        {
            get => IsUpdatingProduct ? updateProductModel.ProductStatus : createProductModel.ProductStatus;
            set
            {
                if (IsUpdatingProduct)
                {
                    updateProductModel.ProductStatus = value;
                }
                else
                {
                    createProductModel.ProductStatus = value;
                }
            }
        }

        private DateTime ProductAddedDate
        {
            get => IsUpdatingProduct ? updateProductModel.ProductAddedDate : createProductModel.ProductAddedDate;
            set
            {
                if (IsUpdatingProduct)
                {
                    updateProductModel.ProductAddedDate = value;
                }
                else
                {
                    createProductModel.ProductAddedDate = value;
                }
            }
        }
        protected override async Task OnInitializedAsync()
        {
            var uri = NavigationManager?.ToAbsoluteUri(NavigationManager.Uri);

            if (IsProductUpdateRoute(uri!))
            {
                await HandleProductDetailRoute(uri!);
            }
            else if (IsProductUpdateRoute(uri!))
            {
                await LoadProductDetailsForUpdate();
            }
            else
            {
                await HandleProductsRoute();
            }
        }
        private bool IsProductUpdateRoute(Uri uri)
        {
            return uri.AbsolutePath.StartsWith("/products/") && uri.Segments.Length == 3;
        }



        private async Task HandleProductDetailRoute(Uri uri)
        {
            var segments = uri.AbsolutePath.Split('/');
            if (segments.Length == 3 && int.TryParse(segments[2], out var productId))
            {
                productIdToUpdate = productId;
                await LoadProductDetailsForUpdate();
            }
        }

        private async Task HandleProductsRoute()
        {
            await ProductService!.GetAllProducts();
        }

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
                        ProductAddedDate = DateTime.Now, 
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

        private async Task<bool> IsUserAuthorizedToUpdate(string? productUserId)
        {
            var authenticationState = await authenticationStateProvider!.GetAuthenticationStateAsync();
            var user = authenticationState.User;
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId == productUserId;
        }

        private async Task UpdateProduct(int productId)
        {
            try
            {
                IsUpdatingProduct = true;

                updateProductModel = new ProductUpdateDto
                {
                    ProductId = productId,
                    ProductName = ProductName,
                    ProductDescription = ProductDescription,
                    ProductPrice = ProductPrice,
                    ProductStatus = ProductStatus,
                    ProductAddedDate = DateTime.Now,
                };

                // Check if there is an image update
                if (!string.IsNullOrEmpty(selectedImageFile) && IsBase64String(selectedImageFile))
                {
                    byte[] imageBytes = Convert.FromBase64String(selectedImageFile);
                    updateProductModel.ProductImageFileName = imageBytes;
                }
                await ProductService!.UpdateProduct(updateProductModel);

                IsUpdatingProduct = false;
                updateProductModel = new ProductUpdateDto();
                selectedImageFile = null;
                errorMessage = null;

                NavigationManager!.NavigateTo("/");
            }
            catch (Exception ex)
            {
                errorMessage = $"An error occurred: {ex.Message}";
            }
        }



        private async Task LoadProductDetailsForUpdate()
        {
            if (productIdToUpdate.HasValue)
            {
                var product = await ProductService!.GetProductById(productIdToUpdate.Value);
                if (!await IsUserAuthorizedToUpdate(product.UserId))
                {
                    errorMessage = "You are not authorized to update this product.";
                    return;
                }
                if (product != null)
                {
                    IsUpdatingProduct = true;
                    updateProductModel = new ProductUpdateDto
                    {
                        ProductName = product.ProductName,
                        ProductDescription = product.ProductDescription,
                        ProductPrice = product.ProductPrice,
                        ProductImageFileName = product.ProductImageFileName,
                        ProductAddedDate = product.ProductAddedDate,
                        ProductStatus = product.ProductStatus,
                    };
                }
                else
                {
                    errorMessage = "Product not found.";
                }
            }
        }

    }
}
