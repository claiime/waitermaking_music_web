using Music_Web.Areas.Admin.Models.BussinessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Music_Web.Areas.Admin.Models.BusinessModel
{
    public class AuthorizeBusiness : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["userid"] == null)
            {
                filterContext.Result = new RedirectResult("/Admin/Home/Login");
                return;
            }
            int userId = int.Parse(HttpContext.Current.Session["userid"].ToString());
            //lay ten action
            string actionName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName + "Controller-" + filterContext.ActionDescriptor.ActionName;

            WebDbContext db = new WebDbContext();
            //lay thong tin user
            var admin = db.User.Where(a => a.UserId == userId && a.IsAdmin == true).FirstOrDefault();

            //neu la admin thi khong can kiem tra
            if (admin != null)

                return;
            //lay permission gan cho nguoi dung
            var listpermission = from p in db.Permission
                                 join g in db.GrantPermission on p.PermissionId equals g.PermissionId
                                 where g.UserId == userId
                                 select p.PermissionName;
            //đến trang thông báo
            if (!listpermission.Contains(actionName))
            {
                filterContext.Result = new RedirectResult("/Admin/Home/NotificationAuthorize");
                return;
            }

        }
    }
}