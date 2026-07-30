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
        var product = context.Products.Include(p => p.Variants)
            .First(p => p.Slug == "dang-phat-flex-dp25");
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

    [Fact]
    public void SeedNewsArticles_CreatesThreeArticlesWithUniqueSlugs()
    {
        using var context = CreateInMemoryContext();
        var slugService = new SlugService();

        DbSeeder.SeedNewsArticles(context, slugService);

        var articles = context.NewsArticles.ToList();
        Assert.Equal(3, articles.Count);
        Assert.Equal(3, articles.Select(a => a.Slug).Distinct().Count());
        Assert.All(articles, a => Assert.False(string.IsNullOrWhiteSpace(a.Slug)));
    }

    [Fact]
    public void SeedNewsArticles_IsIdempotent()
    {
        using var context = CreateInMemoryContext();
        var slugService = new SlugService();

        DbSeeder.SeedNewsArticles(context, slugService);
        DbSeeder.SeedNewsArticles(context, slugService);

        Assert.Equal(3, context.NewsArticles.Count());
    }

    [Fact]
    public void UpdateNewsArticleSeoContent_BackfillsCoverImagesOnOldRows()
    {
        using var context = CreateInMemoryContext();
        var slugService = new SlugService();

        // Simulate a production database seeded before cover images/H2 content existed.
        DbSeeder.SeedNewsArticles(context, slugService);
        foreach (var article in context.NewsArticles)
        {
            article.CoverImageUrl = null;
        }
        context.SaveChanges();

        DbSeeder.UpdateNewsArticleSeoContent(context, slugService);

        Assert.Equal(3, context.NewsArticles.Count());
        Assert.All(context.NewsArticles, a => Assert.False(string.IsNullOrEmpty(a.CoverImageUrl)));
        Assert.All(context.NewsArticles, a => Assert.Contains("## ", a.Content));
    }

    [Fact]
    public void UpdateNewsArticleSeoContent_IsIdempotent()
    {
        using var context = CreateInMemoryContext();
        var slugService = new SlugService();

        DbSeeder.SeedNewsArticles(context, slugService);
        DbSeeder.UpdateNewsArticleSeoContent(context, slugService);
        var contentAfterFirstUpdate = context.NewsArticles.Select(a => a.Content).OrderBy(c => c).ToList();

        DbSeeder.UpdateNewsArticleSeoContent(context, slugService);
        var contentAfterSecondUpdate = context.NewsArticles.Select(a => a.Content).OrderBy(c => c).ToList();

        Assert.Equal(contentAfterFirstUpdate, contentAfterSecondUpdate);
    }
}
