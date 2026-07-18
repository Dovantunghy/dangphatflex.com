using DangPhatFlex.Web.Data;
using DangPhatFlex.Web.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class AppDbContextTests
{
    private static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;
        var context = new AppDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public void CanInsertProductWithVariantAndCategory()
    {
        using var context = CreateInMemoryContext();

        var category = new ProductCategory { Name = "Khớp nối mềm inox", Slug = "khop-noi-mem-inox" };
        var product = new Product
        {
            ProductCategory = category,
            Name = "DP25UB",
            Slug = "dp25ub"
        };
        product.Variants.Add(new ProductVariant
        {
            ProductCode = "DP25UB-15-700",
            InletOutlet = "1x1/2",
            InstallLengthMm = 700
        });

        context.Products.Add(product);
        context.SaveChanges();

        var saved = context.Products
            .Include(p => p.Variants)
            .Include(p => p.ProductCategory)
            .Single();

        Assert.Equal("DP25UB", saved.Name);
        Assert.Single(saved.Variants);
        Assert.Equal("khop-noi-mem-inox", saved.ProductCategory!.Slug);
    }
}
