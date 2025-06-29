using Do_anNhom7.Models;
using Do_anNhom7.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;

namespace Do_anNhom7.Controllers
{
    public class AccountController : Controller
    {
        private QLStoreQuanAoEntities db = new QLStoreQuanAoEntities();
        //Get account/register
        public ActionResult Register()
        {
            return View();
        }
        //post:account/register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                // kiem tra xem ten dang nhap da ton tai chua
                var existingUser = db.Users.SingleOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại!");
                    return View(model);
                }
                //Nếu chưa tồn tại thì tạo bản ghi thông tin tài khoản trong bảng user
                var user = new User()
                {
                    Username = model.Username,
                    Password = model.Password, //:nen ma khoa Mk truoc khi luu
                    UserRole = "Customer"
                };
                db.Users.Add(user);
                //và tạo bản ghi thông tin khách hàng trong customer
                var customer = new Customer()
                {
                    CustomerName = model.CustomerName,
                    CustomerEmail = model.CustomerEmail,
                    CustomerPhone = model.CustomerPhone,
                    CustomerAddress = model.CustomerAddress,
                    Username = model.Username,
                };
                db.Customers.Add(customer);
                //luu thong tin tai khoan va thong tin khach hang vao csdl
                db.SaveChanges();
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }
        //get:Account/login
        public ActionResult Login()
        {
            return View();
        }
        //Post:Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem người dùng có tồn tại với tên đăng nhập và mật khẩu không
                var user = db.Users.SingleOrDefault(u => u.Username == model.Username && u.Password == model.Password);

                if (user != null)
                {
                    // Lưu thông tin đăng nhập vào session và cookie xác thực
                    Session["Username"] = user.Username;
                    Session["UserRole"] = user.UserRole;

                    // Tạo cookie xác thực
                    FormsAuthentication.SetAuthCookie(user.Username,true);

                    // Kiểm tra vai trò người dùng và chuyển hướng tương ứng
                    if (user.UserRole == "Admin")  // Kiểm tra nếu là Admin
                    {
                        return RedirectToAction("Home", "Admin");  // Chuyển hướng đến trang quản trị (có thể thay "Admin" bằng tên controller của bạn)
                    }
                    else if (user.UserRole == "Customer")  // Kiểm tra nếu là Customer
                    {
                        return RedirectToAction("Homepage", "Home");  // Chuyển hướng đến trang chủ người dùng
                    }
                }
                else
                {
                    // Nếu không tìm thấy người dùng, hiển thị thông báo lỗi
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }

            return View(model);  // Nếu model không hợp lệ, quay lại trang đăng nhập
        }

        public ActionResult Logout()
        {
            // Xóa session
            Session.Clear();

            // Xóa cookie xác thực
            FormsAuthentication.SignOut();

            return RedirectToAction("Homepage", "Home");
        }

        [Authorize]
        public ActionResult Details()
        {
            // Kiểm tra nếu người dùng chưa đăng nhập thông qua session
            if (Session["Username"] == null)
            {
                // Nếu chưa đăng nhập, chuyển hướng về trang đăng nhập
                return RedirectToAction("Login", "Account");
            }

            string username = Session["Username"].ToString(); // Lấy tên người dùng từ session
            var customer = db.Customers.SingleOrDefault(c => c.Username == username);

            if (customer == null)
            {
                // Nếu không tìm thấy thông tin người dùng, trả về lỗi
                return HttpNotFound("Không tìm thấy thông tin người dùng.");
            }

            // Trả về thông tin người dùng nếu tìm thấy
            return View(customer);
        }

        public ActionResult Edit()
        {
            // Lấy thông tin người dùng từ session
            string username = Session["Username"]?.ToString();

            if (username == null)
            {
                return RedirectToAction("Login", "Account"); // Nếu chưa đăng nhập, chuyển hướng về trang login
            }

            var customer = db.Customers.SingleOrDefault(c => c.Username == username);

            if (customer == null)
            {
                return HttpNotFound("Không tìm thấy thông tin người dùng.");
            }

            // Trả về view với thông tin người dùng hiện tại
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem người dùng đã đăng nhập chưa
                string username = Session["Username"]?.ToString();
                if (username == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var customer = db.Customers.SingleOrDefault(c => c.Username == username);
                if (customer == null)
                {
                    return HttpNotFound("Không tìm thấy thông tin người dùng.");
                }

                // Cập nhật thông tin người dùng
                customer.CustomerName = model.CustomerName;
                customer.CustomerPhone = model.CustomerPhone;
                customer.CustomerEmail = model.CustomerEmail;
                customer.CustomerAddress = model.CustomerAddress;

                // Lưu thông tin vào database
                db.SaveChanges();

                // Quay lại trang chi tiết thông tin người dùng
                return RedirectToAction("Details");
            }

            return View(model); // Nếu không hợp lệ, quay lại view chỉnh sửa
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OrderHistory()
        {
            // Lấy tên người dùng từ session
            string username = Session["Username"]?.ToString();

            if (username == null)
            {
                // Nếu người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Lấy thông tin khách hàng
                var customer = db.Customers.SingleOrDefault(c => c.Username == username);
                if (customer == null)
                {
                    return HttpNotFound("Không tìm thấy thông tin người dùng.");
                }

                // Lấy danh sách đơn hàng của khách hàng
                var orders = db.Orders
                    .Include(o => o.OrderDetails) // Lấy luôn thông tin chi tiết đơn hàng
                    .Where(o => o.CustomerID == customer.CustomerID)
                    .OrderByDescending(o => o.OrderDate) // Sắp xếp theo ngày đặt hàng mới nhất
                    .ToList();

                // Kiểm tra nếu không có đơn hàng nào
                if (!orders.Any())
                {
                    ViewBag.Message = "Bạn chưa có đơn hàng nào.";
                }

                return View(orders);
            }
            catch (Exception ex)
            {
                // Log lỗi (nếu cần) và hiển thị thông báo lỗi
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi tải dữ liệu. Vui lòng thử lại sau.";
                return View(new List<Order>());
            }
        }
    }
}

