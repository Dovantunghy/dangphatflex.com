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
        var urls = new List<string> { $"{baseUrl}/", $"{baseUrl}/gioi-thieu", $"{baseUrl}/san-pham", $"{baseUrl}/tin-tuc", $"{baseUrl}/lien-he" };

        var products = await _context.Products.Include(p => p.ProductCategory).ToListAsync();
        urls.AddRange(products.Select(p => $"{baseUrl}/san-pham/{p.ProductCategory!.Slug}/{p.Slug}"));

        var articles = await _context.NewsArticles.ToListAsync();
        urls.AddRange(articles.Select(a => $"{baseUrl}/tin-tuc/{a.Slug}"));

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

    [Route("/robots.txt")]
    public IActionResult Robots()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var sb = new StringBuilder();
        sb.AppendLine("User-agent: *");
        sb.AppendLine("Allow: /");
        sb.AppendLine($"Sitemap: {baseUrl}/sitemap.xml");

        return Content(sb.ToString(), "text/plain", Encoding.UTF8);
    }
}
