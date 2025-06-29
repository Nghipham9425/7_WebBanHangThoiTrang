using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Do_anNhom7.Filters
{
    public class AdminRoleFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Kiểm tra nếu người dùng chưa đăng nhập hoặc không phải là Admin
            if (filterContext.HttpContext.Session["UserRole"] == null || filterContext.HttpContext.Session["UserRole"].ToString() != "Admin")
            {
                // Nếu không phải là Admin, chuyển hướng đến trang đăng nhập
                filterContext.Result = new RedirectResult("~/Account/Login");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}