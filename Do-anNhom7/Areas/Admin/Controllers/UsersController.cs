using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Do_anNhom7.Filters;
using Do_anNhom7.Models;

namespace Do_anNhom7.Areas.Admin.Controllers
{
    [AdminRoleFilter]
    public class UsersController : Controller
    {
        private QLStoreQuanAoEntities db = new QLStoreQuanAoEntities();

        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        public ActionResult Details(string id)
        {
            User user = db.Users.Find(id);
            return View(user);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                user.UserRole = "Admin";
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult Edit(string id)
        {
            User user = db.Users.Find(id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult Delete(string id)
        {
            User user = db.Users.Find(id);
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            // Find the user to be deleted
            User user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            try
            {
                // Check if the user is a customer
                var customer = db.Customers.FirstOrDefault(c => c.Username == id);
                if (customer != null)
                {
                    // Delete all orders for the customer
                    var orders = db.Orders.Where(o => o.CustomerID == customer.CustomerID).ToList();
                    foreach (var order in orders)
                    {
                        // Delete order details
                        var orderDetails = db.OrderDetails.Where(od => od.OrderID == order.OrderID).ToList();
                        db.OrderDetails.RemoveRange(orderDetails);

                        // Delete the order
                        db.Orders.Remove(order);
                    }

                    // Delete the customer
                    db.Customers.Remove(customer);
                }

                // Delete the user account
                db.Users.Remove(user);
                db.SaveChanges();

                return RedirectToAction("Index"); // Redirect back to the user list
            }
            catch (Exception ex)
            {
                // Handle any errors
                ModelState.AddModelError("", "Failed to delete user: " + ex.Message);
                return View(user);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}