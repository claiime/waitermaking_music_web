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
    public class WebPermissionsController : Controller
    {
        private WebDbContext db = new WebDbContext();

        // GET: Admin/WebPermissions
        public ActionResult Index(string id)
        {
            var permission = db.Permission.Where(x => x.BusinessId == id);
            return View(permission.ToList());
        }

        // GET: Admin/WebPermissions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Music_Web.Areas.Admin.Models.DataModel.WebPermission webPermission = db.Permission.Find(id);
            if (webPermission == null)
            {
                return HttpNotFound();
            }
            return View(webPermission);
        }

        // GET: Admin/WebPermissions/Create
        public ActionResult Create()
        {
            ViewBag.BusinessId = new SelectList(db.Business, "BusinessId", "BusinessName");
            return View();
        }

        // POST: Admin/WebPermissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PermissionId,PermissionName,Description,BusinessId")] Music_Web.Areas.Admin.Models.DataModel.WebPermission webPermission)
        {
            if (ModelState.IsValid)
            {
                db.Permission.Add(webPermission);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BusinessId = new SelectList(db.Business, "BusinessId", "BusinessName", webPermission.BusinessId);
            return View(webPermission);
        }

        // GET: Admin/WebPermissions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Music_Web.Areas.Admin.Models.DataModel.WebPermission webPermission = db.Permission.Find(id);
            if (webPermission == null)
            {
                return HttpNotFound();
            }
            ViewBag.BusinessId = new SelectList(db.Business, "BusinessId", "BusinessName", webPermission.BusinessId);
            return View(webPermission);
        }

        // POST: Admin/WebPermissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PermissionId,PermissionName,Description,BusinessId")] Music_Web.Areas.Admin.Models.DataModel.WebPermission webPermission)
        {
            if (ModelState.IsValid)
            {
                db.Entry(webPermission).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = webPermission.BusinessId });
            }
            ViewBag.BusinessId = new SelectList(db.Business, "BusinessId", "BusinessName", webPermission.BusinessId);
            return View(webPermission);
        }

        // GET: Admin/WebPermissions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Music_Web.Areas.Admin.Models.DataModel.WebPermission webPermission = db.Permission.Find(id);
            if (webPermission == null)
            {
                return HttpNotFound();
            }
            return View(webPermission);
        }

        // POST: Admin/WebPermissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Music_Web.Areas.Admin.Models.DataModel.WebPermission webPermission = db.Permission.Find(id);
            db.Permission.Remove(webPermission);
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
