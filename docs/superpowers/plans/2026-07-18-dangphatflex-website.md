# Đăng Phát Flex Website Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build an ASP.NET Core MVC (.NET 8) corporate website for CÔNG TY TNHH CƠ ĐIỆN ĐĂNG PHÁT (brand "Đăng Phát Flex") with public SEO-optimized pages (home, about, products, contact) and an admin panel for managing content.

**Architecture:** Single ASP.NET Core MVC solution, two Areas (`Public` server-rendered pages, `Admin` Identity-protected CRUD), EF Core + SQLite, slug-based SEO URLs, dynamic sitemap.xml, JSON-LD structured data.

**Tech Stack:** .NET 8, ASP.NET Core MVC, EF Core 8 + Microsoft.EntityFrameworkCore.Sqlite, ASP.NET Core Identity, xUnit + Microsoft.AspNetCore.Mvc.Testing for tests.

## Global Constraints

- Target framework: `net8.0`
- Database: SQLite by default (`Data Source=dangphatflex.db`), swappable via connection string / provider later
- Language/content: Vietnamese only, no i18n
- No shopping cart / payment, no blog, no third-party CMS (per spec: [2026-07-18-dangphatflex-website-design.md](../specs/2026-07-18-dangphatflex-website-design.md))
- Brand colors: primary blue `#0B5FA8`, accent gold `#D4A537`, from `Logo Đăng Phát Flex.png`
- Every public content entity (Product, ProductCategory) has `Slug`, `MetaTitle`, `MetaDescription`
- Uploaded images require alt text (enforced in Admin forms)
- Commit after every task using Conventional Commit style messages

---

### Task 1: Solution & project scaffold

**Files:**
- Create: `DangPhatFlex.sln`
- Create: `src/DangPhatFlex.Web/DangPhatFlex.Web.csproj`
- Create: `src/DangPhatFlex.Web/Program.cs`
- Create: `src/DangPhatFlex.Web/appsettings.json`
- Create: `src/DangPhatFlex.Web/appsettings.Development.json`
- Create: `.gitignore`
- Create: `tests/DangPhatFlex.Web.Tests/DangPhatFlex.Web.Tests.csproj`

