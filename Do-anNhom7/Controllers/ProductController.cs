using Do_anNhom7.Models;
using Do_anNhom7.Models.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Do_anNhom7.Controllers
{
    public class ProductController : Controller
    {
        QLStoreQuanAoEntities db=new QLStoreQuanAoEntities();
        // GET: Product
        public ActionResult Sanphammoi()
        {
            var products = db.Products.ToList();
            return View(products); // Truyền danh sách sản phẩm vào view
        }

        public ActionResult aonu()
        {
            // Lấy tất cả sản phẩm có CategoryID = 2 và tên sản phẩm chứa từ "áo"
            var products = db.Products
                             .Where(p => p.CategoryID == 2 && p.ProductName.Contains("Áo"))
                             .ToList();
            return View(products); // Truyền danh sách sản phẩm vào view
        }

        public ActionResult aonam()
        {
            // Lấy tất cả sản phẩm có CategoryID = 3
            var products = db.Products.Where(p => p.CategoryID == 10 && p.ProductName.Contains("Áo")).ToList();
            return View(products); // Truyền danh sách sản phẩm vào view
        }
        public ActionResult quannu()
        {
            // Lấy tất cả sản phẩm có CategoryID = 2 và tên sản phẩm chứa từ "áo"
            var products = db.Products
                             .Where(p => p.CategoryID == 2 && p.ProductName.Contains("Quần"))
                             .ToList();
            return View(products); // Truyền danh sách sản phẩm vào view
        }
    
        public ActionResult quannam()
        {
            // Lấy tất cả sản phẩm có CategoryID = 3
            var products = db.Products.Where(p => p.CategoryID == 10 && p.ProductName.Contains("Quần")).ToList();
            return View(products); // Truyền danh sách sản phẩm vào view
        }
        public ActionResult vay()
        {
            // Lấy tất cả sản phẩm có CategoryID = 2 và tên sản phẩm chứa từ "áo"
            var products = db.Products
                             .Where(p => p.CategoryID == 2 && p.ProductName.Contains("Váy"))
                             .ToList();
            return View(products); // Truyền danh sách sản phẩm vào view
        }

        public ActionResult Nu()
        {
            var products = db.Products.Where(p => p.CategoryID == 2).ToList();
            return View(products); // Truyền danh sách sản phẩm vào view
        }
        public ActionResult Nam()
        {
            // Lấy tất cả sản phẩm có CategoryID = 10
            var products = db.Products.Where(p => p.CategoryID == 10).ToList();
            return View(products); // Truyền danh sách sản phẩm vào view
        }
        public ActionResult Phukien()
        {
            // Lấy tất cả sản phẩm có CategoryID = 11
            var products = db.Products.Where(p => p.CategoryID == 11).ToList();
            return View(products); // Truyền danh sách sản phẩm vào view
        }
        public ActionResult tuisach()
        {
            // Lấy tất cả sản phẩm có CategoryID = 11 và có tên túi sách
            var products = db.Products.Where(p => p.CategoryID == 11 && p.ProductName.Contains("Túi")).ToList();
            return View(products); // Truyền danh sách sản phẩm vào view
        }
        public ActionResult balo()
        {
            // Lấy tất cả sản phẩm có CategoryID = 11 và có tên balo
            var products = db.Products.Where(p => p.CategoryID == 11 && p.ProductName.Contains("Balo")).ToList();
            return View(products); // Truyền danh sách sản phẩm vào view
        }
        public ActionResult ProductDetails(int? id, int? quantity, string size, int? page)
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
            model.AvailableSizes = new List<string> { "S", "M", "L" }; // Ví dụ các size có sẵn
            return View(model);
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}