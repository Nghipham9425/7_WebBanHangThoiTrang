using Do_anNhom7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Do_anNhom7.Models.ViewModel;
using PagedList;
using System.Drawing;

namespace Do_anNhom7.Controllers
{
    public class HomeController : Controller
    {
        QLStoreQuanAoEntities db = new QLStoreQuanAoEntities();
        public ActionResult Address()
        {
            return View();
        }
        public ActionResult Homepage(string searchTerm, int? page)
        {
            var model = new HomeProductVM();
            var products = db.Products.AsQueryable();
            //tim kiem sp dua tren tu khoa
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;
                products = products.Where(p => p.ProductName.Contains(searchTerm) ||
                p.ProductDescription.Contains(searchTerm) ||
                p.Category.CategoryName.Contains(searchTerm));
            }
            // doan code lien quan toi phan trang
            // lay so trang hien tai (Mac dinh la trang 1 neu khong co gia tri)
            int pageNumber = page ?? 1;
            int pageSize = 7; //so sp moi trang

            //lay top 10 sp ban chay nhat
            model.FeaturedProducts = products.OrderByDescending(p => p.OrderDetails.Count()).Take(6).ToList();

            //lay 20 sp ban e nhat va phan trang
            model.NewProducts = products.OrderBy(p => p.OrderDetails.Count()).Take(20).ToPagedList(pageNumber, pageSize);
            return View(model);
        }
        // GET: Home/ProductDetail/5

        public ActionResult ProductDetails(int? id, int? quantity,string size,int?page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Product pro = db.Products.Find(id);
            if (pro == null)
            {
                return HttpNotFound();
            }

            // Lấy các sản phẩm cùng danh mục nhưng không bao gồm sản phẩm hiện tại
            var products = db.Products.Where(p => p.CategoryID == pro.CategoryID && p.ProductID != pro.ProductID).AsQueryable();
            ProductDetailsVM model = new ProductDetailsVM();
           //doan code lien quan toi phan trang
           //mac dinh la 1
           int pageNumber = page ?? 1;
            int pageSize = model.PageSize;// so sp moi trang
            model.product = pro;
            model.RelatedProducts = products.OrderBy(p => p.ProductID).Take(8).ToList();
            model.TopProducts = products.OrderByDescending(p => p.OrderDetails.Count()).Take(8).ToPagedList(pageNumber, pageSize);

            if (quantity.HasValue)
            {
                model.quantity = quantity.Value;
            }
            // Xử lý việc chọn size
            if (!string.IsNullOrEmpty(size))
            {
                model.SelectedSize = size;
            }

            // Nếu không có size được chọn, mặc định là "M"
            if (string.IsNullOrEmpty(model.SelectedSize))
            {
                model.SelectedSize = "M"; // Hoặc bất kỳ giá trị mặc định nào
            }

            // Cung cấp danh sách các kích thước có sẵn
            model.AvailableSizes = new List<string> { "S", "M", "L"}; // Ví dụ các size có sẵn
            return View(model);
        }
        [HttpPost]
        public ActionResult Chinhsach()
        {
            return View();
        }
        public ActionResult Gioithieu()
        {
            return View();
        }
        public ActionResult Lienhe()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
    }
}