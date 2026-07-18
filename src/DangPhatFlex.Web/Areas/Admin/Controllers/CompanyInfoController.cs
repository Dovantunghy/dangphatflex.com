using DangPhatFlex.Web.Data;
using DangPhatFlex.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DangPhatFlex.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CompanyInfoController : Controller
{
    private readonly AppDbContext _context;

    public CompanyInfoController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Edit()
    {
        var info = await _context.CompanyInfos.FirstOrDefaultAsync();
        return View(info ?? new CompanyInfo());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CompanyInfo model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var existing = await _context.CompanyInfos.FindAsync(model.Id);
        if (existing is null)
        {
            _context.CompanyInfos.Add(model);
        }
        else
        {
            _context.Entry(existing).CurrentValues.SetValues(model);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Edit));
    }
}
