using DangPhatFlex.Web.Areas.Admin.Models;
using DangPhatFlex.Web.Data;
using DangPhatFlex.Web.Models;
using DangPhatFlex.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DangPhatFlex.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class NewsController : Controller
{
    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

    private readonly AppDbContext _context;
    private readonly ISlugService _slugService;
    private readonly IFileUploadService _fileUploadService;

    public NewsController(AppDbContext context, ISlugService slugService, IFileUploadService fileUploadService)
    {
        _context = context;
        _slugService = slugService;
        _fileUploadService = fileUploadService;
    }

    private static bool HasAllowedExtension(IFormFile file, IReadOnlyCollection<string> allowedExtensions)
    {
        var extension = Path.GetExtension(file.FileName);
        return allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.NewsArticles.OrderByDescending(n => n.PublishedAt).ToListAsync());
    }

    public IActionResult Create()
    {
        return View(new NewsArticleFormViewModel { PublishedAt = DateTime.UtcNow });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NewsArticleFormViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Slug) && !string.IsNullOrWhiteSpace(model.Title))
        {
            model.Slug = _slugService.GenerateSlug(model.Title);
            ModelState.Remove(nameof(model.Slug));
        }

        if (model.CoverImage is not null && !HasAllowedExtension(model.CoverImage, AllowedImageExtensions))
            ModelState.AddModelError(nameof(model.CoverImage), "Chỉ chấp nhận file ảnh (jpg, png, webp, gif).");

        if (!ModelState.IsValid)
            return View(model);

        var article = new NewsArticle
        {
            Title = model.Title,
            Slug = model.Slug!,
            Summary = model.Summary,
            Content = model.Content,
            CoverImageAlt = model.CoverImageAlt,
            PublishedAt = model.PublishedAt ?? DateTime.UtcNow,
            MetaTitle = model.MetaTitle,
            MetaDescription = model.MetaDescription
        };

        if (model.CoverImage is not null)
            article.CoverImageUrl = await _fileUploadService.SaveAsync(model.CoverImage, "news");

        _context.NewsArticles.Add(article);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var article = await _context.NewsArticles.FindAsync(id);
        if (article is null)
            return NotFound();

        return View(new NewsArticleFormViewModel
        {
            Id = article.Id,
            Title = article.Title,
            Slug = article.Slug,
            Summary = article.Summary,
            Content = article.Content,
            CoverImageAlt = article.CoverImageAlt,
            PublishedAt = article.PublishedAt,
            MetaTitle = article.MetaTitle,
            MetaDescription = article.MetaDescription
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NewsArticleFormViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        var article = await _context.NewsArticles.FindAsync(id);
        if (article is null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(model.Slug) && !string.IsNullOrWhiteSpace(model.Title))
        {
            model.Slug = _slugService.GenerateSlug(model.Title);
            ModelState.Remove(nameof(model.Slug));
        }

        if (model.CoverImage is not null && !HasAllowedExtension(model.CoverImage, AllowedImageExtensions))
            ModelState.AddModelError(nameof(model.CoverImage), "Chỉ chấp nhận file ảnh (jpg, png, webp, gif).");

        if (!ModelState.IsValid)
            return View(model);

        article.Title = model.Title;
        article.Slug = model.Slug!;
        article.Summary = model.Summary;
        article.Content = model.Content;
        article.CoverImageAlt = model.CoverImageAlt;
        article.PublishedAt = model.PublishedAt ?? article.PublishedAt;
        article.MetaTitle = model.MetaTitle;
        article.MetaDescription = model.MetaDescription;

        if (model.CoverImage is not null)
            article.CoverImageUrl = await _fileUploadService.SaveAsync(model.CoverImage, "news");

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var article = await _context.NewsArticles.FindAsync(id);
        if (article is not null)
        {
            _context.NewsArticles.Remove(article);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
