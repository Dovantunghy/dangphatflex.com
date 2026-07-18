using DangPhatFlex.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DangPhatFlex.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ContactSubmissionsController : Controller
{
    private readonly AppDbContext _context;

    public ContactSubmissionsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.ContactSubmissions
            .OrderByDescending(c => c.SubmittedAt)
            .ToListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkProcessed(int id)
    {
        var submission = await _context.ContactSubmissions.FindAsync(id);
        if (submission is not null)
        {
            submission.IsProcessed = true;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
