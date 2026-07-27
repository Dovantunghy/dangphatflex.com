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

    [Route("/san-pham")]
    public async Task<IActionResult> Index(string? categorySlug)
    {
        var query = _context.Products.Include(p => p.ProductCategory).AsQueryable();
        if (!string.IsNullOrEmpty(categorySlug))
            query = query.Where(p => p.ProductCategory!.Slug == categorySlug);

        ViewData["MetaTitle"] = "Ống mềm nối đầu phun sprinkler DP25UB/DP25B | Đăng Phát Flex";
        ViewData["MetaDescription"] = "Danh sách ống mềm nối đầu phun sprinkler, dây mềm nối đầu phun sprinkler DP25UB, DP25B sản xuất theo tiêu chuẩn UL/FM/TCVN cho hệ thống chữa cháy.";
        ViewData["Categories"] = await _context.ProductCategories.ToListAsync();
        return View(await query.ToListAsync());
    }

    [Route("/san-pham/{categorySlug}/{productSlug}")]
    public async Task<IActionResult> Detail(string categorySlug, string productSlug)
    {
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
