using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Do_anNhom7.Models.ViewModel
{
    public class SalesStatisticsVM
    {
        public decimal TotalSales { get; set; } // Tổng doanh thu
        public int TotalOrders { get; set; } // Tổng số đơn hàng
        public int TotalProductsSold { get; set; } // Tổng số sản phẩm đã bán
        public decimal AVGPrice { get; set; } // Giá trung bình của sản phẩm
        public List<SalesByCategory> SalesByCategories { get; set; } // Danh sách doanh thu theo danh mục
    }

    public class SalesByCategory
    {
        public string CategoryName { get; set; } // Tên danh mục sản phẩm
        public decimal TotalSales { get; set; } // Tổng doanh thu từ danh mục
        public int TotalProductsSold { get; set; } // Tổng số sản phẩm đã bán trong danh mục
    }
}