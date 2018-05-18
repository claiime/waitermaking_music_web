using Music_Web.Areas.Admin.Models.BusinessModel;
using Music_Web.Areas.Admin.Models.BussinessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Music_Web.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home

        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {

            WebDbContext db = new WebDbContext();
            string passwordMD5 = Common.EncryptMD5(username + password);
            var user = db.User.SingleOrDefault(x => x.UserName == username && x.Password == passwordMD5 && x.Allowed == true);
            if (user != null)// đăng nhap thành công
            {
                if (user.IsAdmin == true)
                {

                    Session["admin"] = user.IsAdmin;
                }
                Session["userid"] = user.UserId;
                Session["username"] = user.UserName;
                Session["fullname"] = user.FullName;
                Session["avatar"] = user.Avatar;
                return RedirectToAction("Index");
            }
            ViewBag.error = " Đăng nhập sai hoặc không có quyền";
            return View();

        }
        public ActionResult Logout()
        {
            Session["admin"] = null;
            Session["userid"] = null;
            Session["username"] = null;
            Session["fullname"] = null;
            Session["avatar"] = null;
            return RedirectToAction("Login");
        }
        public ActionResult NotificationAuthorize()
        {
            return View();
        }

        //GETduy tri session
        public EmptyResult Alive()
        {
            return new EmptyResult();
        }


    }
}