**Interfaces:**
- Produces: `Program.cs` with a `partial class Program` (required so `Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program>` can reference it in Task 17), Areas routing registered, SQLite `AppDbContext` registered via DI (context type defined in Task 2 — this task only wires up the placeholder `builder.Services.AddDbContext<AppDbContext>()` call, which will fail to compile until Task 2 adds the class; so Task 1's build check only verifies the base MVC scaffold **before** adding that line — see Step 3 note).

- [ ] **Step 1: Create solution and project via CLI**

```bash
cd "d:\Smo Media\Dự án website\dangphatflex.com"
dotnet new sln -n DangPhatFlex
dotnet new mvc -n DangPhatFlex.Web -o src/DangPhatFlex.Web --no-https false
dotnet sln add src/DangPhatFlex.Web/DangPhatFlex.Web.csproj
dotnet new xunit -n DangPhatFlex.Web.Tests -o tests/DangPhatFlex.Web.Tests
dotnet sln add tests/DangPhatFlex.Web.Tests/DangPhatFlex.Web.Tests.csproj
```

- [ ] **Step 2: Add required NuGet packages**

```bash
cd src/DangPhatFlex.Web
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.10
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.10
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.10
cd ../../tests/DangPhatFlex.Web.Tests
dotnet add package Microsoft.AspNetCore.Mvc.Testing --version 8.0.10
dotnet add reference ../../src/DangPhatFlex.Web/DangPhatFlex.Web.csproj
cd ../..
```

- [ ] **Step 3: Enable Areas routing and expose `Program` for tests**

Replace the bottom of `src/DangPhatFlex.Web/Program.cs` (the default `dotnet new mvc` template ends with `app.MapControllerRoute(...)` then `app.Run();`) with:

```csharp
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program { }
```

- [ ] **Step 4: Set connection string**

In `src/DangPhatFlex.Web/appsettings.json`, add:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=dangphatflex.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

- [ ] **Step 5: Add `.gitignore`**

```
bin/
obj/
*.db
*.db-shm
*.db-wal
wwwroot/uploads/*
!wwwroot/uploads/.gitkeep
.vs/
```

- [ ] **Step 6: Build to verify scaffold compiles**

Run: `dotnet build`
Expected: `Build succeeded.` with 0 errors (the `AddDbContext<AppDbContext>` call does not exist yet — do not add it until Task 2).

- [ ] **Step 7: Commit**

```bash
git add DangPhatFlex.sln src/DangPhatFlex.Web tests/DangPhatFlex.Web.Tests .gitignore
git commit -m "chore: scaffold ASP.NET Core MVC solution with Areas routing"
```

---

### Task 2: Domain models, DbContext, and initial migration

**Files:**
- Create: `src/DangPhatFlex.Web/Models/ProductCategory.cs`
- Create: `src/DangPhatFlex.Web/Models/Product.cs`
- Create: `src/DangPhatFlex.Web/Models/ProductVariant.cs`
- Create: `src/DangPhatFlex.Web/Models/Accessory.cs`
- Create: `src/DangPhatFlex.Web/Models/CompanyInfo.cs`
- Create: `src/DangPhatFlex.Web/Models/ContactSubmission.cs`
- Create: `src/DangPhatFlex.Web/Data/AppDbContext.cs`
- Modify: `src/DangPhatFlex.Web/Program.cs`
- Test: `tests/DangPhatFlex.Web.Tests/AppDbContextTests.cs`

**Interfaces:**
- Consumes: nothing (first data layer task)
- Produces: `AppDbContext` with `DbSet<ProductCategory> ProductCategories`, `DbSet<Product> Products`, `DbSet<ProductVariant> ProductVariants`, `DbSet<Accessory> Accessories`, `DbSet<CompanyInfo> CompanyInfos`, `DbSet<ContactSubmission> ContactSubmissions`, constructor `AppDbContext(DbContextOptions<AppDbContext> options)`. Later tasks depend on these exact model property names.

- [ ] **Step 1: Create model classes**

`src/DangPhatFlex.Web/Models/ProductCategory.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace DangPhatFlex.Web.Models;

public class ProductCategory
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    [MaxLength(200)]
    public string? MetaTitle { get; set; }

    [MaxLength(300)]
    public string? MetaDescription { get; set; }

    public List<Product> Products { get; set; } = new();
}
```

`src/DangPhatFlex.Web/Models/Product.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace DangPhatFlex.Web.Models;

public class Product
{
    public int Id { get; set; }

    public int ProductCategoryId { get; set; }
    public ProductCategory? ProductCategory { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    [MaxLength(400)]
    public string? MainImageUrl { get; set; }

    [MaxLength(400)]
    public string? MainImageAlt { get; set; }

    [MaxLength(400)]
    public string? DatasheetPdfUrl { get; set; }

    [MaxLength(100)]
    public string? InnerDiameter { get; set; }

    [MaxLength(100)]
    public string? OuterDiameter { get; set; }

    [MaxLength(100)]
    public string? HoseType { get; set; }

    [MaxLength(50)]
    public string? MaxTemperature { get; set; }

    [MaxLength(100)]
    public string? MaxPressure { get; set; }

    [MaxLength(100)]
    public string? MinBendRadius { get; set; }

    [MaxLength(200)]
    public string? Standards { get; set; }

    [MaxLength(200)]
    public string? MetaTitle { get; set; }

    [MaxLength(300)]
    public string? MetaDescription { get; set; }

    public List<ProductVariant> Variants { get; set; } = new();
    public List<Accessory> Accessories { get; set; } = new();
}
```

`src/DangPhatFlex.Web/Models/ProductVariant.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace DangPhatFlex.Web.Models;

public class ProductVariant
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [Required, MaxLength(50)]
    public string ProductCode { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string InletOutlet { get; set; } = string.Empty;

    public int InstallLengthMm { get; set; }

    public int MaxBends90 { get; set; }

    [MaxLength(50)]
    public string? MinBendRadiusIn { get; set; }

    [MaxLength(50)]
    public string? EquivalentSteelPipeLength { get; set; }
}
```

`src/DangPhatFlex.Web/Models/Accessory.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace DangPhatFlex.Web.Models;

public class Accessory
{
    public int Id { get; set; }

    public int? ProductId { get; set; }
    public Product? Product { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(400)]
    public string? ImageUrl { get; set; }

    [MaxLength(400)]
    public string? ImageAlt { get; set; }

    public int DefaultQuantity { get; set; } = 1;
}
```

`src/DangPhatFlex.Web/Models/CompanyInfo.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace DangPhatFlex.Web.Models;

public class CompanyInfo
{
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string LegalName { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string BrandName { get; set; } = string.Empty;

    [Required, MaxLength(300)]
    public string Tagline { get; set; } = string.Empty;

    public string AboutContent { get; set; } = string.Empty;

    [Required, MaxLength(400)]
    public string Address { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Hotline { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(600)]
    public string? MapEmbedUrl { get; set; }

    [MaxLength(200)]
    public string? CoreValueFast { get; set; }

    [MaxLength(200)]
    public string? CoreValueBest { get; set; }

    [MaxLength(200)]
    public string? CoreValueCompetitivePrice { get; set; }
}
```

`src/DangPhatFlex.Web/Models/ContactSubmission.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace DangPhatFlex.Web.Models;

public class ContactSubmission
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? Email { get; set; }

    [Required]
    public string Message { get; set; } = string.Empty;

    public DateTime SubmittedAt { get; set; }

    public bool IsProcessed { get; set; }
}
```

- [ ] **Step 2: Create `AppDbContext`**

`src/DangPhatFlex.Web/Data/AppDbContext.cs`:

```csharp
using DangPhatFlex.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DangPhatFlex.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<Accessory> Accessories => Set<Accessory>();
    public DbSet<CompanyInfo> CompanyInfos => Set<CompanyInfo>();
    public DbSet<ContactSubmission> ContactSubmissions => Set<ContactSubmission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductCategory>()
            .HasIndex(c => c.Slug).IsUnique();

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Slug).IsUnique();

        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductCategory)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.ProductCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProductVariant>()
            .HasIndex(v => v.ProductCode).IsUnique();

        modelBuilder.Entity<ProductVariant>()
            .HasOne(v => v.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Accessory>()
            .HasOne(a => a.Product)
            .WithMany(p => p.Accessories)
            .HasForeignKey(a => a.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

- [ ] **Step 3: Register `AppDbContext` in `Program.cs`**

In `src/DangPhatFlex.Web/Program.cs`, after `var builder = WebApplication.CreateBuilder(args);` and before `builder.Services.AddControllersWithViews();`, add:

```csharp
using DangPhatFlex.Web.Data;
using Microsoft.EntityFrameworkCore;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
```

- [ ] **Step 4: Create and apply initial migration**

```bash
cd src/DangPhatFlex.Web
dotnet tool install --global dotnet-ef --version 8.* 2>$null
dotnet ef migrations add InitialCreate -o Data/Migrations
dotnet ef database update
cd ../..
```

- [ ] **Step 5: Write integration test verifying context resolves and schema applies**

`tests/DangPhatFlex.Web.Tests/AppDbContextTests.cs`:

```csharp
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
```

- [ ] **Step 6: Run test to verify it passes**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter AppDbContextTests`
Expected: `Passed! - Failed: 0`

- [ ] **Step 7: Commit**

```bash
git add src/DangPhatFlex.Web/Models src/DangPhatFlex.Web/Data src/DangPhatFlex.Web/Program.cs tests/DangPhatFlex.Web.Tests/AppDbContextTests.cs
git commit -m "feat: add domain models, AppDbContext, and initial migration"
```

---

### Task 3: Slug generation service (TDD)

**Files:**
- Create: `src/DangPhatFlex.Web/Services/SlugService.cs`
- Test: `tests/DangPhatFlex.Web.Tests/SlugServiceTests.cs`

**Interfaces:**
- Produces: `public interface ISlugService { string GenerateSlug(string input); }` and `public class SlugService : ISlugService`. `Product`/`ProductCategory` Admin CRUD tasks (13, 14) call `ISlugService.GenerateSlug(name)` when a slug isn't manually provided.

- [ ] **Step 1: Write failing tests**

`tests/DangPhatFlex.Web.Tests/SlugServiceTests.cs`:

```csharp
using DangPhatFlex.Web.Services;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class SlugServiceTests
{
    private readonly SlugService _sut = new();

    [Theory]
    [InlineData("Khớp nối mềm inox", "khop-noi-mem-inox")]
    [InlineData("Đăng Phát Flex DP25UB", "dang-phat-flex-dp25ub")]
    [InlineData("  Nhiều   khoảng trắng  ", "nhieu-khoang-trang")]
    [InlineData("Có Dấu Gạch-Ngang!", "co-dau-gach-ngang")]
    public void GenerateSlug_ProducesUrlSafeLowercaseSlug(string input, string expected)
    {
        var result = _sut.GenerateSlug(input);
        Assert.Equal(expected, result);
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter SlugServiceTests`
Expected: FAIL — `SlugService` does not exist yet.

- [ ] **Step 3: Implement `SlugService`**

`src/DangPhatFlex.Web/Services/SlugService.cs`:

```csharp
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DangPhatFlex.Web.Services;

public interface ISlugService
{
    string GenerateSlug(string input);
}

public class SlugService : ISlugService
{
    public string GenerateSlug(string input)
    {
        var normalized = RemoveDiacritics(input.Trim().ToLowerInvariant());
        normalized = Regex.Replace(normalized, @"[^a-z0-9\s-]", "");
        normalized = Regex.Replace(normalized, @"\s+", "-");
        normalized = Regex.Replace(normalized, @"-+", "-");
        return normalized.Trim('-');
    }

    private static string RemoveDiacritics(string text)
    {
        text = text.Replace('đ', 'd').Replace('Đ', 'D');
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
```

- [ ] **Step 4: Register in DI**

In `src/DangPhatFlex.Web/Program.cs`, add near the `AddDbContext` call:

```csharp
using DangPhatFlex.Web.Services;

builder.Services.AddScoped<ISlugService, SlugService>();
```

- [ ] **Step 5: Run tests to verify they pass**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter SlugServiceTests`
Expected: `Passed! - Failed: 0`

- [ ] **Step 6: Commit**

```bash
git add src/DangPhatFlex.Web/Services/SlugService.cs src/DangPhatFlex.Web/Program.cs tests/DangPhatFlex.Web.Tests/SlugServiceTests.cs
git commit -m "feat: add SlugService for SEO-friendly URL generation"
```

---

### Task 4: Seed data from catalog

**Files:**
- Create: `src/DangPhatFlex.Web/Data/DbSeeder.cs`
- Modify: `src/DangPhatFlex.Web/Program.cs`
- Test: `tests/DangPhatFlex.Web.Tests/DbSeederTests.cs`

**Interfaces:**
- Consumes: `AppDbContext` (Task 2), `ISlugService` (Task 3)
- Produces: `public static class DbSeeder { public static void Seed(AppDbContext context, ISlugService slugService); }`

- [ ] **Step 1: Write failing test**

`tests/DangPhatFlex.Web.Tests/DbSeederTests.cs`:

```csharp
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
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter DbSeederTests`
Expected: FAIL — `DbSeeder` does not exist yet.

- [ ] **Step 3: Implement `DbSeeder`**

`src/DangPhatFlex.Web/Data/DbSeeder.cs`:

```csharp
using DangPhatFlex.Web.Models;
using DangPhatFlex.Web.Services;

namespace DangPhatFlex.Web.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context, ISlugService slugService)
    {
        if (context.CompanyInfos.Any())
            return;

        context.CompanyInfos.Add(new CompanyInfo
        {
            LegalName = "CÔNG TY TNHH CƠ ĐIỆN ĐĂNG PHÁT",
            BrandName = "Đăng Phát Flex",
            Tagline = "Giải pháp khớp nối mềm inox",
            AboutContent = "CÔNG TY TNHH CƠ ĐIỆN ĐĂNG PHÁT là đơn vị chuyên sản xuất, " +
                "nhập khẩu, phân phối và cung cấp các sản phẩm đầu nối, ống kim loại dẻo " +
                "bằng thép không gỉ. Chúng tôi tạo dựng uy tín trên thị trường bằng " +
                "phương châm hoạt động xoay quanh ba yếu tố cơ bản: Nhanh nhất – Tốt nhất " +
                "– Giá cả cạnh tranh nhất.",
            Address = "Tầng 2, Khu X3-2 Ngõ 68/45, Đường Nguyễn Văn Linh, P. Long Biên, TP. Hà Nội",
            Hotline = "0364.983.444",
            Email = "Info.dangphat@gmail.com",
            CoreValueFast = "Hàng hóa tại kho luôn đầy đủ chủng loại, giao hàng nhanh chóng đến tận chân công trình.",
            CoreValueBest = "Đội ngũ luôn đi đầu nghiên cứu, tìm kiếm hàng hóa đạt chuẩn quốc tế, phù hợp mọi công trình.",
            CoreValueCompetitivePrice = "Chủ động nguồn hàng giúp chúng tôi tự tin với giá cả cạnh tranh nhất."
        });

        var category = new ProductCategory
        {
            Name = "Khớp nối mềm inox",
            Slug = slugService.GenerateSlug("Khớp nối mềm inox"),
            Description = "Khớp nối mềm inox (ống mềm dạng gân xoắn) dùng cho hệ thống chữa cháy sprinkler.",
            MetaTitle = "Khớp nối mềm inox chữa cháy | Đăng Phát Flex",
            MetaDescription = "Khớp nối mềm inox đạt chuẩn UL/FM/TCVN cho hệ thống sprinkler, giao hàng nhanh, giá cạnh tranh."
        };

        var product = new Product
        {
            ProductCategory = category,
            Name = "Đăng Phát Flex DP25",
            Slug = slugService.GenerateSlug("Dang Phat Flex DP25"),
            Description = "Ống mềm inox gân xoắn (Helical Corrugated Hose) dùng cho đầu phun sprinkler, " +
                "có 2 phiên bản: DP25UB (không bện) và DP25B (có bện).",
            InnerDiameter = "24.2mm",
            OuterDiameter = "24.8mm",
            HoseType = "Ống gân xoắn (Helical Corrugated Hose), loại ren (Threaded)",
            MaxTemperature = "107°C (225°F)",
            MaxPressure = "14kg/cm² (TCVN) / 200 psi (UL) / 200 psi (FM)",
            MinBendRadius = "4 inch (UL/ULC) / 9 inch (FM)",
            Standards = "UL, ULC, FM, TCVN",
            MetaTitle = "Ống mềm inox DP25UB / DP25B | Đăng Phát Flex",
            MetaDescription = "Thông số kỹ thuật đầy đủ dòng ống mềm inox DP25UB/DP25B: áp suất, nhiệt độ, bán kính uốn cong, đạt chuẩn UL/FM/TCVN."
        };

        var variantData = new (string Code, string InletOutlet, int LengthMm, int MaxBends)[]
        {
            ("DP25UB-15-700", "1x1/2", 700, 2),
            ("DP25UB-15-1000", "1x1/2", 1000, 3),
            ("DP25UB-15-1200", "1x1/2", 1200, 3),
            ("DP25UB-15-1500", "1x1/2", 1500, 3),
            ("DP25UB-15-1800", "1x1/2", 1800, 3),
            ("DP25UB-20-700", "1x3/4", 700, 2),
            ("DP25UB-20-1000", "1x3/4", 1000, 3),
            ("DP25UB-20-1200", "1x3/4", 1200, 3),
            ("DP25UB-20-1500", "1x3/4", 1500, 3),
            ("DP25UB-20-1800", "1x3/4", 1800, 3),
            ("DP25B-15-700", "1x1/2", 700, 2),
            ("DP25B-15-1000", "1x1/2", 1000, 3),
            ("DP25B-15-1200", "1x1/2", 1200, 3),
            ("DP25B-15-1500", "1x1/2", 1500, 3),
            ("DP25B-15-1800", "1x1/2", 1800, 3),
            ("DP25B-20-700", "1x3/4", 700, 2),
            ("DP25B-20-1000", "1x3/4", 1000, 3),
            ("DP25B-20-1200", "1x3/4", 1200, 3),
            ("DP25B-20-1500", "1x3/4", 1500, 3),
            ("DP25B-20-1800", "1x3/4", 1800, 3),
        };

        foreach (var v in variantData)
        {
            product.Variants.Add(new ProductVariant
            {
                ProductCode = v.Code,
                InletOutlet = v.InletOutlet,
                InstallLengthMm = v.LengthMm,
                MaxBends90 = v.MaxBends,
                MinBendRadiusIn = "4",
            });
        }

        product.Accessories.Add(new Accessory { Name = "Côn giảm", DefaultQuantity = 1 });
        product.Accessories.Add(new Accessory { Name = "Đai ốc", DefaultQuantity = 2 });
        product.Accessories.Add(new Accessory { Name = "Gioăng cao su", DefaultQuantity = 2 });
        product.Accessories.Add(new Accessory { Name = "Vòng đệm nhựa", DefaultQuantity = 2 });
        product.Accessories.Add(new Accessory { Name = "Nipple", DefaultQuantity = 1 });

        context.ProductCategories.Add(category);
        context.Products.Add(product);
        context.SaveChanges();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter DbSeederTests`
Expected: `Passed! - Failed: 0`

- [ ] **Step 5: Call seeder at startup**

In `src/DangPhatFlex.Web/Program.cs`, before `app.Run();`, add:

```csharp
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var slugService = scope.ServiceProvider.GetRequiredService<ISlugService>();
    context.Database.Migrate();
    DbSeeder.Seed(context, slugService);
}
```

- [ ] **Step 6: Commit**

```bash
git add src/DangPhatFlex.Web/Data/DbSeeder.cs src/DangPhatFlex.Web/Program.cs tests/DangPhatFlex.Web.Tests/DbSeederTests.cs
git commit -m "feat: seed database with catalog company info and DP25 product line"
```

---

### Task 5: Shared layout, corporate theme CSS, and SEO meta partial

**Files:**
- Modify: `src/DangPhatFlex.Web/Views/Shared/_Layout.cshtml`
- Create: `src/DangPhatFlex.Web/Views/Shared/_SeoMeta.cshtml`
- Create: `src/DangPhatFlex.Web/wwwroot/css/site.css` (overwrite scaffold default)
- Create: `src/DangPhatFlex.Web/Models/SeoViewData.cs`

**Interfaces:**
- Produces: `ViewData["MetaTitle"]`, `ViewData["MetaDescription"]`, `ViewData["CanonicalUrl"]`, `ViewData["OgImage"]` conventions consumed by `_SeoMeta.cshtml`, included via `@await Html.PartialAsync("_SeoMeta")` at the top of `_Layout.cshtml`'s `<head>`. All Public views (Tasks 6-10) set these `ViewData` keys.

- [ ] **Step 1: Create `_SeoMeta.cshtml` partial**

`src/DangPhatFlex.Web/Views/Shared/_SeoMeta.cshtml`:

```cshtml
@{
    var metaTitle = ViewData["MetaTitle"] as string ?? "Đăng Phát Flex - Giải pháp khớp nối mềm inox";
    var metaDescription = ViewData["MetaDescription"] as string ?? "Chuyên sản xuất, phân phối khớp nối mềm inox cho hệ thống chữa cháy sprinkler. Nhanh nhất - Tốt nhất - Giá cạnh tranh nhất.";
    var canonicalUrl = ViewData["CanonicalUrl"] as string ?? $"{Context.Request.Scheme}://{Context.Request.Host}{Context.Request.Path}";
    var ogImage = ViewData["OgImage"] as string ?? "/images/og-default.jpg";
}
<title>@metaTitle</title>
<meta name="description" content="@metaDescription" />
<link rel="canonical" href="@canonicalUrl" />
<meta property="og:title" content="@metaTitle" />
<meta property="og:description" content="@metaDescription" />
<meta property="og:image" content="@ogImage" />
<meta property="og:url" content="@canonicalUrl" />
<meta property="og:type" content="website" />
<meta name="twitter:card" content="summary_large_image" />
<meta name="twitter:title" content="@metaTitle" />
<meta name="twitter:description" content="@metaDescription" />
```

- [ ] **Step 2: Update `_Layout.cshtml` head and nav**

Replace the `<head>` section of `src/DangPhatFlex.Web/Views/Shared/_Layout.cshtml` with:

```cshtml
<!DOCTYPE html>
<html lang="vi">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    @await Html.PartialAsync("_SeoMeta")
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
</head>
<body>
    <header class="site-header">
        <nav class="navbar navbar-expand-lg site-navbar">
            <div class="container">
                <a class="navbar-brand" asp-area="Public" asp-controller="Home" asp-action="Index">
                    <img src="~/images/logo.png" alt="Đăng Phát Flex" height="48" />
                </a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#mainNav">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="collapse navbar-collapse" id="mainNav">
                    <ul class="navbar-nav ms-auto">
                        <li class="nav-item"><a class="nav-link" asp-area="Public" asp-controller="Home" asp-action="Index">Trang chủ</a></li>
                        <li class="nav-item"><a class="nav-link" asp-area="Public" asp-controller="About" asp-action="Index">Giới thiệu</a></li>
                        <li class="nav-item"><a class="nav-link" asp-area="Public" asp-controller="Products" asp-action="Index">Sản phẩm</a></li>
                        <li class="nav-item"><a class="nav-link" asp-area="Public" asp-controller="Contact" asp-action="Index">Liên hệ</a></li>
                    </ul>
                </div>
            </div>
        </nav>
    </header>
    <main>
        @RenderBody()
    </main>
    <footer class="site-footer">
        <div class="container">
            <p>&copy; @DateTime.Now.Year CÔNG TY TNHH CƠ ĐIỆN ĐĂNG PHÁT - Đăng Phát Flex</p>
        </div>
    </footer>
    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

