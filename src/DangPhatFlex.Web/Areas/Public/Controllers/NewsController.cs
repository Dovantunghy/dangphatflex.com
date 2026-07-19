using DangPhatFlex.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DangPhatFlex.Web.Areas.Public.Controllers;

[Area("Public")]
public class NewsController : Controller
{
    private readonly AppDbContext _context;

    public NewsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["MetaTitle"] = "Tin tức - Kiến thức khớp nối mềm inox chữa cháy | Đăng Phát Flex";
        ViewData["MetaDescription"] = "Cẩm nang, hướng dẫn chọn ống mềm nối đầu phun sprinkler và tin tức sản phẩm khớp nối mềm inox từ Đăng Phát Flex.";

        var articles = await _context.NewsArticles
            .OrderByDescending(n => n.PublishedAt)
            .ToListAsync();
        return View(articles);
    }

    [Route("Public/News/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var article = await _context.NewsArticles.FirstOrDefaultAsync(n => n.Slug == slug);
        if (article is null)
            return NotFound();

        var related = await _context.NewsArticles
            .Where(n => n.Id != article.Id)
            .OrderByDescending(n => n.PublishedAt)
            .Take(3)
            .ToListAsync();
        ViewData["RelatedArticles"] = related;

        return View(article);
    }
}
