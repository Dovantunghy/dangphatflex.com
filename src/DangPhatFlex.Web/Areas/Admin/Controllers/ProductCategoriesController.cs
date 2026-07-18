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
        if (string.IsNullOrWhiteSpace(model.Slug) && !string.IsNullOrWhiteSpace(model.Name))
        {
            model.Slug = _slugService.GenerateSlug(model.Name);
            ModelState.Remove(nameof(model.Slug));
        }

        if (!ModelState.IsValid)
            return View(model);

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

        if (string.IsNullOrWhiteSpace(model.Slug) && !string.IsNullOrWhiteSpace(model.Name))
        {
            model.Slug = _slugService.GenerateSlug(model.Name);
            ModelState.Remove(nameof(model.Slug));
        }

        if (!ModelState.IsValid)
            return View(model);

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