- [ ] **Step 3: Write corporate theme CSS**

`src/DangPhatFlex.Web/wwwroot/css/site.css`:

```css
:root {
    --dpf-blue: #0B5FA8;
    --dpf-blue-dark: #084A85;
    --dpf-gold: #D4A537;
    --dpf-gray-900: #1A1D21;
    --dpf-gray-600: #5B6470;
    --dpf-gray-100: #F5F7FA;
}

body {
    font-family: "Segoe UI", Roboto, Arial, sans-serif;
    color: var(--dpf-gray-900);
}

.site-header .site-navbar {
    background: #fff;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
}

.site-navbar .nav-link {
    color: var(--dpf-gray-900);
    font-weight: 600;
}

.site-navbar .nav-link:hover {
    color: var(--dpf-blue);
}

.hero {
    background: linear-gradient(135deg, var(--dpf-blue) 0%, var(--dpf-blue-dark) 100%);
    color: #fff;
    padding: 64px 0;
}

.hero h1 {
    font-weight: 700;
}

.btn-brand {
    background-color: var(--dpf-gold);
    border-color: var(--dpf-gold);
    color: var(--dpf-gray-900);
    font-weight: 600;
}

.btn-brand:hover {
    background-color: #b98d29;
    border-color: #b98d29;
    color: #fff;
}

.core-values .card {
    border: none;
    border-top: 3px solid var(--dpf-gold);
    box-shadow: 0 2px 12px rgba(0, 0, 0, 0.06);
}

.spec-table th {
    background-color: var(--dpf-gray-100);
    width: 40%;
}

.site-footer {
    background: var(--dpf-gray-900);
    color: #fff;
    padding: 24px 0;
    margin-top: 48px;
}
```

- [ ] **Step 4: Build and manually verify home page renders without errors**

Run: `dotnet run --project src/DangPhatFlex.Web`
Expected: app starts, navigating to `http://localhost:5000/` renders the scaffold home page with new header/footer/CSS (no 500 error). Stop the server with Ctrl+C afterward.

- [ ] **Step 5: Commit**

```bash
git add src/DangPhatFlex.Web/Views/Shared src/DangPhatFlex.Web/wwwroot/css/site.css
git commit -m "feat: add corporate blue/gold theme, layout, and SEO meta partial"
```

---

### Task 6: Public Home page

**Files:**
- Create: `src/DangPhatFlex.Web/Areas/Public/Controllers/HomeController.cs`
- Create: `src/DangPhatFlex.Web/Areas/Public/Views/Home/Index.cshtml`
- Create: `src/DangPhatFlex.Web/Areas/Public/Views/_ViewImports.cshtml`
- Create: `src/DangPhatFlex.Web/Areas/Public/Views/_ViewStart.cshtml`
- Test: `tests/DangPhatFlex.Web.Tests/HomeControllerTests.cs`

**Interfaces:**
- Consumes: `AppDbContext` (Task 2)
- Produces: route `GET /` (via area default route from Task 1) rendering company Organization JSON-LD + core values + featured products

- [ ] **Step 1: Add Area view configuration**

`src/DangPhatFlex.Web/Areas/Public/Views/_ViewImports.cshtml`:

```cshtml
@using DangPhatFlex.Web
@using DangPhatFlex.Web.Models
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

`src/DangPhatFlex.Web/Areas/Public/Views/_ViewStart.cshtml`:

```cshtml
@{
    Layout = "_Layout";
}
```

- [ ] **Step 2: Write failing integration test**

`tests/DangPhatFlex.Web.Tests/HomeControllerTests.cs`:

```csharp
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class HomeControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HomeControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HomePage_ReturnsOk_AndContainsBrandName()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Đăng Phát Flex", content);
        Assert.Contains("application/ld+json", content);
    }
}
```

- [ ] **Step 3: Run test to verify it fails**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter HomeControllerTests`
Expected: FAIL — route returns 404 (no `HomeController` in `Public` area yet).

- [ ] **Step 4: Implement `HomeController`**

`src/DangPhatFlex.Web/Areas/Public/Controllers/HomeController.cs`:

```csharp
using DangPhatFlex.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DangPhatFlex.Web.Areas.Public.Controllers;

[Area("Public")]
public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["CompanyInfo"] = await _context.CompanyInfos.FirstOrDefaultAsync();
        ViewData["FeaturedProducts"] = await _context.Products
            .Include(p => p.ProductCategory)
            .Take(3)
            .ToListAsync();
        return View();
    }
}
```

- [ ] **Step 5: Implement `Index.cshtml`**

`src/DangPhatFlex.Web/Areas/Public/Views/Home/Index.cshtml`:

```cshtml
@using DangPhatFlex.Web.Models
@{
    var company = ViewData["CompanyInfo"] as CompanyInfo;
    var featured = ViewData["FeaturedProducts"] as List<Product> ?? new();
    ViewData["MetaTitle"] = $"{company?.BrandName} - {company?.Tagline}";
    ViewData["MetaDescription"] = company?.AboutContent?.Length > 160
        ? company.AboutContent[..160]
        : company?.AboutContent;
}

<script type="application/ld+json">
{
  "@@context": "https://schema.org",
  "@@type": "Organization",
  "name": "@company?.LegalName",
  "alternateName": "@company?.BrandName",
  "email": "@company?.Email",
  "telephone": "@company?.Hotline",
  "address": "@company?.Address"
}
</script>

<section class="hero">
    <div class="container text-center">
        <h1>@company?.BrandName</h1>
        <p class="lead">@company?.Tagline</p>
        <a class="btn btn-brand btn-lg" asp-controller="Contact" asp-action="Index">Liên hệ tư vấn</a>
    </div>
</section>

<section class="core-values py-5">
    <div class="container">
        <div class="row g-4">
            <div class="col-md-4">
                <div class="card h-100 p-4">
                    <h3>Nhanh nhất</h3>
                    <p>@company?.CoreValueFast</p>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card h-100 p-4">
                    <h3>Tốt nhất</h3>
                    <p>@company?.CoreValueBest</p>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card h-100 p-4">
                    <h3>Giá cạnh tranh nhất</h3>
                    <p>@company?.CoreValueCompetitivePrice</p>
                </div>
            </div>
        </div>
    </div>
</section>

<section class="featured-products py-5 bg-light">
    <div class="container">
        <h2 class="mb-4">Sản phẩm nổi bật</h2>
        <div class="row g-4">
            @foreach (var product in featured)
            {
                <div class="col-md-4">
                    <div class="card h-100">
                        <div class="card-body">
                            <h5 class="card-title">@product.Name</h5>
                            <p class="card-text">@product.Description</p>
                            <a class="btn btn-outline-primary"
                               asp-controller="Products" asp-action="Detail"
                               asp-route-categorySlug="@product.ProductCategory?.Slug"
                               asp-route-productSlug="@product.Slug">Xem chi tiết</a>
                        </div>
                    </div>
                </div>
            }
        </div>
    </div>
</section>
```

- [ ] **Step 6: Run test to verify it passes**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter HomeControllerTests`
Expected: `Passed! - Failed: 0`

- [ ] **Step 7: Commit**

```bash
git add src/DangPhatFlex.Web/Areas/Public/Controllers/HomeController.cs src/DangPhatFlex.Web/Areas/Public/Views tests/DangPhatFlex.Web.Tests/HomeControllerTests.cs
git commit -m "feat: add Public home page with Organization JSON-LD and core values"
```

---

### Task 7: Public About page

**Files:**
- Create: `src/DangPhatFlex.Web/Areas/Public/Controllers/AboutController.cs`
- Create: `src/DangPhatFlex.Web/Areas/Public/Views/About/Index.cshtml`
- Test: `tests/DangPhatFlex.Web.Tests/AboutControllerTests.cs`

**Interfaces:**
- Consumes: `AppDbContext` (Task 2)
- Produces: route `GET /Public/About` rendering `CompanyInfo.AboutContent`

- [ ] **Step 1: Write failing test**

`tests/DangPhatFlex.Web.Tests/AboutControllerTests.cs`:

```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class AboutControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AboutControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AboutPage_ReturnsOk_AndContainsCompanyLegalName()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Public/About");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("CÔNG TY TNHH CƠ ĐIỆN ĐĂNG PHÁT", content);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter AboutControllerTests`
Expected: FAIL — 404, no `AboutController` yet.

- [ ] **Step 3: Implement `AboutController`**

