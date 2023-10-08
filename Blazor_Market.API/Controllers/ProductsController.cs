using Microsoft.AspNetCore.Mvc;
using Blazor_Market.API.DbContext;
using Blazor_Market.API.Model.ProductModel;

namespace Blazor_Market.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _dataContext;
        public ProductsController(ApplicationDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public IActionResult GetAllProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _dataContext.Set<Product>().AsQueryable();
                query = query.Skip((page - 1) * pageSize).Take(pageSize);

                var data = query
                    .Select(product => new ProductGetDto
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        ProductDescription = product.ProductDescription,
                        ProductPrice = product.ProductPrice,
                        ProductImageBase64 = Convert.ToBase64String(product.ProductImageFileName!),
                        ProductAddedDate = product.ProductAddedDate,
                        ProductStatus = product.ProductStatus,
                    }).ToList();

                return Ok(data);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving products.");
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _dataContext.Set<Product>()
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var productDto = new ProductGetDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductPrice = product.ProductPrice,
                ProductImageBase64 = Convert.ToBase64String(product.ProductImageFileName!),
                ProductAddedDate = product.ProductAddedDate,
                ProductStatus = product.ProductStatus,
            };

            return Ok(productDto);
        }


        [HttpPost]
        public IActionResult Create([FromBody] CreateProductDto createProduct)
        {
            try
            {
                if (string.IsNullOrEmpty(createProduct.ProductName))
                {
                    return BadRequest("Product Name required");
                }
                if (createProduct.ProductPrice > 0)
                {
                    return BadRequest("Product price must be positive");
                }

                byte[] imageBytes = Convert.FromBase64String(createProduct.ProductImageBase64!);

                var productToCreate = new Product
                {
                    ProductName = createProduct.ProductName,
                    ProductDescription = createProduct.ProductDescription,
                    ProductPrice = createProduct.ProductPrice,
                    ProductImageFileName = imageBytes,
                    ProductAddedDate = createProduct.ProductAddedDate,
                    ProductStatus = createProduct.ProductStatus,
                };

                _dataContext.Set<Product>().Add(productToCreate);
                _dataContext.SaveChanges();

                var productCreated = new ProductGetDto
                {
                    ProductId = productToCreate.ProductId,
                    ProductName = createProduct.ProductName,
                    ProductDescription = createProduct.ProductDescription,
                    ProductPrice = createProduct.ProductPrice,
                    ProductImageBase64 = createProduct.ProductImageBase64,
                    ProductAddedDate = createProduct.ProductAddedDate,
                    ProductStatus = createProduct.ProductStatus,
                };

                return CreatedAtAction(nameof(GetProductById), new { id = productCreated.ProductId }, productCreated);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] ProductUpdateDto updateDto)
        {
            var product = _dataContext.Set<Product>()
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            product.ProductName = updateDto.ProductName;
            product.ProductPrice = updateDto.ProductPrice;
            product.ProductDescription = updateDto.ProductDescription;
            product.ProductAddedDate = updateDto.ProductAddedDate;
            product.ProductStatus = updateDto.ProductStatus;

            if (!string.IsNullOrEmpty(updateDto.ProductImageBase64))
            {
                byte[] imageBytes = Convert.FromBase64String(updateDto.ProductImageBase64);
                product.ProductImageFileName = imageBytes;
            }

            _dataContext.SaveChanges();

            return Ok("Product updated successfully.");
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _dataContext.Set<Product>()
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            _dataContext.Set<Product>().Remove(product);
            _dataContext.SaveChanges();

            return Ok("Product deleted successfully.");
        }



    }
}