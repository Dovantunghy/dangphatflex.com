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
        if (string.IsNullOrWhiteSpace(model.Slug) && !string.IsNullOrWhiteSpace(model.Name))
        {
            model.Slug = _slugService.GenerateSlug(model.Name);
            ModelState.Remove(nameof(model.Slug));
        }

        if (!ModelState.IsValid)
        {
            ViewData["Categories"] = await _context.ProductCategories.ToListAsync();
            return View(model);
        }

        var product = new Product
        {
            ProductCategoryId = model.ProductCategoryId,
            Name = model.Name,
            Slug = model.Slug!,
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

        if (string.IsNullOrWhiteSpace(model.Slug) && !string.IsNullOrWhiteSpace(model.Name))
        {
            model.Slug = _slugService.GenerateSlug(model.Name);
            ModelState.Remove(nameof(model.Slug));
        }

        if (!ModelState.IsValid)
        {
            ViewData["Categories"] = await _context.ProductCategories.ToListAsync();
            return View(model);
        }

        product.ProductCategoryId = model.ProductCategoryId;
        product.Name = model.Name;
        product.Slug = model.Slug!;
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