`src/DangPhatFlex.Web/Areas/Public/Controllers/AboutController.cs`:

```csharp
using DangPhatFlex.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DangPhatFlex.Web.Areas.Public.Controllers;

[Area("Public")]
public class AboutController : Controller
{
    private readonly AppDbContext _context;

    public AboutController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var company = await _context.CompanyInfos.FirstOrDefaultAsync();
        ViewData["MetaTitle"] = $"Giới thiệu công ty - {company?.BrandName}";
        ViewData["MetaDescription"] = "Tìm hiểu về CÔNG TY TNHH CƠ ĐIỆN ĐĂNG PHÁT, đơn vị sản xuất và phân phối khớp nối mềm inox uy tín.";
        return View(company);
    }
}
```

- [ ] **Step 4: Implement `Index.cshtml`**

`src/DangPhatFlex.Web/Areas/Public/Views/About/Index.cshtml`:

```cshtml
@model DangPhatFlex.Web.Models.CompanyInfo

<section class="container py-5">
    <h1>Giới thiệu công ty</h1>
    <h2 class="text-muted">@Model?.LegalName</h2>
    <p class="lead">@Model?.Tagline</p>
    <div class="about-content">
        <p>@Model?.AboutContent</p>
    </div>
    <div class="row mt-4">
        <div class="col-md-6">
            <strong>Địa chỉ:</strong> @Model?.Address
        </div>
        <div class="col-md-3">
            <strong>Hotline:</strong> @Model?.Hotline
        </div>
        <div class="col-md-3">
            <strong>Email:</strong> @Model?.Email
        </div>
    </div>
</section>
```

- [ ] **Step 5: Run test to verify it passes**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter AboutControllerTests`
Expected: `Passed! - Failed: 0`

- [ ] **Step 6: Commit**

```bash
git add src/DangPhatFlex.Web/Areas/Public/Controllers/AboutController.cs src/DangPhatFlex.Web/Areas/Public/Views/About tests/DangPhatFlex.Web.Tests/AboutControllerTests.cs
git commit -m "feat: add Public about page"
```

---

### Task 8: Public Products listing page

**Files:**
- Create: `src/DangPhatFlex.Web/Areas/Public/Controllers/ProductsController.cs`
- Create: `src/DangPhatFlex.Web/Areas/Public/Views/Products/Index.cshtml`
- Test: `tests/DangPhatFlex.Web.Tests/ProductsControllerTests.cs`

**Interfaces:**
- Consumes: `AppDbContext` (Task 2)
- Produces: route `GET /Public/Products` (list, optional `?categorySlug=`), and `ProductsController.Detail(string categorySlug, string productSlug)` action stub used by Task 6's view links and completed fully in Task 9.

- [ ] **Step 1: Write failing test**

`tests/DangPhatFlex.Web.Tests/ProductsControllerTests.cs`:

```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProductsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ProductsIndex_ReturnsOk_AndListsSeededProduct()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Public/Products");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Đăng Phát Flex DP25", content);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter ProductsControllerTests`
Expected: FAIL — 404, no `ProductsController` yet.

- [ ] **Step 3: Implement `ProductsController` (Index + Detail stub)**

`src/DangPhatFlex.Web/Areas/Public/Controllers/ProductsController.cs`:

```csharp
using DangPhatFlex.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DangPhatFlex.Web.Areas.Public.Controllers;

[Area("Public")]
public class ProductsController : Controller
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? categorySlug)
    {
        var query = _context.Products.Include(p => p.ProductCategory).AsQueryable();
        if (!string.IsNullOrEmpty(categorySlug))
            query = query.Where(p => p.ProductCategory!.Slug == categorySlug);

        ViewData["MetaTitle"] = "Sản phẩm khớp nối mềm inox | Đăng Phát Flex";
        ViewData["MetaDescription"] = "Danh sách sản phẩm khớp nối mềm inox DP25UB, DP25B đạt chuẩn UL/FM/TCVN cho hệ thống chữa cháy.";
        ViewData["Categories"] = await _context.ProductCategories.ToListAsync();
        return View(await query.ToListAsync());
    }

    [Route("Public/Products/{categorySlug}/{productSlug}")]
    public async Task<IActionResult> Detail(string categorySlug, string productSlug)
    {
        // Full implementation in Task 9
        var product = await _context.Products
            .Include(p => p.ProductCategory)
            .Include(p => p.Variants)
            .Include(p => p.Accessories)
            .FirstOrDefaultAsync(p => p.Slug == productSlug && p.ProductCategory!.Slug == categorySlug);

        if (product is null)
            return NotFound();

        return View(product);
    }
}
```

- [ ] **Step 4: Implement `Index.cshtml`**

`src/DangPhatFlex.Web/Areas/Public/Views/Products/Index.cshtml`:

```cshtml
@model List<DangPhatFlex.Web.Models.Product>

<section class="container py-5">
    <h1>Sản phẩm</h1>
    <div class="row g-4">
        @foreach (var product in Model)
        {
            <div class="col-md-4">
                <div class="card h-100">
                    <div class="card-body">
                        <h5 class="card-title">@product.Name</h5>
                        <p class="card-text">@product.Description</p>
                        <a class="btn btn-outline-primary"
                           asp-action="Detail"
                           asp-route-categorySlug="@product.ProductCategory?.Slug"
                           asp-route-productSlug="@product.Slug">Xem chi tiết</a>
                    </div>
                </div>
            </div>
        }
    </div>
</section>
```

- [ ] **Step 5: Create a minimal `Detail.cshtml` placeholder so build succeeds (replaced fully in Task 9)**

`src/DangPhatFlex.Web/Areas/Public/Views/Products/Detail.cshtml`:

```cshtml
@model DangPhatFlex.Web.Models.Product

<section class="container py-5">
    <h1>@Model.Name</h1>
    <p>@Model.Description</p>
</section>
```

- [ ] **Step 6: Run test to verify it passes**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter ProductsControllerTests`
Expected: `Passed! - Failed: 0`

- [ ] **Step 7: Commit**

```bash
git add src/DangPhatFlex.Web/Areas/Public/Controllers/ProductsController.cs src/DangPhatFlex.Web/Areas/Public/Views/Products tests/DangPhatFlex.Web.Tests/ProductsControllerTests.cs
git commit -m "feat: add Public products listing page and detail route"
```

---

### Task 9: Public Product detail page (full spec table + JSON-LD)

**Files:**
- Modify: `src/DangPhatFlex.Web/Areas/Public/Views/Products/Detail.cshtml`
- Test: `tests/DangPhatFlex.Web.Tests/ProductDetailTests.cs`

**Interfaces:**
- Consumes: `ProductsController.Detail` (Task 8), `Product`/`ProductVariant`/`Accessory` models (Task 2)

- [ ] **Step 1: Write failing test**

`tests/DangPhatFlex.Web.Tests/ProductDetailTests.cs`:

```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class ProductDetailTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProductDetailTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ProductDetail_ReturnsOk_AndContainsVariantCodeAndJsonLd()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Public/Products/khop-noi-mem-inox/dang-phat-flex-dp25");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("DP25UB-15-700", content);
        Assert.Contains("\"@@type\": \"Product\"", content);
    }

    [Fact]
    public async Task ProductDetail_UnknownSlug_ReturnsNotFound()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Public/Products/khop-noi-mem-inox/khong-ton-tai");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter ProductDetailTests`
Expected: FAIL — placeholder view doesn't render variant codes or JSON-LD.

- [ ] **Step 3: Implement full `Detail.cshtml`**

`src/DangPhatFlex.Web/Areas/Public/Views/Products/Detail.cshtml`:

```cshtml
@model DangPhatFlex.Web.Models.Product
@{
    ViewData["MetaTitle"] = Model.MetaTitle ?? Model.Name;
    ViewData["MetaDescription"] = Model.MetaDescription ?? Model.Description;
}

<script type="application/ld+json">
{
  "@@context": "https://schema.org",
  "@@type": "Product",
  "name": "@Model.Name",
  "description": "@Model.Description",
  "brand": { "@@type": "Brand", "name": "Đăng Phát Flex" }
}
</script>

<section class="container py-5">
    <nav aria-label="breadcrumb">
        <ol class="breadcrumb">
            <li class="breadcrumb-item"><a asp-action="Index">Sản phẩm</a></li>
            <li class="breadcrumb-item active">@Model.Name</li>
        </ol>
    </nav>

    <h1>@Model.Name</h1>
    <p class="lead">@Model.Description</p>

    @if (!string.IsNullOrEmpty(Model.MainImageUrl))
    {
        <img src="@Model.MainImageUrl" alt="@(Model.MainImageAlt ?? Model.Name)" class="img-fluid mb-4" />
    }

    <h2>Thông số kỹ thuật</h2>
    <table class="table spec-table">
        <tbody>
            <tr><th>Đường kính ống</th><td>ID: @Model.InnerDiameter / OD: @Model.OuterDiameter</td></tr>
            <tr><th>Loại ống</th><td>@Model.HoseType</td></tr>
            <tr><th>Nhiệt độ hoạt động tối đa</th><td>@Model.MaxTemperature</td></tr>
            <tr><th>Áp suất hoạt động tối đa</th><td>@Model.MaxPressure</td></tr>
            <tr><th>Bán kính uốn cong nhỏ nhất</th><td>@Model.MinBendRadius</td></tr>
            <tr><th>Tiêu chuẩn</th><td>@Model.Standards</td></tr>
        </tbody>
    </table>

    <h2>Bảng mã sản phẩm</h2>
    <div class="table-responsive">
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Mã sản phẩm</th>
                    <th>Kết nối (Inlet x Outlet)</th>
                    <th>Chiều dài lắp đặt (mm)</th>
                    <th>Số lần uốn 90° tối đa</th>
                    <th>Bán kính uốn cong tối thiểu (in)</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var v in Model.Variants)
                {
                    <tr>
                        <td>@v.ProductCode</td>
                        <td>@v.InletOutlet</td>
                        <td>@v.InstallLengthMm</td>
                        <td>@v.MaxBends90</td>
                        <td>@v.MinBendRadiusIn</td>
                    </tr>
                }
            </tbody>
        </table>
    </div>

    @if (Model.Accessories.Any())
    {
        <h2>Phụ kiện đi kèm</h2>
        <ul>
            @foreach (var a in Model.Accessories)
            {
                <li>@a.Name (SL: @a.DefaultQuantity)</li>
            }
        </ul>
    }

    @if (!string.IsNullOrEmpty(Model.DatasheetPdfUrl))
    {
        <a class="btn btn-brand" href="@Model.DatasheetPdfUrl" target="_blank" rel="noopener">Tải Datasheet PDF</a>
    }
</section>
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter ProductDetailTests`
Expected: `Passed! - Failed: 0`

- [ ] **Step 5: Commit**

```bash
git add src/DangPhatFlex.Web/Areas/Public/Views/Products/Detail.cshtml tests/DangPhatFlex.Web.Tests/ProductDetailTests.cs
git commit -m "feat: add full product detail page with spec table and Product JSON-LD"
```

---

### Task 10: Public Contact page (form + save + email)

**Files:**
- Create: `src/DangPhatFlex.Web/Services/IEmailSender.cs`
- Create: `src/DangPhatFlex.Web/Services/SmtpEmailSender.cs`
- Create: `src/DangPhatFlex.Web/Areas/Public/Controllers/ContactController.cs`
- Create: `src/DangPhatFlex.Web/Areas/Public/Models/ContactFormViewModel.cs`
- Create: `src/DangPhatFlex.Web/Areas/Public/Views/Contact/Index.cshtml`
- Modify: `src/DangPhatFlex.Web/appsettings.json`
- Modify: `src/DangPhatFlex.Web/Program.cs`
- Test: `tests/DangPhatFlex.Web.Tests/ContactControllerTests.cs`

