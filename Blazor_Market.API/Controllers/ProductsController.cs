﻿using Microsoft.AspNetCore.Mvc;
using Blazor_Market.API.DbContext;
using Blazor_Market.API.Model.ProductModel;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Blazor_Market.API.Static;

namespace Blazor_Market.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductsController(ApplicationDbContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult GetAllProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _dataContext.Set<Product>().Include(p => p.User).AsQueryable();
                query = query.Skip((page - 1) * pageSize).Take(pageSize);

                var data = query
                    .Select(product => new ProductGetDto
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        ProductDescription = product.ProductDescription,
                        ProductPrice = product.ProductPrice,
                        ProductImageFileName = product.ProductImageFileName!,
                        ProductAddedDate = product.ProductAddedDate,
                        ProductStatus = product.ProductStatus,
                        FirstName = product.User!.FirstName
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
            var product = _dataContext.Set<Product>().Include(u => u.User)
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
                ProductImageFileName = product.ProductImageFileName!,
                ProductAddedDate = product.ProductAddedDate,
                ProductStatus = product.ProductStatus,
                UserId=product.User!.Id
            };

            return Ok(productDto);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto createProduct)
        {
            try
            {
                if (string.IsNullOrEmpty(createProduct.ProductName))
                {
                    return BadRequest("Product Name required");
                }
                if (createProduct.ProductPrice < 0)
                {
                    return BadRequest("Product price must be positive");
                }
                var productToCreate = new Product
                {
                    UserId = createProduct.UserId,
                    ProductName = createProduct.ProductName,
                    ProductDescription = createProduct.ProductDescription,
                    ProductPrice = createProduct.ProductPrice,
                    ProductImageFileName = createProduct.ProductImageFileName,
                    ProductAddedDate = createProduct.ProductAddedDate,
                    ProductStatus = createProduct.ProductStatus,
                };
                
                _dataContext.Set<Product>().Add(productToCreate);
                await _dataContext.SaveChangesAsync();

                var productCreated = new ProductGetDto
                {
                    ProductId = productToCreate.ProductId,
                    ProductName = createProduct.ProductName,
                    ProductDescription = createProduct.ProductDescription,
                    ProductPrice = createProduct.ProductPrice,
                    ProductImageFileName = createProduct.ProductImageFileName,
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
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto updateDto)
        {
            var product = _dataContext.Set<Product>().Include(p => p.User)
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            if (updateDto.ProductImageFileName != null && updateDto.ProductImageFileName.Length > 0)
            {
                product.ProductImageFileName = updateDto.ProductImageFileName;
            }
            else
            {
                product.ProductImageFileName = product.ProductImageFileName;
            }

            product.ProductName = updateDto.ProductName;
            product.ProductPrice = updateDto.ProductPrice;
            product.ProductDescription = updateDto.ProductDescription;
            product.ProductAddedDate = updateDto.ProductAddedDate;
            product.ProductStatus = updateDto.ProductStatus;

            await _dataContext.SaveChangesAsync();

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