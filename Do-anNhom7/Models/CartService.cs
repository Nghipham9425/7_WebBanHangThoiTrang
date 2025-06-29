using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Do_anNhom7.Models.ViewModel
{
    //Dùng để quản lý giỏ hàng trong Session.
    public class CartService
    {
        private readonly HttpSessionStateBase session;

        public CartService(HttpSessionStateBase session)
        {
            this.session = session;
        }
        // Lấy giỏ hàng từ session

        public Cart GetCart()
        {
            // Kiểm tra giỏ hàng có tồn tại trong session hay chưa
            var cart = session["Cart"] as Cart;
            if (cart == null)
            {
                cart = new Cart();
                session["Cart"] = cart;
            }
            return cart;
        }
        // Xóa toàn bộ giỏ hàng

        public void ClearCart()
        {
            session["Cart"] = null;  // Xóa giỏ hàng khỏi session
        }
    }
}