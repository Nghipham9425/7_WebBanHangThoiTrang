using Do_anNhom7.Filters;
using Do_anNhom7.Models;
using Do_anNhom7.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Do_anNhom7.Areas.Admin.Controllers
{
    [AdminRoleFilter]
    public class SalesStatisticsController : Controller
    {

        private QLStoreQuanAoEntities db = new QLStoreQuanAoEntities();

        public ActionResult SalesStatistics()
        {
            var model = new SalesStatisticsVM();

            // 1. Tính tổng doanh thu
            model.TotalSales = db.Orders.Sum(o => (decimal?)o.TotalAmount) ?? 0; //Tính tổng cột TotalAmount từ bảng Orders.

            // 2. Đếm tổng số đơn hàng
            model.TotalOrders = db.Orders.Count(); //Đếm số dòng trong bảng Orders.

            // 4. Tính giá trung bình của sản phẩm
            if (model.TotalOrders > 0) // Kiểm tra tránh chia cho 0
            {
                model.AVGPrice = model.TotalSales / model.TotalOrders;
            }
            else
            {
                model.AVGPrice = 0; // Nếu không có sản phẩm đã bán, giá trung bình là 0
            }


            // 3. Tổng số sản phẩm đã bán
            model.TotalProductsSold = db.OrderDetails.Sum(od => (int?)od.Quantity) ?? 0; //Tính tổng cột Quantity từ bảng OrderDetails.

            // 4. Doanh số theo danh mục.Tính tổng doanh thu (TotalSales) và tổng sản phẩm đã bán (TotalProductsSold) từ các sản phẩm thuộc danh mục đó.
            model.SalesByCategories = db.Categories.Select(c => new SalesByCategory 
            {
                CategoryName = c.CategoryName, // Lấy tên danh mục
                TotalSales = c.Products.SelectMany(p => p.OrderDetails).Sum(od => (decimal?)od.TotalPrice) ?? 0, // Tổng doanh thu từ danh mục
                TotalProductsSold = c.Products.SelectMany(p => p.OrderDetails).Sum(od => (int?)od.Quantity) ?? 0 // Tổng số sản phẩm đã bán
            }).ToList();

            // 5. Trả dữ liệu về View
            return View(model);
        }


        // GET: Admin/SalesStatistics
        public ActionResult Index()
        {
            return View();
        }
    }
}
