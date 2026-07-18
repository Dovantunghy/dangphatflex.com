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
