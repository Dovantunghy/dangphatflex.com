using DangPhatFlex.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DangPhatFlex.Web.Areas.Public.Controllers;

[Area("Public")]
public class AboutController : Controller
{
    private readonly AppDbContext _context;

    public AboutController(AppDbContext context)
    {
        _context = context;
    }

    [Route("/gioi-thieu")]
    public async Task<IActionResult> Index()
    {
        var company = await _context.CompanyInfos.FirstOrDefaultAsync();
        ViewData["MetaTitle"] = $"Giới thiệu công ty - {company?.BrandName}";
        ViewData["MetaDescription"] = "Tìm hiểu về CÔNG TY TNHH CƠ ĐIỆN ĐĂNG PHÁT, đơn vị sản xuất và phân phối khớp nối mềm inox uy tín.";
        return View(company);
    }
}
