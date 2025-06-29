using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Do_anNhom7.Models;
using Do_anNhom7.Models.ViewModel;

namespace Do_anNhom7.Controllers
{
    public class CartController : Controller
    {
        private QLStoreQuanAoEntities db = new QLStoreQuanAoEntities();

        //Phương thức này trả về một đối tượng CartService, chịu trách nhiệm quản lý logic giỏ hàng thông qua session.

        private CartService GetCartService()
        {
            return new CartService(Session);
        }

        public ActionResult Index()
        {
            var cart = GetCartService().GetCart();
            return View(cart);
        }

        public ActionResult AddToCart(int id, int quantity = 1)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                var cart = GetCartService().GetCart();
                cart.AddItem(product.ProductID, product.ProductImage, product.ProductName, product.ProductPrice, quantity, product.Category.CategoryName);
            }
            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromCart(int id)
        {
            var cart = GetCartService().GetCart();
            cart.RemoveItem(id);
            return RedirectToAction("Index");
        }

        public ActionResult ClearCart()
        {
            GetCartService().ClearCart();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = GetCartService().GetCart();
            cart.UpdateQuantity(id, quantity);
            return RedirectToAction("Index");
        }
    }

}
