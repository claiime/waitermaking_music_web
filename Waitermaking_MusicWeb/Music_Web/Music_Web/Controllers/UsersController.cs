using Music_Web.Areas.Admin.Models.BusinessModel;
using Music_Web.Areas.Admin.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Music_Web.Controllers
{
    public class UsersController : Controller
    {
        WebDbContext db = new WebDbContext();
        // GET: Users
        public ActionResult Index(int maND)
        {
            WebUser us = db.User.SingleOrDefault(x => x.UserId == maND);
            if (us != null)
            {
                return View(us);
            }
            return View("Loi404", "Home");
        }
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(WebUser mb)
        {
           
            if (ModelState.IsValid)
            {

                WebUser temp = db.User.SingleOrDefault(n => n.UserName.ToLower() == mb.UserName.ToLower());

                if (mb.Allowed != true)
                {
                    ViewBag.LoiDangNhap = "Đồng ý điều khoản";
                }
                if (temp != null)
                {
                    ViewBag.LoiDangKy = "Tên người dùng đã tồn tại";
                    return View(mb);
                }
                else
                {
                    mb.Date = DateTime.Now;
                    mb.Password = Common.EncryptMD5(mb.UserName + mb.Password);
                    //if (mb.Avatar == "")
                    //{
                    mb.Avatar = "/Areas/Admin/Image/avatar.png";
                    //}
                    ViewBag.LoiDangKy = "here";
                    db.User.Add(mb);
                    db.SaveChanges();
                    Session["admin"] = null;
                    Session["username"] = mb.UserName;
                    Session["userid"] = mb.UserId;
                    Session["fullname"] = mb.FullName;
                    Session["avatar"] = mb.Avatar;
                    return RedirectToAction("Index", new { @maND = mb.UserId });
                }

            }

            return View(mb);
        }
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection f)
        {

            string sTaiKhoan = f["txtTaiKhoan"].ToString();
            string sMatKhau = f["txtMatKhau"].ToString();//hai cach như nhau
            string passwordMD5 = Common.EncryptMD5(sTaiKhoan + sMatKhau);
            WebUser mb = db.User.SingleOrDefault(n => n.UserName == sTaiKhoan && n.Password == passwordMD5 && n.Allowed == true);
            if (mb != null)
            {
                ViewBag.ThongBao = "Bạn đã đăng nhập thàng công!";
                if (mb.IsAdmin == true)
                {
                    Session["admin"] = mb.IsAdmin;
                }
                Session["TaiKhoan"] = mb;
                Session["username"] = mb.UserName;
                Session["fullname"] = mb.FullName;
                Session["avatar"] = mb.Avatar;

                Session["userid"] = mb.UserId;
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ThongBao = "Tên hoặc tài khoản không đúng";
            return View();
        }
        public ActionResult DangXuat()
        {
            Session["admin"] = null;
            Session["userid"] = null;
            Session["username"] = null;
            Session["fullname"] = null;
            Session["avatar"] = null;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ChiTiet(int maND)
        {
            WebUser us = db.User.SingleOrDefault(x => x.UserId == maND);
            if (us != null)
            {
                return View(us);
            }
            return View("Loi404", "Default");
        }
       

    }
}