**Interfaces:**
- Consumes: `AppDbContext` (Task 2)
- Produces: `IEmailSender.SendAsync(string toEmail, string subject, string body)`, route `GET/POST /Public/Contact`

- [ ] **Step 1: Write failing test**

`tests/DangPhatFlex.Web.Tests/ContactControllerTests.cs`:

```csharp
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class ContactControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ContactControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<DangPhatFlex.Web.Services.IEmailSender, FakeEmailSender>();
            });
        });
    }

    [Fact]
    public async Task PostContactForm_ValidInput_RedirectsAndSaves()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var form = new Dictionary<string, string>
        {
            ["FullName"] = "Nguyễn Văn A",
            ["Phone"] = "0900000000",
            ["Message"] = "Tôi cần báo giá khớp nối mềm inox DP25UB."
        };

        var response = await client.PostAsync("/Public/Contact", new FormUrlEncodedContent(form));

        Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
    }

    private class FakeEmailSender : DangPhatFlex.Web.Services.IEmailSender
    {
        public Task SendAsync(string toEmail, string subject, string body) => Task.CompletedTask;
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter ContactControllerTests`
Expected: FAIL — 404, no `ContactController` yet.

- [ ] **Step 3: Implement `IEmailSender` and `SmtpEmailSender`**

`src/DangPhatFlex.Web/Services/IEmailSender.cs`:

```csharp
namespace DangPhatFlex.Web.Services;

public interface IEmailSender
{
    Task SendAsync(string toEmail, string subject, string body);
}
```

`src/DangPhatFlex.Web/Services/SmtpEmailSender.cs`:

```csharp
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace DangPhatFlex.Web.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string subject, string body)
    {
        var host = _configuration["Smtp:Host"];
        if (string.IsNullOrEmpty(host))
        {
            _logger.LogWarning("Smtp:Host not configured; skipping email send to {ToEmail}", toEmail);
            return;
        }

        using var client = new SmtpClient(host, int.Parse(_configuration["Smtp:Port"] ?? "587"))
        {
            Credentials = new NetworkCredential(_configuration["Smtp:User"], _configuration["Smtp:Password"]),
            EnableSsl = true
        };

        using var message = new MailMessage(_configuration["Smtp:From"] ?? "no-reply@dangphatflex.vn", toEmail, subject, body);
        await client.SendMailAsync(message);
    }
}
```

- [ ] **Step 4: Add view model and controller**

`src/DangPhatFlex.Web/Areas/Public/Models/ContactFormViewModel.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace DangPhatFlex.Web.Areas.Public.Models;

public class ContactFormViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [MaxLength(50)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [MaxLength(150)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập nội dung")]
    public string Message { get; set; } = string.Empty;
}
```

`src/DangPhatFlex.Web/Areas/Public/Controllers/ContactController.cs`:

```csharp
using DangPhatFlex.Web.Areas.Public.Models;
using DangPhatFlex.Web.Data;
using DangPhatFlex.Web.Models;
using DangPhatFlex.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DangPhatFlex.Web.Areas.Public.Controllers;

[Area("Public")]
public class ContactController : Controller
{
    private readonly AppDbContext _context;
    private readonly IEmailSender _emailSender;

    public ContactController(AppDbContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    public IActionResult Index()
    {
        ViewData["MetaTitle"] = "Liên hệ | Đăng Phát Flex";
        ViewData["MetaDescription"] = "Liên hệ CÔNG TY TNHH CƠ ĐIỆN ĐĂNG PHÁT để được tư vấn khớp nối mềm inox cho hệ thống chữa cháy.";
        return View(new ContactFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _context.ContactSubmissions.Add(new ContactSubmission
        {
            FullName = model.FullName,
            Phone = model.Phone,
            Email = model.Email,
            Message = model.Message,
            SubmittedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var company = await _context.CompanyInfos.FirstOrDefaultAsync();
        if (company is not null)
        {
            await _emailSender.SendAsync(
                company.Email,
                $"Liên hệ mới từ {model.FullName}",
                $"SĐT: {model.Phone}\nEmail: {model.Email}\nNội dung: {model.Message}");
        }

        TempData["ContactSuccess"] = "Cảm ơn bạn đã liên hệ. Chúng tôi sẽ phản hồi sớm nhất!";
        return RedirectToAction(nameof(Index));
    }
}
```

- [ ] **Step 5: Register `IEmailSender` in DI**

In `src/DangPhatFlex.Web/Program.cs`, add:

```csharp
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
```

- [ ] **Step 6: Implement view**

`src/DangPhatFlex.Web/Areas/Public/Views/Contact/Index.cshtml`:

```cshtml
@model DangPhatFlex.Web.Areas.Public.Models.ContactFormViewModel

<section class="container py-5">
    <h1>Liên hệ</h1>

    @if (TempData["ContactSuccess"] is string successMessage)
    {
        <div class="alert alert-success">@successMessage</div>
    }

    <form asp-action="Index" method="post" class="row g-3">
        <div asp-validation-summary="ModelOnly" class="text-danger"></div>
        <div class="col-md-6">
            <label asp-for="FullName" class="form-label"></label>
            <input asp-for="FullName" class="form-control" />
            <span asp-validation-for="FullName" class="text-danger"></span>
        </div>
        <div class="col-md-6">
            <label asp-for="Phone" class="form-label"></label>
            <input asp-for="Phone" class="form-control" />
            <span asp-validation-for="Phone" class="text-danger"></span>
        </div>
        <div class="col-md-6">
            <label asp-for="Email" class="form-label"></label>
            <input asp-for="Email" class="form-control" />
            <span asp-validation-for="Email" class="text-danger"></span>
        </div>
        <div class="col-12">
            <label asp-for="Message" class="form-label"></label>
            <textarea asp-for="Message" class="form-control" rows="4"></textarea>
            <span asp-validation-for="Message" class="text-danger"></span>
        </div>
        <div class="col-12">
            <button type="submit" class="btn btn-brand">Gửi liên hệ</button>
        </div>
    </form>
</section>
```

- [ ] **Step 7: Run test to verify it passes**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter ContactControllerTests`
Expected: `Passed! - Failed: 0`

- [ ] **Step 8: Commit**

```bash
git add src/DangPhatFlex.Web/Services/IEmailSender.cs src/DangPhatFlex.Web/Services/SmtpEmailSender.cs src/DangPhatFlex.Web/Areas/Public/Controllers/ContactController.cs src/DangPhatFlex.Web/Areas/Public/Models src/DangPhatFlex.Web/Areas/Public/Views/Contact src/DangPhatFlex.Web/Program.cs tests/DangPhatFlex.Web.Tests/ContactControllerTests.cs
git commit -m "feat: add Public contact page with form persistence and email notification"
```

---

### Task 11: SEO infrastructure (sitemap.xml, robots.txt)

**Files:**
- Create: `src/DangPhatFlex.Web/Areas/Public/Controllers/SeoController.cs`
- Create: `src/DangPhatFlex.Web/wwwroot/robots.txt`
- Test: `tests/DangPhatFlex.Web.Tests/SeoControllerTests.cs`

**Interfaces:**
- Consumes: `AppDbContext` (Task 2)
- Produces: route `GET /sitemap.xml`

- [ ] **Step 1: Write failing test**

`tests/DangPhatFlex.Web.Tests/SeoControllerTests.cs`:

```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class SeoControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SeoControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Sitemap_ReturnsXml_ContainingProductUrl()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/sitemap.xml");

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/xml", response.Content.Headers.ContentType?.MediaType);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("dang-phat-flex-dp25", content);
    }

    [Fact]
    public async Task RobotsTxt_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/robots.txt");

        response.EnsureSuccessStatusCode();
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter SeoControllerTests`
Expected: FAIL — `/sitemap.xml` 404, `/robots.txt` 404.

- [ ] **Step 3: Implement `SeoController`**

`src/DangPhatFlex.Web/Areas/Public/Controllers/SeoController.cs`:

```csharp
using System.Text;
using DangPhatFlex.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DangPhatFlex.Web.Areas.Public.Controllers;

[Area("Public")]
public class SeoController : Controller
{
    private readonly AppDbContext _context;

    public SeoController(AppDbContext context)
    {
        _context = context;
    }

    [Route("/sitemap.xml")]
    public async Task<IActionResult> Sitemap()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var urls = new List<string> { $"{baseUrl}/", $"{baseUrl}/Public/About", $"{baseUrl}/Public/Products", $"{baseUrl}/Public/Contact" };

        var products = await _context.Products.Include(p => p.ProductCategory).ToListAsync();
        urls.AddRange(products.Select(p => $"{baseUrl}/Public/Products/{p.ProductCategory!.Slug}/{p.Slug}"));

        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
        foreach (var url in urls)
        {
            sb.AppendLine("<url>");
            sb.AppendLine($"<loc>{url}</loc>");
            sb.AppendLine("</url>");
        }
        sb.AppendLine("</urlset>");

        return Content(sb.ToString(), "application/xml", Encoding.UTF8);
    }
}
```

- [ ] **Step 4: Add static `robots.txt`**

`src/DangPhatFlex.Web/wwwroot/robots.txt`:

```
User-agent: *
Allow: /
Sitemap: /sitemap.xml
```

- [ ] **Step 5: Run test to verify it passes**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter SeoControllerTests`
Expected: `Passed! - Failed: 0`

- [ ] **Step 6: Commit**

```bash
git add src/DangPhatFlex.Web/Areas/Public/Controllers/SeoController.cs src/DangPhatFlex.Web/wwwroot/robots.txt tests/DangPhatFlex.Web.Tests/SeoControllerTests.cs
git commit -m "feat: add dynamic sitemap.xml and robots.txt"
```

---

### Task 12: ASP.NET Core Identity setup and Admin auth

**Files:**
- Modify: `src/DangPhatFlex.Web/Data/AppDbContext.cs`
- Create: `src/DangPhatFlex.Web/Data/IdentitySeeder.cs`
- Modify: `src/DangPhatFlex.Web/Program.cs`
- Create: `src/DangPhatFlex.Web/Areas/Admin/Views/_ViewImports.cshtml`
- Create: `src/DangPhatFlex.Web/Areas/Admin/Views/_ViewStart.cshtml`
- Test: `tests/DangPhatFlex.Web.Tests/AdminAuthTests.cs`

**Interfaces:**
- Consumes: `AppDbContext` (Task 2)
- Produces: `AppDbContext : IdentityDbContext<IdentityUser>`, seeded Admin role + user (`admin@dangphatflex.vn` / configurable password via `appsettings`), `[Authorize(Roles = "Admin")]` convention applied to all Admin area controllers (Tasks 13-16)

- [ ] **Step 1: Change `AppDbContext` to extend `IdentityDbContext`**

