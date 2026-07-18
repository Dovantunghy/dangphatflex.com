using DangPhatFlex.Web.Areas.Public.Models;
using DangPhatFlex.Web.Data;
using DangPhatFlex.Web.Models;
using DangPhatFlex.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DangPhatFlex.Web.Areas.Public.Controllers;

[Area("Public")]
public class ContactController : Controller
{
    private readonly AppDbContext _context;
    private readonly IEmailSender _emailSender;

    public ContactController(AppDbContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    public IActionResult Index()
    {
        ViewData["MetaTitle"] = "Liên hệ | Đăng Phát Flex";
        ViewData["MetaDescription"] = "Liên hệ CÔNG TY TNHH CƠ ĐIỆN ĐĂNG PHÁT để được tư vấn khớp nối mềm inox cho hệ thống chữa cháy.";
        return View(new ContactFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _context.ContactSubmissions.Add(new ContactSubmission
        {
            FullName = model.FullName,
            Phone = model.Phone,
            Email = model.Email,
            Message = model.Message,
            SubmittedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var company = await _context.CompanyInfos.FirstOrDefaultAsync();
        if (company is not null)
        {
            await _emailSender.SendAsync(
                company.Email,
                $"Liên hệ mới từ {model.FullName}",
                $"SĐT: {model.Phone}\nEmail: {model.Email}\nNội dung: {model.Message}");
        }

        TempData["ContactSuccess"] = "Cảm ơn bạn đã liên hệ. Chúng tôi sẽ phản hồi sớm nhất!";
        return RedirectToAction(nameof(Index));
    }
}
