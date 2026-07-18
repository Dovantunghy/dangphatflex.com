using DangPhatFlex.Web.Data;
using DangPhatFlex.Web.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class DbSeederTests
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
    public void Seed_CreatesCompanyInfoCategoryProductAndVariants()
    {
        using var context = CreateInMemoryContext();
        var slugService = new SlugService();

        DbSeeder.Seed(context, slugService);

        Assert.Single(context.CompanyInfos);
        Assert.True(context.ProductCategories.Any(c => c.Slug == "khop-noi-mem-inox"));
        var product = context.Products.Include(p => p.Variants).Single();
        Assert.True(product.Variants.Count >= 4);
        Assert.Contains(product.Variants, v => v.ProductCode == "DP25UB-15-700");
    }

    [Fact]
    public void Seed_IsIdempotent()
    {
        using var context = CreateInMemoryContext();
        var slugService = new SlugService();

        DbSeeder.Seed(context, slugService);
        DbSeeder.Seed(context, slugService);

        Assert.Single(context.CompanyInfos);
    }
}
