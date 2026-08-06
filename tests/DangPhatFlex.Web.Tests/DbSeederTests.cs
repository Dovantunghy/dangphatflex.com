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

    [Fact]
    public void NewsBacklogSeeder_Creates24ArticlesWithUniqueSlugsCoverImagesAndKeywords()
    {
        using var context = CreateInMemoryContext();
        var slugService = new SlugService();

        DbSeeder.SeedNewsArticles(context, slugService);
        NewsBacklogSeeder.Seed(context, slugService);

        var backlog = context.NewsArticles
            .Where(a => a.PublishedAt >= new DateTime(2026, 7, 20))
            .ToList();

        Assert.Equal(24, backlog.Count);
        Assert.Equal(24, backlog.Select(a => a.Slug).Distinct().Count());
        Assert.Equal(27, context.NewsArticles.Select(a => a.Slug).Distinct().Count());
        Assert.All(backlog, a => Assert.False(string.IsNullOrWhiteSpace(a.CoverImageUrl)));
        Assert.All(backlog, a => Assert.Contains("## ", a.Content));
        Assert.All(backlog, a =>
        {
            var haystack = a.Title + " " + a.Content;
            Assert.Contains("ống mềm nối đầu phun", haystack);
        });

        // 12 days x 2 posts (08:00 and 17:00), 20/07-31/07/2026.
        var days = backlog.Select(a => a.PublishedAt.Date).Distinct().OrderBy(d => d).ToList();
        Assert.Equal(12, days.Count);
        Assert.All(days, d => Assert.Equal(2, backlog.Count(a => a.PublishedAt.Date == d)));
    }

    [Fact]
    public void NewsBacklogSeeder_IsIdempotent()
    {
        using var context = CreateInMemoryContext();
        var slugService = new SlugService();

        DbSeeder.SeedNewsArticles(context, slugService);
        NewsBacklogSeeder.Seed(context, slugService);
        NewsBacklogSeeder.Seed(context, slugService);

        Assert.Equal(27, context.NewsArticles.Count());
    }

    [Fact]
    public void NewsBacklogSeeder2_Creates12ArticlesWithUniqueSlugsCoverImagesAndKeywords()
    {
        using var context = CreateInMemoryContext();
        var slugService = new SlugService();

        DbSeeder.SeedNewsArticles(context, slugService);
        NewsBacklogSeeder.Seed(context, slugService);
        NewsBacklogSeeder2.Seed(context, slugService);

        var batch2 = context.NewsArticles
            .Where(a => a.PublishedAt >= new DateTime(2026, 8, 1))
            .ToList();

        Assert.Equal(12, batch2.Count);
        Assert.Equal(12, batch2.Select(a => a.Slug).Distinct().Count());
        Assert.Equal(39, context.NewsArticles.Select(a => a.Slug).Distinct().Count());
        Assert.All(batch2, a => Assert.False(string.IsNullOrWhiteSpace(a.CoverImageUrl)));
        Assert.All(batch2, a => Assert.Contains("## ", a.Content));
        Assert.All(batch2, a =>
        {
            var haystack = a.Title + " " + a.Content;
            Assert.Contains("ống mềm nối đầu phun", haystack);
        });

        // 6 days x 2 posts (08:00 and 17:00), 01/08-06/08/2026.
        var days = batch2.Select(a => a.PublishedAt.Date).Distinct().OrderBy(d => d).ToList();
        Assert.Equal(6, days.Count);
        Assert.All(days, d => Assert.Equal(2, batch2.Count(a => a.PublishedAt.Date == d)));
    }

    [Fact]
    public void NewsBacklogSeeder2_IsIdempotent()
    {
        using var context = CreateInMemoryContext();
        var slugService = new SlugService();

        DbSeeder.SeedNewsArticles(context, slugService);
        NewsBacklogSeeder.Seed(context, slugService);
        NewsBacklogSeeder2.Seed(context, slugService);
        NewsBacklogSeeder2.Seed(context, slugService);

        Assert.Equal(39, context.NewsArticles.Count());
    }
}
