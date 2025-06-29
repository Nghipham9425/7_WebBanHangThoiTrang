using Do_anNhom7.Models;
using Do_anNhom7.Models.ViewModel;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using PayPal.Api;
using System.Collections.Generic;

namespace Do_anNhom7.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private QLStoreQuanAoEntities db = new QLStoreQuanAoEntities();
        private const decimal ExchangeRate = 25000m; // Tỷ giá 1 USD = 25,000 VND

        // GET: Checkout
        [Authorize]
        public ActionResult Checkout()
        {
            var cart = Session["Cart"] as Cart;

            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Homepage", "Home");
            }

            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new CheckoutVM
            {
                CartItems = cart.Items.ToList(),
                TotalAmount = cart.TotalValue(),
                OrderDate = DateTime.Now,
                ShippingAddress = customer.CustomerAddress,
                CustomerID = customer.CustomerID,
                Username = customer.Username
            };

            return View(model);
        }

        // POST: Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var cart = Session["Cart"] as Cart;
                if (cart == null || !cart.Items.Any())
                {
                    return RedirectToAction("Homepage", "Home");
                }

                var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
                if (customer == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Kiểm tra phương thức giao hàng
                if (string.IsNullOrEmpty(model.DeliveryMethod))
                {
                    ModelState.AddModelError("DeliveryMethod", "Vui lòng chọn phương thức giao hàng.");
                    return View(model);
                }

                // Xác định trạng thái thanh toán
                string paymentStatus = model.PaymentMethod == "Paypal" ? "Đã thanh toán" : "Chưa thanh toán";

                var order = new Do_anNhom7.Models.Order
                {
                    CustomerID = customer.CustomerID,
                    OrderDate = model.OrderDate < new DateTime(1753, 1, 1) ? DateTime.Now : model.OrderDate,
                    TotalAmount = model.TotalAmount,
                    PaymentStatus = paymentStatus,
                    PaymentMethod = model.PaymentMethod,
                    DeliveryMethod = model.DeliveryMethod,
                    ShippingAddress = model.ShippingAddress, // Sử dụng địa chỉ giao hàng mới
                    OrderDetails = cart.Items.Select(item => new OrderDetail
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    }).ToList()
                };

                db.Orders.Add(order);
                db.SaveChanges();
                Session["Cart"] = null;

                return RedirectToAction("OrderSuccess", new { id = order.OrderID });
            }

            return View(model);
        }

        // Payment with PayPal
        [HttpGet]
        public ActionResult PaymentWithPaypal(string guid, string payerId)
        {
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                if (executedPayment.state.ToLower() != "approved")
                {
                    return View("FailureView");
                }

                // Lấy thông tin đơn hàng từ session hoặc cơ sở dữ liệu
                var cart = Session["Cart"] as Cart;
                if (cart == null || !cart.Items.Any())
                {
                    return RedirectToAction("Homepage", "Home");
                }

                var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
                if (customer == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var deliveryMethod = Session["DeliveryMethod"] as string;
                if (string.IsNullOrEmpty(deliveryMethod))
                {
                    deliveryMethod = "ViettelPost"; // Giá trị mặc định nếu không có trong session
                }

                var shippingAddress = Session["ShippingAddress"] as string;
                if (string.IsNullOrEmpty(shippingAddress))
                {
                    shippingAddress = customer.CustomerAddress; // Giá trị mặc định nếu không có trong session
                }

                var order = new Do_anNhom7.Models.Order
                {
                    CustomerID = customer.CustomerID,
                    OrderDate = DateTime.Now,
                    TotalAmount = cart.TotalValue(),
                    PaymentStatus = "Đã thanh toán",
                    PaymentMethod = "Paypal",
                    DeliveryMethod = deliveryMethod,
                    ShippingAddress = shippingAddress, // Sử dụng địa chỉ giao hàng mới
                    OrderDetails = cart.Items.Select(item => new OrderDetail
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    }).ToList()
                };

                db.Orders.Add(order);
                db.SaveChanges();
                Session["Cart"] = null;

                // Chuyển hướng đến trang thành công với mã đơn hàng
                return RedirectToAction("OrderSuccess", new { id = order.OrderID });
            }
            catch (Exception ex)
            {
                // Log lỗi để dễ dàng xác định vấn đề
                System.Diagnostics.Debug.WriteLine("Lỗi khi thanh toán với PayPal: " + ex.Message);
                return View("FailureView");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PaymentWithPaypal(CheckoutVM model)
        {
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Order/PaymentWithPaypal?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);

                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    Session.Add(guid, createdPayment.id);
                    Session["DeliveryMethod"] = model.DeliveryMethod; // Lưu phương thức giao hàng vào session
                    Session["ShippingAddress"] = model.ShippingAddress; // Lưu địa chỉ giao hàng vào session
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }

                    // Lấy thông tin đơn hàng từ session hoặc cơ sở dữ liệu
                    var cart = Session["Cart"] as Cart;
                    if (cart == null || !cart.Items.Any())
                    {
                        return RedirectToAction("Homepage", "Home");
                    }

                    var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
                    if (user == null)
                    {
                        return RedirectToAction("Login", "Account");
                    }

                    var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
                    if (customer == null)
                    {
                        return RedirectToAction("Login", "Account");
                    }

                    var deliveryMethod = Session["DeliveryMethod"] as string;
                    if (string.IsNullOrEmpty(deliveryMethod))
                    {
                        deliveryMethod = "ViettelPost"; // Giá trị mặc định nếu không có trong session
                    }

                    var shippingAddress = Session["ShippingAddress"] as string;
                    if (string.IsNullOrEmpty(shippingAddress))
                    {
                        shippingAddress = customer.CustomerAddress; // Giá trị mặc định nếu không có trong session
                    }

                    var order = new Do_anNhom7.Models.Order
                    {
                        CustomerID = customer.CustomerID,
                        OrderDate = DateTime.Now,
                        TotalAmount = cart.TotalValue(),
                        PaymentStatus = "Đã thanh toán",
                        PaymentMethod = "Paypal",
                        DeliveryMethod = deliveryMethod,
                        ShippingAddress = shippingAddress, // Sử dụng địa chỉ giao hàng mới
                        OrderDetails = cart.Items.Select(item => new OrderDetail
                        {
                            ProductID = item.ProductID,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            TotalPrice = item.TotalPrice
                        }).ToList()
                    };

                    db.Orders.Add(order);
                    db.SaveChanges();
                    Session["Cart"] = null;

                    // Chuyển hướng đến trang thành công với mã đơn hàng
                    return RedirectToAction("OrderSuccess", new { id = order.OrderID });
                }
            }
            catch (Exception ex)
            {
                // Log lỗi để dễ dàng xác định vấn đề
                System.Diagnostics.Debug.WriteLine("Lỗi khi thanh toán với PayPal: " + ex.Message);
                return View("FailureView");
            }
        }

        // Execute payment after PayPal payment confirmation
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            var payment = new Payment()
            {
                id = paymentId
            };
            return payment.Execute(apiContext, paymentExecution);
        }

        // Create PayPal payment
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };

            var cart = Session["Cart"] as Cart;
            if (cart != null)
            {
                foreach (var item in cart.Items)
                {
                    itemList.items.Add(new Item()
                    {
                        name = item.ProductName,
                        currency = "USD",
                        price = (item.UnitPrice / ExchangeRate).ToString("F2"),
                        quantity = item.Quantity.ToString(),
                        sku = item.ProductID.ToString()
                    });
                }
            }

            var payer = new Payer()
            {
                payment_method = "paypal"
            };

            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = (cart.TotalValue() / ExchangeRate).ToString("F2")
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = (cart.TotalValue() / ExchangeRate).ToString("F2"),
                details = details
            };

            var transactionList = new List<Transaction>();
            var paypalOrderId = DateTime.Now.Ticks;
            transactionList.Add(new Transaction()
            {
                description = $"Invoice #{paypalOrderId}",
                invoice_number = paypalOrderId.ToString(),
                amount = amount,
                item_list = itemList
            });

            var payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            return payment.Create(apiContext);
        }

        // Order success page
        public ActionResult OrderSuccess(int id)
        {
            var order = db.Orders.FirstOrDefault(o => o.OrderID == id);
            if (order == null)
            {
                return HttpNotFound("Không tìm thấy đơn hàng.");
            }

            ViewBag.OrderId = id;
            ViewBag.PaymentMethod = order.PaymentMethod;

            return View();
        }

        // Failure page (Payment failed)
        public ActionResult FailureView()
        {
            return View();
        }

        // Order details page
        public ActionResult OrderDetails(int id)
        {
            var order = db.Orders
                .Include("OrderDetails")
                .FirstOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return HttpNotFound("Không tìm thấy đơn hàng.");
            }

            return View(order);
        }
    }
}
