using System.ComponentModel.DataAnnotations;
using DangPhatFlex.Web.Areas.Admin.Controllers;
using DangPhatFlex.Web.Data;
using DangPhatFlex.Web.Models;
using DangPhatFlex.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class ProductCategoriesAdminTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProductCategoriesAdminTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Index_WithoutLogin_RedirectsToLogin()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var response = await client.GetAsync("/Admin/ProductCategories");

        Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location!.ToString());
    }

    // NOTE ON TEST STRATEGY: an HTTP-level login flow (POST /Identity/Account/Login) is not
    // exercisable here because the app's default Identity UI login page 500s — it's missing a
    // _LoginPartial view (a separate, pre-existing gap in the app unrelated to this bug, and
    // outside the scope of the Task 13 slug fix). So these tests invoke the controller action
    // directly, reproducing exactly what ASP.NET Core's model binder does before the action
    // runs (running [Required]/[MaxLength] validation and populating ModelState from it), which
    // is precisely the mechanism the bug lived in: ModelState.IsValid was evaluated before the
    // blank-Slug fallback ran, so the "leave blank to auto-generate" case always failed
    // validation instead of ever reaching the auto-generate code.

    [Fact]
    public async Task Create_WithBlankSlug_AutoGeneratesSlugFromName()
    {
        using var context = CreateInMemoryContext();
        var controller = CreateController(context);

        var model = new ProductCategory { Name = "Khớp Nối Mềm Test Blank Slug", Slug = "" };
        PopulateModelStateFromDataAnnotations(controller, model);
        Assert.False(controller.ModelState.IsValid); // sanity check: blank Slug fails [Required] before the fix's fallback runs

        var result = await controller.Create(model);

        Assert.IsType<RedirectToActionResult>(result);

        var created = await context.ProductCategories.SingleOrDefaultAsync(c => c.Name == model.Name);
        Assert.NotNull(created);
        Assert.False(string.IsNullOrWhiteSpace(created!.Slug));
        Assert.Equal("khop-noi-mem-test-blank-slug", created.Slug);
    }

    [Fact]
    public async Task Edit_WithBlankSlug_AutoGeneratesSlugFromName()
    {
        using var context = CreateInMemoryContext();
        var existing = new ProductCategory { Name = "Old Name", Slug = "old-slug" };
        context.ProductCategories.Add(existing);
        await context.SaveChangesAsync();
        context.Entry(existing).State = EntityState.Detached;

        var controller = CreateController(context);

        var model = new ProductCategory { Id = existing.Id, Name = "Khớp Nối Mềm Edited", Slug = "" };
        PopulateModelStateFromDataAnnotations(controller, model);
        Assert.False(controller.ModelState.IsValid);

        var result = await controller.Edit(existing.Id, model);

        Assert.IsType<RedirectToActionResult>(result);

        var updated = await context.ProductCategories.SingleAsync(c => c.Id == existing.Id);
        Assert.False(string.IsNullOrWhiteSpace(updated.Slug));
        Assert.Equal("khop-noi-mem-edited", updated.Slug);
    }

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

    private static ProductCategoriesController CreateController(AppDbContext context)
    {
        var controller = new ProductCategoriesController(context, new SlugService());
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return controller;
    }

    /// <summary>
    /// Reproduces the ModelState population that ASP.NET Core's model binder performs from data
    /// annotations (e.g. [Required] on ProductCategory.Slug) before an action method runs, so
    /// this unit test faithfully recreates the "blank Slug submission" ModelState the real
    /// pipeline would produce.
    /// </summary>
    private static void PopulateModelStateFromDataAnnotations(ControllerBase controller, object model)
    {
        var validationContext = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, validationContext, results, validateAllProperties: true);

        foreach (var result in results)
        {
            var memberNames = result.MemberNames.Any() ? result.MemberNames : new[] { string.Empty };
            foreach (var memberName in memberNames)
            {
                controller.ModelState.AddModelError(memberName, result.ErrorMessage ?? "Invalid value.");
            }
        }
    }
}
