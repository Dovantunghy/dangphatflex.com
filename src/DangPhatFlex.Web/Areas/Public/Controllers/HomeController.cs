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
