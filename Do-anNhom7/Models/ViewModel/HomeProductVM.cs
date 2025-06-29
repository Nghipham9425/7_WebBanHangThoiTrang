using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Do_anNhom7.Models.ViewModel
{
    public class HomeProductVM
    {
        //tiêu chí để search theo tên, mô tả sp
        //hoặc loại sp
        public string SearchTerm {  get; set; }
        //các thuộc tính hỗ trợ phân trang
        public int PageNumber {  get; set; }
        public int PageSize { get; set; }
        //danh sách sp nổi bật
        public List<Product> FeaturedProducts { get; set; }
        //danh sách sp mới đã phân trang
        public PagedList.IPagedList<Product> NewProducts { get; set; }
    }
}