using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList.Mvc;

namespace Do_anNhom7.Models.ViewModel
{
    public class ProductSearchVM
    {
        //tiêu chí để search theo tên,mô tả,phân loại
        public string SearchTerm {  get; set; }
        //tiêu chí search theo giá
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        //thứ tự sắp xếp
        public string SortOrder { get; set; }
        //Các thuộc tính hỗ trợ phân trang
        public int PageNumber {  get; set; } //trang hiện tại
        public int PageSize { get; set; } = 10; // số sp mỗi trang
        //danh sách sản phẩm đã phân trang
        public PagedList.IPagedList<Product> Products { get; set; }
        public List<Product> NewProducts { get; set; }
        ////Ds sp thỏa đk tìm kiếm
        //public List<Product> Products { get; set; }
    }
}   