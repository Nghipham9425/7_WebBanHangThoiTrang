using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Do_anNhom7.Models.ViewModel
{
    public class ProductDetailsVM
    {
        public Product product { get; set; }
        public int quantity { get; set; } = 1;
        //tinh gia tri tam thoi
        public decimal estimatedValue
        { get; set; }
        public List<string> AvailableSizes { get; set; }  // Danh sách các size có sẵn
        public string SelectedSize { get; set; }  // Size mà người dùng đã chọn
        // cac thuoc tinh ho tro phan trang
        public int PageNumber { get; set; } //trang hien tai
        public int PageSize { get; set; } = 3; // so sp moi trang

        //ds sp cung danh muc
        public List<Product> RelatedProducts { get; set; }
        //ds sp ban chay nhat cung danh muc
        public PagedList.IPagedList<Product> TopProducts { get; set; } // Chỉ cần List<Product>

    }
}