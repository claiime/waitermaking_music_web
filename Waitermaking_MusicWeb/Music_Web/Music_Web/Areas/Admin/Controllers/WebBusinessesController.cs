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

namespace Music_Web.Areas.Admin.Controllers
{

    [AuthorizeBusiness]
    public class WebBusinessesController : Controller
    {
        private WebDbContext db = new WebDbContext();

        //GET:/Business/UpdateBusiness--CCa65p nhật danh sách nghiệp vụt
        public ActionResult UpdateBusiness()
        {
            ReflectionController rc = new ReflectionController();
            List<Type> listControllerType = rc.GetControllers("Music_Web.Areas.Admin");
            List<string> listControllerOld = db.Business.Select(c => c.BusinessId).ToList();
            List<string> listPermisionOld = db.Permission.Select(p => p.PermissionName).ToList();
            foreach (var c in listControllerType)
            {
                if (!listControllerOld.Contains(c.Name))
                {
                    WebBusiness b = new WebBusiness() { BusinessId = c.Name, BusinessName = "Chưa có mô tả" };
                    db.Business.Add(b);
                    db.SaveChanges();
                }
                List<string> listPermission = rc.GetActions(c);
                
                foreach (var p in listPermission)
                {
                    if (!listPermisionOld.Contains(c.Name + "-" + p))
                    {
                        Models.DataModel.WebPermission permission = new Models.DataModel.WebPermission() { PermissionName = c.Name + "-" + p, Description = "Chưa có mô tả", BusinessId = c.Name };
                        db.Permission.Add(permission);
                        db.SaveChanges();
                    }
                }
            }
            db.SaveChanges();
            TempData["err"] = "<div class='alert alert-info' role='alert'><span class='glyphicon glyphicon-exclamation-sign' aria-hidden='true'></span><span class='sr-only'></span>Cập nhật thành công</div>";
            return RedirectToAction("Index");

        }

        // GET: Admin/WebBusinesses
        public ActionResult Index()
        {

            return View(db.Business.ToList());
        }

        // GET: Admin/WebBusinesses/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebBusiness webBusiness = db.Business.Find(id);
            if (webBusiness == null)
            {
                return HttpNotFound();
            }
            return View(webBusiness);
        }

        // GET: Admin/WebBusinesses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/WebBusinesses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BusinessId,BusinessName")] WebBusiness webBusiness)
        {
            if (ModelState.IsValid)
            {
                db.Business.Add(webBusiness);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(webBusiness);
        }

        // GET: Admin/WebBusinesses/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebBusiness webBusiness = db.Business.Find(id);
            if (webBusiness == null)
            {
                return HttpNotFound();
            }
            return View(webBusiness);
        }

        // POST: Admin/WebBusinesses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BusinessId,BusinessName")] WebBusiness webBusiness)
        {
            if (ModelState.IsValid)
            {
                db.Entry(webBusiness).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(webBusiness);
        }

        // GET: Admin/WebBusinesses/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebBusiness webBusiness = db.Business.Find(id);
            if (webBusiness == null)
            {
                return HttpNotFound();
            }
            return View(webBusiness);
        }

        // POST: Admin/WebBusinesses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            WebBusiness webBusiness = db.Business.Find(id);
            db.Business.Remove(webBusiness);
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