In `src/DangPhatFlex.Web/Data/AppDbContext.cs`, change:

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    // ... existing DbSets and OnModelCreating unchanged, but call base.OnModelCreating(modelBuilder) first:

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // ... existing configuration from Task 2 stays below this line
    }
}
```

- [ ] **Step 2: Add Identity services in `Program.cs`**

```csharp
using Microsoft.AspNetCore.Identity;

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();
```

Add after `app.UseRouting();` and before `app.MapControllerRoute`:

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

Add `builder.Services.AddRazorPages();` next to `AddControllersWithViews()`, and `app.MapRazorPages();` next to the area route mapping (Identity's default UI uses Razor Pages for login).

- [ ] **Step 3: Add admin credentials to configuration**

In `src/DangPhatFlex.Web/appsettings.Development.json`:

```json
{
  "AdminSeed": {
    "Email": "admin@dangphatflex.vn",
    "Password": "ChangeMe123!"
  }
}
```

- [ ] **Step 4: Create `IdentitySeeder`**

`src/DangPhatFlex.Web/Data/IdentitySeeder.cs`:

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace DangPhatFlex.Web.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        const string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
            await roleManager.CreateAsync(new IdentityRole(adminRole));

        var email = configuration["AdminSeed:Email"];
        var password = configuration["AdminSeed:Password"];
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            return;

        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null)
            return;

        var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, adminRole);
    }
}
```

Call it in `Program.cs` inside the existing seeding scope block (after `DbSeeder.Seed(...)`):

```csharp
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
await IdentitySeeder.SeedAsync(userManager, roleManager, app.Configuration);
```

(Since this block is synchronous today, change the enclosing `using` block to run inline `await` calls — the surrounding `Program.cs` top-level statements already support `await` since `dotnet new mvc` templates are async-capable top-level statements.)

- [ ] **Step 5: Regenerate migration for Identity tables**

```bash
cd src/DangPhatFlex.Web
dotnet ef migrations add AddIdentity -o Data/Migrations
dotnet ef database update
cd ../..
```

- [ ] **Step 6: Add Area view configuration for Admin**

`src/DangPhatFlex.Web/Areas/Admin/Views/_ViewImports.cshtml`:

```cshtml
@using DangPhatFlex.Web
@using DangPhatFlex.Web.Models
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

`src/DangPhatFlex.Web/Areas/Admin/Views/_ViewStart.cshtml`:

```cshtml
@{
    Layout = "_Layout";
}
```

- [ ] **Step 7: Write integration test verifying Admin area requires auth**

`tests/DangPhatFlex.Web.Tests/AdminAuthTests.cs`:

```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class AdminAuthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AdminAuthTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AdminDashboard_WithoutLogin_RedirectsToLogin()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var response = await client.GetAsync("/Admin/Dashboard");

        Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location!.ToString());
    }
}
```

This test will pass once Task 13 adds a `[Authorize(Roles = "Admin")]`-decorated controller at `/Admin/Dashboard` — run it after Task 13's dashboard controller exists; for now confirm the Identity middleware/build compiles.

- [ ] **Step 8: Build to verify Identity wiring compiles**

Run: `dotnet build`
Expected: `Build succeeded.`

- [ ] **Step 9: Commit**

```bash
git add src/DangPhatFlex.Web/Data src/DangPhatFlex.Web/Program.cs src/DangPhatFlex.Web/Areas/Admin/Views src/DangPhatFlex.Web/appsettings.Development.json tests/DangPhatFlex.Web.Tests/AdminAuthTests.cs
git commit -m "feat: add ASP.NET Core Identity with seeded Admin role and user"
```

---

### Task 13: Admin dashboard and ProductCategory CRUD

**Files:**
- Create: `src/DangPhatFlex.Web/Areas/Admin/Controllers/DashboardController.cs`
- Create: `src/DangPhatFlex.Web/Areas/Admin/Controllers/ProductCategoriesController.cs`
- Create: `src/DangPhatFlex.Web/Areas/Admin/Views/Dashboard/Index.cshtml`
- Create: `src/DangPhatFlex.Web/Areas/Admin/Views/ProductCategories/Index.cshtml`
- Create: `src/DangPhatFlex.Web/Areas/Admin/Views/ProductCategories/Create.cshtml`
- Create: `src/DangPhatFlex.Web/Areas/Admin/Views/ProductCategories/Edit.cshtml`
- Test: `tests/DangPhatFlex.Web.Tests/ProductCategoriesAdminTests.cs`

**Interfaces:**
- Consumes: `AppDbContext`, `ISlugService` (Tasks 2-3), Identity `[Authorize(Roles = "Admin")]`
- Produces: authenticated CRUD pattern reused by Task 14 (`ProductsController`)

- [ ] **Step 1: Write failing test (auth-gated CRUD)**

`tests/DangPhatFlex.Web.Tests/ProductCategoriesAdminTests.cs`:

```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class ProductCategoriesAdminTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProductCategoriesAdminTests(WebApplicationFactory<Program> factory)
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
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter ProductCategoriesAdminTests`
Expected: FAIL — route doesn't exist (404, not redirect).

- [ ] **Step 3: Implement `DashboardController`**

`src/DangPhatFlex.Web/Areas/Admin/Controllers/DashboardController.cs`:

```csharp
using DangPhatFlex.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DangPhatFlex.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["ProductCount"] = await _context.Products.CountAsync();
        ViewData["NewContactCount"] = await _context.ContactSubmissions.CountAsync(c => !c.IsProcessed);
        return View();
    }
}
```

`src/DangPhatFlex.Web/Areas/Admin/Views/Dashboard/Index.cshtml`:

```cshtml
<section class="container py-4">
    <h1>Bảng điều khiển</h1>
    <div class="row g-3">
        <div class="col-md-4">
            <div class="card p-3"><strong>Sản phẩm:</strong> @ViewData["ProductCount"]</div>
        </div>
        <div class="col-md-4">
            <div class="card p-3"><strong>Liên hệ mới:</strong> @ViewData["NewContactCount"]</div>
        </div>
    </div>
    <ul class="mt-4">
        <li><a asp-controller="ProductCategories" asp-action="Index">Quản lý danh mục sản phẩm</a></li>
        <li><a asp-controller="Products" asp-action="Index">Quản lý sản phẩm</a></li>
        <li><a asp-controller="CompanyInfo" asp-action="Edit">Thông tin công ty</a></li>
        <li><a asp-controller="ContactSubmissions" asp-action="Index">Danh sách liên hệ</a></li>
    </ul>
