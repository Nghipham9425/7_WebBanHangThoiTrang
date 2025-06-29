using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Do_anNhom7.Models.ViewModel
{
    public class CheckoutVM
    {
        public List<CartItem> CartItems { get; set; }

        public int CustomerID { get; set; }
        [Display(Name ="Ngày đặt hàng")]
        public System.DateTime OrderDate { get; set; }
        [Display(Name ="Tổng giá trị")]
        public decimal TotalAmount {  get; set; }

        [Display(Name = "Trạng thái thanh toán")]
        public string PaymentStatus { get; set; }
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; }

        [Display(Name = "Phương thức giao hàng")]
        public string DeliveryMethod { get; set; }

        [Display(Name = "Địa chỉ giao hàng")]
        public string ShippingAddress { get; set; }

        public string Username {  get; set; }

        //các thuộc tính khác của đơn hàng
        public List<OrderDetail> OrderDetails { get; set; }

    }
}