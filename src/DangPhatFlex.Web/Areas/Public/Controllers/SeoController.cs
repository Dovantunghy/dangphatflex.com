using System.Text;
using DangPhatFlex.Web.Data;
using DangPhatFlex.Web.Services;
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
        // (url, lastmod) — lastmod is null for pages with no tracked last-modified value; those
        // are rendered without a <lastmod> tag rather than a misleading "checked today" date.
        var urls = new List<(string Url, DateTime? LastMod)>
        {
            ($"{baseUrl}/", null),
            ($"{baseUrl}/gioi-thieu", null),
            ($"{baseUrl}/san-pham", null),
            ($"{baseUrl}/tin-tuc", null),
            ($"{baseUrl}/lien-he", null),
        };

        var products = await _context.Products.Include(p => p.ProductCategory).ToListAsync();
        urls.AddRange(products.Select(p => ($"{baseUrl}/san-pham/{p.ProductCategory!.Slug}/{p.Slug}", (DateTime?)null)));

        var articles = await _context.NewsArticles.ToListAsync();
        urls.AddRange(articles.Select(a => ($"{baseUrl}/tin-tuc/{a.Slug}", (DateTime?)a.PublishedAt)));

        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
        foreach (var (url, lastMod) in urls)
        {
            sb.AppendLine("<url>");
            sb.AppendLine($"<loc>{url}</loc>");
            if (lastMod.HasValue)
                sb.AppendLine($"<lastmod>{lastMod.Value:yyyy-MM-dd}</lastmod>");
            sb.AppendLine("</url>");
        }
        sb.AppendLine("</urlset>");

        return Content(sb.ToString(), "application/xml", Encoding.UTF8);
    }

    // IndexNow key-verification file — must be served at /{key}.txt with the key as the body.
    // Bing/Yandex fetch this once to confirm the site controls the domain before accepting pings.
    [Route("/" + IndexNowService.Key + ".txt")]
    public IActionResult IndexNowKey() => Content(IndexNowService.Key, "text/plain", Encoding.UTF8);

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