</section>
```

- [ ] **Step 4: Implement `ProductCategoriesController`**

`src/DangPhatFlex.Web/Areas/Admin/Controllers/ProductCategoriesController.cs`:

```csharp
using DangPhatFlex.Web.Data;
using DangPhatFlex.Web.Models;
using DangPhatFlex.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DangPhatFlex.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductCategoriesController : Controller
{
    private readonly AppDbContext _context;
    private readonly ISlugService _slugService;

    public ProductCategoriesController(AppDbContext context, ISlugService slugService)
    {
        _context = context;
        _slugService = slugService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.ProductCategories.ToListAsync());
    }

    public IActionResult Create() => View(new ProductCategory());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCategory model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (string.IsNullOrWhiteSpace(model.Slug))
            model.Slug = _slugService.GenerateSlug(model.Name);

        _context.ProductCategories.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _context.ProductCategories.FindAsync(id);
        if (category is null)
            return NotFound();
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductCategory model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        if (string.IsNullOrWhiteSpace(model.Slug))
            model.Slug = _slugService.GenerateSlug(model.Name);

        _context.Update(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.ProductCategories.FindAsync(id);
        if (category is not null)
        {
            _context.ProductCategories.Remove(category);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
```

- [ ] **Step 5: Implement views**

`src/DangPhatFlex.Web/Areas/Admin/Views/ProductCategories/Index.cshtml`:

```cshtml
@model List<DangPhatFlex.Web.Models.ProductCategory>

<section class="container py-4">
    <h1>Danh mục sản phẩm</h1>
    <a class="btn btn-brand mb-3" asp-action="Create">Thêm danh mục</a>
    <table class="table">
        <thead><tr><th>Tên</th><th>Slug</th><th></th></tr></thead>
        <tbody>
            @foreach (var c in Model)
            {
                <tr>
                    <td>@c.Name</td>
                    <td>@c.Slug</td>
                    <td>
                        <a asp-action="Edit" asp-route-id="@c.Id">Sửa</a>
                        <form asp-action="Delete" asp-route-id="@c.Id" method="post" class="d-inline">
                            <button type="submit" class="btn btn-link text-danger p-0 ms-2"
                                    onclick="return confirm('Xóa danh mục này?');">Xóa</button>
                        </form>
                    </td>
                </tr>
            }
        </tbody>
    </table>
</section>
```

`src/DangPhatFlex.Web/Areas/Admin/Views/ProductCategories/Create.cshtml`:

```cshtml
@model DangPhatFlex.Web.Models.ProductCategory

<section class="container py-4">
    <h1>Thêm danh mục</h1>
    <form asp-action="Create" method="post">
        <div asp-validation-summary="ModelOnly" class="text-danger"></div>
        <div class="mb-3">
            <label asp-for="Name" class="form-label"></label>
            <input asp-for="Name" class="form-control" />
        </div>
        <div class="mb-3">
            <label asp-for="Slug" class="form-label">Slug (để trống để tự sinh)</label>
            <input asp-for="Slug" class="form-control" />
        </div>
        <div class="mb-3">
            <label asp-for="Description" class="form-label"></label>
            <textarea asp-for="Description" class="form-control"></textarea>
        </div>
        <button type="submit" class="btn btn-brand">Lưu</button>
    </form>
</section>
```

`src/DangPhatFlex.Web/Areas/Admin/Views/ProductCategories/Edit.cshtml`:

```cshtml
@model DangPhatFlex.Web.Models.ProductCategory

<section class="container py-4">
    <h1>Sửa danh mục</h1>
    <form asp-action="Edit" asp-route-id="@Model.Id" method="post">
        <input type="hidden" asp-for="Id" />
        <div asp-validation-summary="ModelOnly" class="text-danger"></div>
        <div class="mb-3">
            <label asp-for="Name" class="form-label"></label>
            <input asp-for="Name" class="form-control" />
        </div>
        <div class="mb-3">
            <label asp-for="Slug" class="form-label"></label>
            <input asp-for="Slug" class="form-control" />
        </div>
        <div class="mb-3">
            <label asp-for="Description" class="form-label"></label>
            <textarea asp-for="Description" class="form-control">@Model.Description</textarea>
        </div>
        <button type="submit" class="btn btn-brand">Lưu</button>
    </form>
</section>
```

- [ ] **Step 6: Run test to verify it passes**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter "ProductCategoriesAdminTests|AdminAuthTests"`
Expected: `Passed! - Failed: 0`

- [ ] **Step 7: Commit**

```bash
git add src/DangPhatFlex.Web/Areas/Admin/Controllers/DashboardController.cs src/DangPhatFlex.Web/Areas/Admin/Controllers/ProductCategoriesController.cs src/DangPhatFlex.Web/Areas/Admin/Views/Dashboard src/DangPhatFlex.Web/Areas/Admin/Views/ProductCategories tests/DangPhatFlex.Web.Tests/ProductCategoriesAdminTests.cs
git commit -m "feat: add Admin dashboard and ProductCategory CRUD"
```

---

### Task 14: Admin Product CRUD (with variants, accessories, image/PDF upload)

**Files:**
- Create: `src/DangPhatFlex.Web/Areas/Admin/Models/ProductFormViewModel.cs`
- Create: `src/DangPhatFlex.Web/Services/IFileUploadService.cs`
- Create: `src/DangPhatFlex.Web/Services/FileUploadService.cs`
- Create: `src/DangPhatFlex.Web/Areas/Admin/Controllers/ProductsController.cs`
- Create: `src/DangPhatFlex.Web/Areas/Admin/Views/Products/Index.cshtml`
- Create: `src/DangPhatFlex.Web/Areas/Admin/Views/Products/Create.cshtml`
- Create: `src/DangPhatFlex.Web/Areas/Admin/Views/Products/Edit.cshtml`
- Modify: `src/DangPhatFlex.Web/Program.cs`
- Test: `tests/DangPhatFlex.Web.Tests/FileUploadServiceTests.cs`

**Interfaces:**
- Consumes: `AppDbContext`, `ISlugService` (Tasks 2-3)
- Produces: `public interface IFileUploadService { Task<string> SaveAsync(IFormFile file, string subfolder); }`

- [ ] **Step 1: Write failing test for `FileUploadService`**

`tests/DangPhatFlex.Web.Tests/FileUploadServiceTests.cs`:

```csharp
using DangPhatFlex.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class FileUploadServiceTests
{
    [Fact]
    public async Task SaveAsync_WritesFileToUploadsSubfolder_AndReturnsRelativeUrl()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempRoot);
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(e => e.WebRootPath).Returns(tempRoot);

        var sut = new FileUploadService(envMock.Object);

        var content = "fake-image-bytes"u8.ToArray();
        using var stream = new MemoryStream(content);
        var file = new FormFile(stream, 0, content.Length, "file", "logo.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        var url = await sut.SaveAsync(file, "products");

        Assert.StartsWith("/uploads/products/", url);
        Assert.True(File.Exists(Path.Combine(tempRoot, url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar))));

        Directory.Delete(tempRoot, true);
    }
}
```

- [ ] **Step 2: Add `Moq` test dependency and run test to verify it fails**

```bash
cd tests/DangPhatFlex.Web.Tests
dotnet add package Moq --version 4.20.72
cd ../..
dotnet test tests/DangPhatFlex.Web.Tests --filter FileUploadServiceTests
```
Expected: FAIL — `FileUploadService` does not exist yet.

- [ ] **Step 3: Implement `IFileUploadService`/`FileUploadService`**

`src/DangPhatFlex.Web/Services/IFileUploadService.cs`:

```csharp
using Microsoft.AspNetCore.Http;

namespace DangPhatFlex.Web.Services;

public interface IFileUploadService
{
    Task<string> SaveAsync(IFormFile file, string subfolder);
}
```

`src/DangPhatFlex.Web/Services/FileUploadService.cs`:

```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace DangPhatFlex.Web.Services;

public class FileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _environment;

    public FileUploadService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveAsync(IFormFile file, string subfolder)
    {
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", subfolder);
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/{subfolder}/{fileName}";
    }
}
```

- [ ] **Step 4: Register in DI**

In `src/DangPhatFlex.Web/Program.cs`:

```csharp
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
```

- [ ] **Step 5: Run test to verify it passes**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter FileUploadServiceTests`
Expected: `Passed! - Failed: 0`

- [ ] **Step 6: Implement `ProductFormViewModel`**

`src/DangPhatFlex.Web/Areas/Admin/Models/ProductFormViewModel.cs`:

```csharp
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DangPhatFlex.Web.Areas.Admin.Models;

public class ProductFormViewModel
{
    public int Id { get; set; }

    [Required]
    public int ProductCategoryId { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Slug { get; set; }

    public string? Description { get; set; }

    public IFormFile? MainImage { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập alt text cho ảnh")]
    public string MainImageAlt { get; set; } = string.Empty;

    public IFormFile? DatasheetPdf { get; set; }

    public string? InnerDiameter { get; set; }
    public string? OuterDiameter { get; set; }
    public string? HoseType { get; set; }
    public string? MaxTemperature { get; set; }
    public string? MaxPressure { get; set; }
    public string? MinBendRadius { get; set; }
    public string? Standards { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}
```

- [ ] **Step 7: Implement `Admin.ProductsController`**

`src/DangPhatFlex.Web/Areas/Admin/Controllers/ProductsController.cs`:

```csharp
using DangPhatFlex.Web.Areas.Admin.Models;
using DangPhatFlex.Web.Data;
using DangPhatFlex.Web.Models;
using DangPhatFlex.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DangPhatFlex.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly AppDbContext _context;
    private readonly ISlugService _slugService;
    private readonly IFileUploadService _fileUploadService;

    public ProductsController(AppDbContext context, ISlugService slugService, IFileUploadService fileUploadService)
    {
        _context = context;
        _slugService = slugService;
        _fileUploadService = fileUploadService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Products.Include(p => p.ProductCategory).ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        ViewData["Categories"] = await _context.ProductCategories.ToListAsync();
        return View(new ProductFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Categories"] = await _context.ProductCategories.ToListAsync();
            return View(model);
        }

        var product = new Product
        {
            ProductCategoryId = model.ProductCategoryId,
            Name = model.Name,
            Slug = string.IsNullOrWhiteSpace(model.Slug) ? _slugService.GenerateSlug(model.Name) : model.Slug,
            Description = model.Description,
            MainImageAlt = model.MainImageAlt,
            InnerDiameter = model.InnerDiameter,
            OuterDiameter = model.OuterDiameter,
            HoseType = model.HoseType,
            MaxTemperature = model.MaxTemperature,
            MaxPressure = model.MaxPressure,
            MinBendRadius = model.MinBendRadius,
            Standards = model.Standards,
            MetaTitle = model.MetaTitle,
            MetaDescription = model.MetaDescription
        };

        if (model.MainImage is not null)
            product.MainImageUrl = await _fileUploadService.SaveAsync(model.MainImage, "products");

        if (model.DatasheetPdf is not null)
            product.DatasheetPdfUrl = await _fileUploadService.SaveAsync(model.DatasheetPdf, "datasheets");

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null)
            return NotFound();

        ViewData["Categories"] = await _context.ProductCategories.ToListAsync();
        return View(new ProductFormViewModel
        {
            Id = product.Id,
            ProductCategoryId = product.ProductCategoryId,
            Name = product.Name,
            Slug = product.Slug,
            Description = product.Description,
            MainImageAlt = product.MainImageAlt ?? string.Empty,
            InnerDiameter = product.InnerDiameter,
            OuterDiameter = product.OuterDiameter,
            HoseType = product.HoseType,
            MaxTemperature = product.MaxTemperature,
            MaxPressure = product.MaxPressure,
            MinBendRadius = product.MinBendRadius,
            Standards = product.Standards,
            MetaTitle = product.MetaTitle,
            MetaDescription = product.MetaDescription
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductFormViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        var product = await _context.Products.FindAsync(id);
        if (product is null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            ViewData["Categories"] = await _context.ProductCategories.ToListAsync();
            return View(model);
        }

        product.ProductCategoryId = model.ProductCategoryId;
        product.Name = model.Name;
        product.Slug = string.IsNullOrWhiteSpace(model.Slug) ? _slugService.GenerateSlug(model.Name) : model.Slug;
        product.Description = model.Description;
        product.MainImageAlt = model.MainImageAlt;
        product.InnerDiameter = model.InnerDiameter;
        product.OuterDiameter = model.OuterDiameter;
        product.HoseType = model.HoseType;
        product.MaxTemperature = model.MaxTemperature;
        product.MaxPressure = model.MaxPressure;
        product.MinBendRadius = model.MinBendRadius;
        product.Standards = model.Standards;
        product.MetaTitle = model.MetaTitle;
        product.MetaDescription = model.MetaDescription;

        if (model.MainImage is not null)
            product.MainImageUrl = await _fileUploadService.SaveAsync(model.MainImage, "products");

        if (model.DatasheetPdf is not null)
            product.DatasheetPdfUrl = await _fileUploadService.SaveAsync(model.DatasheetPdf, "datasheets");

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is not null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
```

- [ ] **Step 8: Implement views**

`src/DangPhatFlex.Web/Areas/Admin/Views/Products/Index.cshtml`:

```cshtml
@model List<DangPhatFlex.Web.Models.Product>

<section class="container py-4">
    <h1>Sản phẩm</h1>
    <a class="btn btn-brand mb-3" asp-action="Create">Thêm sản phẩm</a>
    <table class="table">
        <thead><tr><th>Tên</th><th>Danh mục</th><th>Slug</th><th></th></tr></thead>
        <tbody>
            @foreach (var p in Model)
            {
                <tr>
                    <td>@p.Name</td>
                    <td>@p.ProductCategory?.Name</td>
                    <td>@p.Slug</td>
                    <td>
                        <a asp-action="Edit" asp-route-id="@p.Id">Sửa</a>
                        <form asp-action="Delete" asp-route-id="@p.Id" method="post" class="d-inline">
                            <button type="submit" class="btn btn-link text-danger p-0 ms-2"
                                    onclick="return confirm('Xóa sản phẩm này?');">Xóa</button>
                        </form>
                    </td>
                </tr>
            }
        </tbody>
    </table>
</section>
```

`src/DangPhatFlex.Web/Areas/Admin/Views/Products/Create.cshtml`:

```cshtml
@model DangPhatFlex.Web.Areas.Admin.Models.ProductFormViewModel
@{
    var categories = ViewData["Categories"] as List<DangPhatFlex.Web.Models.ProductCategory> ?? new();
}

<section class="container py-4">
    <h1>Thêm sản phẩm</h1>
    <form asp-action="Create" method="post" enctype="multipart/form-data">
        <div asp-validation-summary="ModelOnly" class="text-danger"></div>
        <div class="mb-3">
            <label asp-for="ProductCategoryId" class="form-label">Danh mục</label>
            <select asp-for="ProductCategoryId" class="form-select" asp-items="@(new SelectList(categories, "Id", "Name"))"></select>
        </div>
        <div class="mb-3">
            <label asp-for="Name" class="form-label"></label>
            <input asp-for="Name" class="form-control" />
        </div>
        <div class="mb-3">
            <label asp-for="Slug" class="form-label">Slug (để trống để tự sinh)</label>
            <input asp-for="Slug" class="form-control" />
        </div>
        <div class="mb-3">
            <label asp-for="Description" class="form-label"></label>
            <textarea asp-for="Description" class="form-control" rows="4"></textarea>
        </div>
        <div class="mb-3">
            <label asp-for="MainImage" class="form-label">Ảnh chính</label>
            <input asp-for="MainImage" type="file" class="form-control" />
        </div>
        <div class="mb-3">
            <label asp-for="MainImageAlt" class="form-label">Alt text ảnh (bắt buộc)</label>
            <input asp-for="MainImageAlt" class="form-control" />
            <span asp-validation-for="MainImageAlt" class="text-danger"></span>
        </div>
        <div class="mb-3">
            <label asp-for="DatasheetPdf" class="form-label">Datasheet PDF</label>
            <input asp-for="DatasheetPdf" type="file" class="form-control" />
        </div>
        <div class="row">
            <div class="col-md-6 mb-3"><label asp-for="InnerDiameter" class="form-label"></label><input asp-for="InnerDiameter" class="form-control" /></div>
            <div class="col-md-6 mb-3"><label asp-for="OuterDiameter" class="form-label"></label><input asp-for="OuterDiameter" class="form-control" /></div>
            <div class="col-md-6 mb-3"><label asp-for="HoseType" class="form-label"></label><input asp-for="HoseType" class="form-control" /></div>
            <div class="col-md-6 mb-3"><label asp-for="MaxTemperature" class="form-label"></label><input asp-for="MaxTemperature" class="form-control" /></div>
            <div class="col-md-6 mb-3"><label asp-for="MaxPressure" class="form-label"></label><input asp-for="MaxPressure" class="form-control" /></div>
            <div class="col-md-6 mb-3"><label asp-for="MinBendRadius" class="form-label"></label><input asp-for="MinBendRadius" class="form-control" /></div>
            <div class="col-md-12 mb-3"><label asp-for="Standards" class="form-label"></label><input asp-for="Standards" class="form-control" /></div>
        </div>
        <div class="mb-3"><label asp-for="MetaTitle" class="form-label"></label><input asp-for="MetaTitle" class="form-control" /></div>
        <div class="mb-3"><label asp-for="MetaDescription" class="form-label"></label><textarea asp-for="MetaDescription" class="form-control"></textarea></div>
        <button type="submit" class="btn btn-brand">Lưu</button>
    </form>
</section>
```

`src/DangPhatFlex.Web/Areas/Admin/Views/Products/Edit.cshtml` — same form as `Create.cshtml`, with `asp-action="Edit" asp-route-id="@Model.Id"` and a hidden `<input type="hidden" asp-for="Id" />` added at the top of the `<form>`.

- [ ] **Step 9: Build and run full test suite**

Run: `dotnet build && dotnet test tests/DangPhatFlex.Web.Tests`
Expected: build succeeds, all tests pass.

- [ ] **Step 10: Commit**

```bash
git add src/DangPhatFlex.Web/Areas/Admin/Models src/DangPhatFlex.Web/Services/IFileUploadService.cs src/DangPhatFlex.Web/Services/FileUploadService.cs src/DangPhatFlex.Web/Areas/Admin/Controllers/ProductsController.cs src/DangPhatFlex.Web/Areas/Admin/Views/Products src/DangPhatFlex.Web/Program.cs tests/DangPhatFlex.Web.Tests/FileUploadServiceTests.cs
git commit -m "feat: add Admin Product CRUD with image/PDF upload and alt text requirement"
```

---

### Task 15: Admin CompanyInfo edit

**Files:**
- Create: `src/DangPhatFlex.Web/Areas/Admin/Controllers/CompanyInfoController.cs`
- Create: `src/DangPhatFlex.Web/Areas/Admin/Views/CompanyInfo/Edit.cshtml`
- Test: `tests/DangPhatFlex.Web.Tests/CompanyInfoAdminTests.cs`

**Interfaces:**
- Consumes: `AppDbContext` (Task 2)

- [ ] **Step 1: Write failing test**

`tests/DangPhatFlex.Web.Tests/CompanyInfoAdminTests.cs`:

```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class CompanyInfoAdminTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CompanyInfoAdminTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Edit_WithoutLogin_RedirectsToLogin()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var response = await client.GetAsync("/Admin/CompanyInfo/Edit");

        Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location!.ToString());
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter CompanyInfoAdminTests`
Expected: FAIL — 404, route doesn't exist.

- [ ] **Step 3: Implement `CompanyInfoController`**

`src/DangPhatFlex.Web/Areas/Admin/Controllers/CompanyInfoController.cs`:

```csharp
using DangPhatFlex.Web.Data;
using DangPhatFlex.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DangPhatFlex.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CompanyInfoController : Controller
{
    private readonly AppDbContext _context;

    public CompanyInfoController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Edit()
    {
        var info = await _context.CompanyInfos.FirstOrDefaultAsync();
        return View(info ?? new CompanyInfo());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CompanyInfo model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var existing = await _context.CompanyInfos.FindAsync(model.Id);
        if (existing is null)
        {
            _context.CompanyInfos.Add(model);
        }
        else
        {
            _context.Entry(existing).CurrentValues.SetValues(model);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Edit));
    }
}
```

- [ ] **Step 4: Implement view**

`src/DangPhatFlex.Web/Areas/Admin/Views/CompanyInfo/Edit.cshtml`:

```cshtml
@model DangPhatFlex.Web.Models.CompanyInfo

<section class="container py-4">
    <h1>Thông tin công ty</h1>
    <form asp-action="Edit" method="post">
        <input type="hidden" asp-for="Id" />
        <div asp-validation-summary="ModelOnly" class="text-danger"></div>
        <div class="mb-3"><label asp-for="LegalName" class="form-label"></label><input asp-for="LegalName" class="form-control" /></div>
        <div class="mb-3"><label asp-for="BrandName" class="form-label"></label><input asp-for="BrandName" class="form-control" /></div>
        <div class="mb-3"><label asp-for="Tagline" class="form-label"></label><input asp-for="Tagline" class="form-control" /></div>
        <div class="mb-3"><label asp-for="AboutContent" class="form-label"></label><textarea asp-for="AboutContent" class="form-control" rows="6">@Model.AboutContent</textarea></div>
        <div class="mb-3"><label asp-for="Address" class="form-label"></label><input asp-for="Address" class="form-control" /></div>
        <div class="mb-3"><label asp-for="Hotline" class="form-label"></label><input asp-for="Hotline" class="form-control" /></div>
        <div class="mb-3"><label asp-for="Email" class="form-label"></label><input asp-for="Email" class="form-control" /></div>
        <div class="mb-3"><label asp-for="MapEmbedUrl" class="form-label"></label><input asp-for="MapEmbedUrl" class="form-control" /></div>
        <div class="mb-3"><label asp-for="CoreValueFast" class="form-label"></label><input asp-for="CoreValueFast" class="form-control" /></div>
        <div class="mb-3"><label asp-for="CoreValueBest" class="form-label"></label><input asp-for="CoreValueBest" class="form-control" /></div>
        <div class="mb-3"><label asp-for="CoreValueCompetitivePrice" class="form-label"></label><input asp-for="CoreValueCompetitivePrice" class="form-control" /></div>
        <button type="submit" class="btn btn-brand">Lưu</button>
    </form>
</section>
```

- [ ] **Step 5: Run test to verify it passes**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter CompanyInfoAdminTests`
Expected: `Passed! - Failed: 0`

- [ ] **Step 6: Commit**

```bash
git add src/DangPhatFlex.Web/Areas/Admin/Controllers/CompanyInfoController.cs src/DangPhatFlex.Web/Areas/Admin/Views/CompanyInfo tests/DangPhatFlex.Web.Tests/CompanyInfoAdminTests.cs
git commit -m "feat: add Admin company info edit page"
```

---

### Task 16: Admin ContactSubmission list

**Files:**
- Create: `src/DangPhatFlex.Web/Areas/Admin/Controllers/ContactSubmissionsController.cs`
- Create: `src/DangPhatFlex.Web/Areas/Admin/Views/ContactSubmissions/Index.cshtml`
- Test: `tests/DangPhatFlex.Web.Tests/ContactSubmissionsAdminTests.cs`

**Interfaces:**
- Consumes: `AppDbContext` (Task 2), `ContactSubmission` records written by Task 10

- [ ] **Step 1: Write failing test**

`tests/DangPhatFlex.Web.Tests/ContactSubmissionsAdminTests.cs`:

```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class ContactSubmissionsAdminTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ContactSubmissionsAdminTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Index_WithoutLogin_RedirectsToLogin()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var response = await client.GetAsync("/Admin/ContactSubmissions");

        Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location!.ToString());
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter ContactSubmissionsAdminTests`
Expected: FAIL — 404, route doesn't exist.

- [ ] **Step 3: Implement `ContactSubmissionsController`**

`src/DangPhatFlex.Web/Areas/Admin/Controllers/ContactSubmissionsController.cs`:

```csharp
using DangPhatFlex.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DangPhatFlex.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ContactSubmissionsController : Controller
{
    private readonly AppDbContext _context;

    public ContactSubmissionsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.ContactSubmissions
            .OrderByDescending(c => c.SubmittedAt)
            .ToListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkProcessed(int id)
    {
        var submission = await _context.ContactSubmissions.FindAsync(id);
        if (submission is not null)
        {
            submission.IsProcessed = true;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
```

- [ ] **Step 4: Implement view**

`src/DangPhatFlex.Web/Areas/Admin/Views/ContactSubmissions/Index.cshtml`:

```cshtml
@model List<DangPhatFlex.Web.Models.ContactSubmission>

<section class="container py-4">
    <h1>Liên hệ khách gửi</h1>
    <table class="table">
        <thead><tr><th>Ngày</th><th>Họ tên</th><th>SĐT</th><th>Email</th><th>Nội dung</th><th>Trạng thái</th><th></th></tr></thead>
        <tbody>
            @foreach (var c in Model)
            {
                <tr class="@(c.IsProcessed ? "" : "table-warning")">
                    <td>@c.SubmittedAt.ToString("dd/MM/yyyy HH:mm")</td>
                    <td>@c.FullName</td>
                    <td>@c.Phone</td>
                    <td>@c.Email</td>
                    <td>@c.Message</td>
                    <td>@(c.IsProcessed ? "Đã xử lý" : "Mới")</td>
                    <td>
                        @if (!c.IsProcessed)
                        {
                            <form asp-action="MarkProcessed" asp-route-id="@c.Id" method="post">
                                <button type="submit" class="btn btn-sm btn-outline-success">Đánh dấu đã xử lý</button>
                            </form>
                        }
                    </td>
                </tr>
            }
        </tbody>
    </table>
</section>
```

- [ ] **Step 5: Run test to verify it passes**

Run: `dotnet test tests/DangPhatFlex.Web.Tests --filter ContactSubmissionsAdminTests`
Expected: `Passed! - Failed: 0`

- [ ] **Step 6: Commit**

```bash
git add src/DangPhatFlex.Web/Areas/Admin/Controllers/ContactSubmissionsController.cs src/DangPhatFlex.Web/Areas/Admin/Views/ContactSubmissions tests/DangPhatFlex.Web.Tests/ContactSubmissionsAdminTests.cs
git commit -m "feat: add Admin contact submissions list"
```

---

### Task 17: Final verification pass

**Files:**
- None created — verification only

- [ ] **Step 1: Run full test suite**

Run: `dotnet test`
Expected: all tests across `AppDbContextTests`, `SlugServiceTests`, `DbSeederTests`, `HomeControllerTests`, `AboutControllerTests`, `ProductsControllerTests`, `ProductDetailTests`, `ContactControllerTests`, `SeoControllerTests`, `AdminAuthTests`, `ProductCategoriesAdminTests`, `FileUploadServiceTests`, `CompanyInfoAdminTests`, `ContactSubmissionsAdminTests` pass with `Failed: 0`.

- [ ] **Step 2: Manual smoke test of the running app**

```bash
dotnet run --project src/DangPhatFlex.Web
```

Visit in a browser and confirm:
- `/` shows hero, 3 core values, featured products, view-source contains `application/ld+json`
- `/Public/About` shows company legal name and address
- `/Public/Products` lists "Đăng Phát Flex DP25"; clicking through reaches `/Public/Products/khop-noi-mem-inox/dang-phat-flex-dp25` with the full variant table
- `/Public/Contact` form submits successfully and shows the success alert
- `/sitemap.xml` and `/robots.txt` return valid content
- `/Identity/Account/Login` logs in with the seeded admin credentials from `appsettings.Development.json`, after which `/Admin/Dashboard` is reachable and `/Admin/ProductCategories`, `/Admin/Products`, `/Admin/CompanyInfo/Edit`, `/Admin/ContactSubmissions` all work end-to-end (create/edit/delete a test category and product)

Stop the server with Ctrl+C when done.

- [ ] **Step 3: Commit any final fixes found during manual testing**

If manual testing surfaces bugs, fix them, re-run `dotnet test`, then:

```bash
git add -A
git commit -m "fix: address issues found during final verification pass"
```

(Skip this step if no issues were found.)
