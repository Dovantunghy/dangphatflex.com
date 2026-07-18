using DangPhatFlex.Web.Areas.Admin.Controllers;
using DangPhatFlex.Web.Areas.Admin.Models;
using DangPhatFlex.Web.Data;
using DangPhatFlex.Web.Models;
using DangPhatFlex.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class ProductsAdminTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProductsAdminTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Index_WithoutLogin_RedirectsToLogin()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var response = await client.GetAsync("/Admin/Products");

        Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location!.ToString());
    }

    // Same rationale as ProductCategoriesAdminTests: the Identity UI login page 500s (a
    // pre-existing, out-of-scope gap), so these tests invoke the controller action directly to
    // reproduce exactly what ASP.NET Core's model binder / validation pipeline does before the
    // action runs.

    [Fact]
    public async Task Create_WithBlankSlug_AutoGeneratesSlugFromName()
    {
        using var context = CreateInMemoryContext();
        var category = new ProductCategory { Name = "Ống Mềm", Slug = "ong-mem" };
        context.ProductCategories.Add(category);
        await context.SaveChangesAsync();

        var controller = CreateController(context);

        var model = new ProductFormViewModel
        {
            ProductCategoryId = category.Id,
            Name = "Ống Thủy Lực Test Blank Slug",
            Slug = "",
            MainImageAlt = "ảnh sản phẩm"
        };
        PopulateModelStateFromDataAnnotations(controller, model);

        var result = await controller.Create(model);

        Assert.IsType<RedirectToActionResult>(result);

        var created = await context.Products.SingleOrDefaultAsync(p => p.Name == model.Name);
        Assert.NotNull(created);
        Assert.False(string.IsNullOrWhiteSpace(created!.Slug));
        Assert.Equal("ong-thuy-luc-test-blank-slug", created.Slug);
    }

    [Fact]
    public async Task Edit_WithBlankSlug_AutoGeneratesSlugFromName()
    {
        using var context = CreateInMemoryContext();
        var category = new ProductCategory { Name = "Ống Mềm", Slug = "ong-mem" };
        context.ProductCategories.Add(category);
        var existing = new Product
        {
            Name = "Old Name",
            Slug = "old-slug",
            ProductCategory = category,
            MainImageAlt = "old alt"
        };
        context.Products.Add(existing);
        await context.SaveChangesAsync();
        context.Entry(existing).State = EntityState.Detached;

        var controller = CreateController(context);

        var model = new ProductFormViewModel
        {
            Id = existing.Id,
            ProductCategoryId = category.Id,
            Name = "Ống Thủy Lực Edited",
            Slug = "",
            MainImageAlt = "ảnh sản phẩm"
        };
        PopulateModelStateFromDataAnnotations(controller, model);

        var result = await controller.Edit(existing.Id, model);

        Assert.IsType<RedirectToActionResult>(result);

        var updated = await context.Products.SingleAsync(p => p.Id == existing.Id);
        Assert.False(string.IsNullOrWhiteSpace(updated.Slug));
        Assert.Equal("ong-thuy-luc-edited", updated.Slug);
    }

    [Fact]
    public async Task Create_WithoutMainImageAlt_FailsValidation()
    {
        using var context = CreateInMemoryContext();
        var category = new ProductCategory { Name = "Ống Mềm", Slug = "ong-mem" };
        context.ProductCategories.Add(category);
        await context.SaveChangesAsync();

        var controller = CreateController(context);

        var model = new ProductFormViewModel
        {
            ProductCategoryId = category.Id,
            Name = "Sản phẩm không có alt text",
            MainImageAlt = ""
        };
        PopulateModelStateFromDataAnnotations(controller, model);
        Assert.False(controller.ModelState.IsValid);

        var result = await controller.Create(model);

        Assert.IsType<ViewResult>(result);
        var noneCreated = await context.Products.AnyAsync(p => p.Name == model.Name);
        Assert.False(noneCreated);
    }

    [Fact]
    public async Task Create_WithDisallowedMainImageExtension_FailsValidationAndDoesNotSave()
    {
        using var context = CreateInMemoryContext();
        var category = new ProductCategory { Name = "Ống Mềm", Slug = "ong-mem" };
        context.ProductCategories.Add(category);
        await context.SaveChangesAsync();

        var fileUploadServiceMock = new Mock<IFileUploadService>();
        var controller = CreateController(context, fileUploadServiceMock);

        var model = new ProductFormViewModel
        {
            ProductCategoryId = category.Id,
            Name = "Sản phẩm ảnh không hợp lệ",
            MainImageAlt = "ảnh sản phẩm",
            MainImage = CreateFormFile("malicious.exe", "application/octet-stream")
        };
        PopulateModelStateFromDataAnnotations(controller, model);

        var result = await controller.Create(model);

        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey(nameof(model.MainImage)));

        var noneCreated = await context.Products.AnyAsync(p => p.Name == model.Name);
        Assert.False(noneCreated);
        fileUploadServiceMock.Verify(
            s => s.SaveAsync(It.IsAny<IFormFile>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WithDisallowedDatasheetExtension_FailsValidationAndDoesNotSave()
    {
        using var context = CreateInMemoryContext();
        var category = new ProductCategory { Name = "Ống Mềm", Slug = "ong-mem" };
        context.ProductCategories.Add(category);
        await context.SaveChangesAsync();

        var fileUploadServiceMock = new Mock<IFileUploadService>();
        var controller = CreateController(context, fileUploadServiceMock);

        var model = new ProductFormViewModel
        {
            ProductCategoryId = category.Id,
            Name = "Sản phẩm datasheet không hợp lệ",
            MainImageAlt = "ảnh sản phẩm",
            DatasheetPdf = CreateFormFile("script.html", "text/html")
        };
        PopulateModelStateFromDataAnnotations(controller, model);

        var result = await controller.Create(model);

        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey(nameof(model.DatasheetPdf)));

        var noneCreated = await context.Products.AnyAsync(p => p.Name == model.Name);
        Assert.False(noneCreated);
        fileUploadServiceMock.Verify(
            s => s.SaveAsync(It.IsAny<IFormFile>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WithAllowedImageAndPdfExtensions_SucceedsAndUploadsFiles()
    {
        using var context = CreateInMemoryContext();
        var category = new ProductCategory { Name = "Ống Mềm", Slug = "ong-mem" };
        context.ProductCategories.Add(category);
        await context.SaveChangesAsync();

        var fileUploadServiceMock = new Mock<IFileUploadService>();
        fileUploadServiceMock
            .Setup(s => s.SaveAsync(It.IsAny<IFormFile>(), "products"))
            .ReturnsAsync("/uploads/products/fake.png");
        fileUploadServiceMock
            .Setup(s => s.SaveAsync(It.IsAny<IFormFile>(), "datasheets"))
            .ReturnsAsync("/uploads/datasheets/fake.pdf");
        var controller = CreateController(context, fileUploadServiceMock);

        var model = new ProductFormViewModel
        {
            ProductCategoryId = category.Id,
            Name = "Sản phẩm hợp lệ",
            MainImageAlt = "ảnh sản phẩm",
            MainImage = CreateFormFile("photo.PNG", "image/png"),
            DatasheetPdf = CreateFormFile("sheet.pdf", "application/pdf")
        };
        PopulateModelStateFromDataAnnotations(controller, model);

        var result = await controller.Create(model);

        Assert.IsType<RedirectToActionResult>(result);

        var created = await context.Products.SingleOrDefaultAsync(p => p.Name == model.Name);
        Assert.NotNull(created);
        Assert.Equal("/uploads/products/fake.png", created!.MainImageUrl);
        Assert.Equal("/uploads/datasheets/fake.pdf", created.DatasheetPdfUrl);
        fileUploadServiceMock.Verify(s => s.SaveAsync(It.IsAny<IFormFile>(), "products"), Times.Once);
        fileUploadServiceMock.Verify(s => s.SaveAsync(It.IsAny<IFormFile>(), "datasheets"), Times.Once);
    }

    private static IFormFile CreateFormFile(string fileName, string contentType)
    {
        var content = System.Text.Encoding.UTF8.GetBytes("dummy content");
        var stream = new MemoryStream(content);
        return new FormFile(stream, 0, content.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
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

    private static ProductsController CreateController(AppDbContext context, Mock<IFileUploadService>? fileUploadServiceMock = null)
    {
        fileUploadServiceMock ??= new Mock<IFileUploadService>();
        var controller = new ProductsController(context, new SlugService(), fileUploadServiceMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return controller;
    }

    private static void PopulateModelStateFromDataAnnotations(ControllerBase controller, object model)
    {
        var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(model);
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        System.ComponentModel.DataAnnotations.Validator.TryValidateObject(model, validationContext, results, validateAllProperties: true);

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
