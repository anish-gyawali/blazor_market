namespace Blazor_Market.API.Model.ProductModel
{
    public class Product
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public byte[]? ProductImageFileName { get; set; }
        public decimal ProductPrice { get; set; }
        public string? ProductDescription { get; set; }
        public DateTime ProductAddedDate { get; set; }
        public ProductStatus ProductStatus { get; set; }

        public UserModel? User { get; set; }
        public string? UserId { get; set; }
    }
    public class ProductGetDto 
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public byte[]? ProductImageFileName { get; set; }
        public decimal ProductPrice { get; set; }
        public string? ProductDescription { get; set; }
        public DateTime ProductAddedDate { get; set; }
        public ProductStatus ProductStatus { get; set; }
        public string? FirstName { get; set; }

        public string ProductImageBase64 =>
            ProductImageFileName != null ? $"data:image/png;base64,{Convert.ToBase64String(ProductImageFileName)}" : "";
    }

    public class CreateProductDto
    {
        public CreateProductDto()
        {
            ProductAddedDate = DateTime.Now; 
        }
        public string? UserId { get; set; }
        public string? ProductName { get; set; }
        public byte[]? ProductImageFileName { get; set; }
        public decimal ProductPrice { get; set; }
        public string? ProductDescription { get; set; }
        public DateTime ProductAddedDate { get; set; }
        public ProductStatus ProductStatus { get; set; }

    }
    public class ProductUpdateDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public byte[]? ProductImageFileName { get; set; }
        public decimal ProductPrice { get; set; }
        public string? ProductDescription { get; set; }
        public DateTime ProductAddedDate { get; set; }
        public ProductStatus ProductStatus { get; set; }
    }
    public enum ProductStatus
    {
        New,
        Used
    }


}


