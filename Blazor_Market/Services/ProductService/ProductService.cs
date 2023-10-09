
using Blazor_Market.API.Model.ProductModel;

namespace Blazor_Market.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public List<ProductGetDto> Products { get ; set; }=new List<ProductGetDto>();

        public async Task CreateProduct(CreateProductDto createProduct)
        {
            var response = await _httpClient.PostAsJsonAsync("api/products", createProduct);
            response.EnsureSuccessStatusCode();
        }
        public async Task GetAllProducts()
        {
            var result = await _httpClient.GetFromJsonAsync<List<ProductGetDto>>("api/products");
            if (result != null)
                Products = result;
        }
        public async Task<ProductGetDto> GetProductById(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<ProductGetDto>($"api/products/{id}");
            return response!;
        }
        public async Task UpdateProduct(ProductUpdateDto updateProduct)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/products/{updateProduct.ProductId}", updateProduct);
            response.EnsureSuccessStatusCode();
        }
        public async Task DeleteProduct(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/products/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
