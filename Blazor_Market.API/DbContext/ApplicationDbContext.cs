using Blazor_Market.API.Model;
using Blazor_Market.API.Model.ProductModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blazor_Market.API.DbContext
{
    public class ApplicationDbContext:IdentityDbContext<UserModel>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        { 
        }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>()
                .Property(p => p.ProductPrice)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Product>()
                .HasOne(u => u.User)
                .WithMany(p => p.Products)
                .HasForeignKey(u => u.UserId)
                .IsRequired();
        }
    }
}
