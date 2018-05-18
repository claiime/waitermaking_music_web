using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Music_Web.Areas.Admin.Models.BussinessModel;
using Music_Web.Areas.Admin.Models.DataModel;
using Music_Web.Areas.Admin.Models.BusinessModel;
using PagedList;
using PagedList.Mvc;
namespace Music_Web.Areas.Admin.Controllers
{
    public class WebAlbumUsersController : Controller
    {
        private WebDbContext db = new WebDbContext();

        // GET: Admin/WebAlbumUsers
        public ActionResult Index(int? page = 1)
        {

            int pageNumber = (page ?? 1);
            int pageSize = 8;
            return View(db.User.OrderBy(x => x.UserId).ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/WebAlbumUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebAlbumUser webAlbumUser = db.AlbumUser.Find(id);
            if (webAlbumUser == null)
            {
                return HttpNotFound();
            }
            return View(webAlbumUser);
        }

        // GET: Admin/WebAlbumUsers/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.User, "UserId", "UserName");
            return View();
        }

        // POST: Admin/WebAlbumUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,SoName,ngayDat")] WebAlbumUser webAlbumUser)
        {
            if (ModelState.IsValid)
            {
                db.AlbumUser.Add(webAlbumUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.User, "UserId", "UserName", webAlbumUser.UserId);
            return View(webAlbumUser);
        }

        // GET: Admin/WebAlbumUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebAlbumUser webAlbumUser = db.AlbumUser.Find(id);
            if (webAlbumUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.User, "UserId", "UserName", webAlbumUser.UserId);
            return View(webAlbumUser);
        }

        // POST: Admin/WebAlbumUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,SoName,ngayDat")] WebAlbumUser webAlbumUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(webAlbumUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.User, "UserId", "UserName", webAlbumUser.UserId);
            return View(webAlbumUser);
        }

        // GET: Admin/WebAlbumUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebAlbumUser webAlbumUser = db.AlbumUser.Find(id);
            if (webAlbumUser == null)
            {
                return HttpNotFound();
            }
            return View(webAlbumUser);
        }

        // POST: Admin/WebAlbumUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WebAlbumUser webAlbumUser = db.AlbumUser.Find(id);
            db.AlbumUser.Remove(webAlbumUser);
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
    }
}
