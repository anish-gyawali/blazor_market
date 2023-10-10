using Blazor_Market.API.Model.ProductModel;

namespace Blazor_Market.Services.ProductService
{
    public interface IProductService
    {
        List<ProductGetDto> Products { get; set; }
        Task<List<ProductGetDto>> GetAllProducts();
        Task<ProductGetDto> GetProductById(int id);
        Task CreateProduct(CreateProductDto createProduct);
        Task UpdateProduct(ProductUpdateDto updateProduct);
        Task DeleteProduct(int id);
    }
}