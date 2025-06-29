using Do_anNhom7.Filters;
using Do_anNhom7.Models;
using System.Linq;
using System.Web.Mvc;

namespace Do_anNhom7.Areas.Admin.Controllers
{
    [AdminRoleFilter]  // Kiểm tra vai trò Admin từ filter
    [Authorize]  // Đảm bảo rằng người dùng đã đăng nhập
    public class HomeController : Controller
    {
        private QLStoreQuanAoEntities db = new QLStoreQuanAoEntities();

        // GET: Admin/Home
        public ActionResult Index()
        {
            // Không cần kiểm tra lại vai trò Admin nữa, vì filter đã làm việc này
            return View();  // Nếu là Admin, cho phép truy cập vào trang admin
        }
    }
}
