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
        ViewData["CategoryCount"] = await _context.ProductCategories.CountAsync();
        ViewData["NewsCount"] = await _context.NewsArticles.CountAsync();
        ViewData["NewContactCount"] = await _context.ContactSubmissions.CountAsync(c => !c.IsProcessed);
        ViewData["RecentContacts"] = await _context.ContactSubmissions
            .OrderByDescending(c => c.SubmittedAt)
            .Take(5)
            .ToListAsync();
        return View();
    }
}
