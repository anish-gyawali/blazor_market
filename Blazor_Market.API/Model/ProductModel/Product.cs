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
        public string? ProductImageBase64 { get; set; }
        public decimal ProductPrice { get; set; }
        public string? ProductDescription { get; set; }
        public DateTime ProductAddedDate { get; set; }
        public ProductStatus ProductStatus { get; set; }
    }

    public class CreateProductDto
    {
        public string? ProductName { get; set; }
        public string? ProductImageBase64 { get; set; }
        public decimal ProductPrice { get; set; }
        public string? ProductDescription { get; set; }
        public DateTime ProductAddedDate { get; set; }
        public ProductStatus ProductStatus { get; set; }

    }
    public class ProductUpdateDto
    {
        public string? ProductName { get; set; }
        public string? ProductImageBase64 { get; set; }
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

