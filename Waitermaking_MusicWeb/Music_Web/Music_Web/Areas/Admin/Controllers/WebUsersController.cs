using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Music_Web.Areas.Admin.Models.BusinessModel;
using Music_Web.Areas.Admin.Models.DataModel;
using PagedList;
using PagedList.Mvc;
namespace Music_Web.Areas.Admin.Controllers
{
    [AuthorizeBusiness]
    public class WebUsersController : Controller
    {
        private WebDbContext db = new WebDbContext();
        // GET: Admin/WebUsers
        public ActionResult Index(int? page = 1)
        {

            int pageNumber = (page ?? 1);
            int pageSize = 8;
            return View(db.User.OrderBy(x => x.UserId).ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/WebUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebUser webUser = db.User.Find(id);
            if (webUser == null)
            {
                return HttpNotFound();
            }
            return View(webUser);
        }

        // GET: Admin/WebUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/WebUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,UserName,Password,FullName,PhoneNumbers,Email,Avatar,isAdmin,Allowed,GioiTinh")] WebUser webUser)
        {
            if (ModelState.IsValid)
            {
                webUser.Date = DateTime.Now;
                db.User.Add(webUser);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(webUser);
        }

        // GET: Admin/WebUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebUser webUser = db.User.Find(id);
            if (webUser == null)
            {
                return HttpNotFound();
            }
            return View(webUser);
        }

        // POST: Admin/WebUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,UserName,Password,FullName,PhoneNumber,Email,Avatar,isAdmin,Allowed,GioiTinh,Date")] WebUser webUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(webUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(webUser);
        }

        // GET: Admin/WebUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebUser webUser = db.User.Find(id);
            if (webUser == null)
            {
                return HttpNotFound();
            }
            return View(webUser);
        }

        // POST: Admin/WebUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WebUser webUser = db.User.Find(id);
            db.User.Remove(webUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public string ChangeImage(string id, string picture)
        {
            if (id == null)
            {
                return "Mã không tồn tại";
            }
            WebUser b = db.User.Find(id);
            if (b == null)
            {
                return "Mã không tồn tại";
            }
            b.Avatar = picture;
            db.SaveChanges();
            return "";
        }
    }
}
