using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Do_anNhom7.Filters;
using Do_anNhom7.Models;
using Do_anNhom7.Models.ViewModel;
using PagedList;

namespace Do_anNhom7.Areas.Admin.Controllers
{
    [AdminRoleFilter]
    public class ProductsController : Controller
    {
        private QLStoreQuanAoEntities db = new QLStoreQuanAoEntities();

        // GET: Admin/Products
        public ActionResult Index(string searchTerm,decimal? minPrice,decimal? maxPrice,string sortOrder,int? page)
        {
            var model = new ProductSearchVM();
            var products = db.Products.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            { //tim kiem dua tren tu khoa   
                model.SearchTerm = searchTerm;
                products = products.Where(p =>
                p.ProductName.Contains(searchTerm) ||
                p.ProductDescription.Contains(searchTerm) ||
                p.Category.CategoryName.Contains(searchTerm));
            }
            //tìm kiếm sp theo giá tối thiểu
            if (minPrice.HasValue)
            {
                model.MinPrice = minPrice.Value;
                products=products.Where(p=>p.ProductPrice>=minPrice.Value);
            }
            //tim kiem sp dua tren gia toi da
            if (maxPrice.HasValue)
            {
                model.MaxPrice = maxPrice.Value;
                products=products.Where(p=>p.ProductPrice<=maxPrice.Value);
            }
            //Áp dụng sắp xếp dựa trên lựa chọn người dùng
            switch (sortOrder)
            {
                case "name_asc":products=products.OrderBy(p=>p.ProductName); 
                    break;
                case "name_desc":products = products.OrderByDescending(p => p.ProductName);
                    break;
                case "price_asc": products = products.OrderBy(p => p.ProductPrice);
                    break;
                case "price_desc": products=products.OrderByDescending(p => p.ProductPrice);
                    break;
                default: //Mac dinh sap xep theo ten
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }
            model.SortOrder = sortOrder;
            //Đoạn code liên quan tới phân trang
            //lấy số trang hiện tại (Mặc định là trang 1 nếu không có giá trị)
            int PageNumber = page ?? 1;
            int pageSize = 5; //số Sp mỗi trang

            // đóng lệnh này,sử dụng ToPageList để lấy ds đã phân trang
            //model.Products = products.ToList();
            model.Products=products.ToPagedList(PageNumber, pageSize);
            return View(model);
        }
    

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken] //kiểm tra bảo mật
        public ActionResult Create(Product product, HttpPostedFileBase ProductImage)
        {
            if (ModelState.IsValid) //hợp lệ
            {
                // Kiểm tra nếu có ảnh được tải lên
                if (ProductImage != null && ProductImage.ContentLength > 0)
                {
                    // Lấy tên tệp và đường dẫn thư mục
                    var fileName = Path.GetFileName(ProductImage.FileName);
                    var directoryPath = Server.MapPath("~/Content/img-sp/");

                    // Kiểm tra nếu thư mục chưa tồn tại thì tạo mới
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Lấy đường dẫn lưu ảnh
                    var path = Path.Combine(directoryPath, fileName);

                    // Lưu ảnh vào thư mục
                    ProductImage.SaveAs(path);

                    // Lưu tên ảnh (chỉ tên file, không phải đường dẫn đầy đủ) vào cơ sở dữ liệu
                    product.ProductImage = fileName;
                }

                // Thêm sản phẩm vào cơ sở dữ liệu và lưu thay đổi
                db.Products.Add(product);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Nếu Model không hợp lệ, trả về view hiện tại với thông tin sản phẩm
            return View(product);
        }


        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //don dep tai nguyen khi k su dung
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
