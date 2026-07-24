using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DangPhatFlex.Web.Areas.Admin.Controllers;

/// <summary>
/// Route mặc định của area là "{area:exists}/{controller=Home}/{action=Index}", nên "/Admin"
/// trỏ tới Admin/Home/Index. Trước khi có controller này, "/Admin" trả về 404; giờ nó chuyển
/// tiếp sang bảng điều khiển.
/// </summary>
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction("Index", "Dashboard");
}
