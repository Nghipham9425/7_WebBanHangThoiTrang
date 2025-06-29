using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using Do_anNhom7.Filters;
using Do_anNhom7.Models;

namespace Do_anNhom7.Areas.Admin.Controllers
{
    [AdminRoleFilter]
    public class CategoriesController : Controller
    {
        QLStoreQuanAoEntities csdl = new QLStoreQuanAoEntities();

        // GET: Admin/Categories
        public ActionResult Index()
        {
            // Lấy danh sách tất cả các danh mục và truyền vào View
            return View(csdl.Categories.ToList());
        }

        // GET: Admin/Categories/Create
        [HttpGet]
        public ActionResult Create()
        {
            // Trả về View để tạo mới danh mục
            return View();
        }

        // POST: Admin/Categories/Create
        [HttpPost]
        public ActionResult Create(Category cate)
        {
            // Thêm danh mục mới vào cơ sở dữ liệu
            csdl.Categories.Add(cate);
            csdl.SaveChanges();
            // Chuyển hướng về trang danh sách danh mục
            return RedirectToAction("Index");
        }

        // GET: Admin/Categories/Delete
        [HttpGet]
        public ActionResult Delete(Category cate)
        {
            // Lấy danh mục cần xóa từ cơ sở dữ liệu và truyền vào View
            cate = csdl.Categories.Where(s => s.CategoryID == cate.CategoryID).FirstOrDefault();
            return View(cate);
        }

        // POST: Admin/Categories/Delete
        [HttpPost]
        public ActionResult Delete(int CategoryID)
        {
            // Lấy danh mục cần xóa từ cơ sở dữ liệu
            var cate = csdl.Categories.Where(s => s.CategoryID == CategoryID).FirstOrDefault();
            if (cate != null)
            {
                // Xóa danh mục và lưu thay đổi vào cơ sở dữ liệu
                csdl.Categories.Remove(cate);
                csdl.SaveChanges();
                // Chuyển hướng về trang danh sách danh mục
                return RedirectToAction("Index");
            }
            else
            {
                // Nếu không tìm thấy danh mục, trả về View hiện tại
                return View();
            }
        }

        // GET: Admin/Categories/Edit
        [HttpGet]
        public ActionResult Edit(Category cate)
        {
            // Lấy danh mục cần chỉnh sửa từ cơ sở dữ liệu và truyền vào View
            cate = csdl.Categories.Where(s => s.CategoryID == cate.CategoryID).FirstOrDefault();
            return View(cate);
        }

        // POST: Admin/Categories/Edit
        [HttpPost]
        public ActionResult Edit(Category cate, string CategoryID)
        {
            // Lấy danh mục cần chỉnh sửa từ cơ sở dữ liệu
            var frmcate = csdl.Categories.Where(s => s.CategoryID == cate.CategoryID).FirstOrDefault();
            if (frmcate != null)
            {
                // Cập nhật tên danh mục và lưu thay đổi vào cơ sở dữ liệu
                frmcate.CategoryName = cate.CategoryName;
                csdl.SaveChanges();
                // Chuyển hướng về trang danh sách danh mục
                return RedirectToAction("Index");
            }
            else
            {
                // Nếu không tìm thấy danh mục, trả về View hiện tại
                return View();
            }
        }

        // GET: Admin/Categories/Details
        public ActionResult Details(Category cate)
        {
            // Lấy danh mục cần xem chi tiết từ cơ sở dữ liệu và truyền vào View
            cate = csdl.Categories.Where(s => s.CategoryID == cate.CategoryID).FirstOrDefault();
            return View(cate);
        }
    }
}
