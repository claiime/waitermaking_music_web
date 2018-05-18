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
    public class WebManagerFilesController : Controller
    {
        private WebDbContext db = new WebDbContext();

        // GET: Admin/WebManagerFiles
        public ActionResult Index(int? page = 1)
        {

            int pageNumber = (page ?? 1);
            int pageSize = 8;
            return View(db.ManagerFile.OrderBy(x => x.IdSong).ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/WebManagerFiles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebManagerFile webManagerFile = db.ManagerFile.Find(id);
            if (webManagerFile == null)
            {
                return HttpNotFound();
            }
            return View(webManagerFile);
        }

        // GET: Admin/WebManagerFiles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/WebManagerFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdSong,FileID,NameSong,FileIDAf,NameSongAf")] WebManagerFile webManagerFile)
        {
            if (ModelState.IsValid)
            {
                db.ManagerFile.Add(webManagerFile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(webManagerFile);
        }

        // GET: Admin/WebManagerFiles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebManagerFile webManagerFile = db.ManagerFile.Find(id);
            if (webManagerFile == null)
            {
                return HttpNotFound();
            }
            return View(webManagerFile);
        }

        // POST: Admin/WebManagerFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdSong,FileID,NameSong,FileIDAf,NameSongAf")] WebManagerFile webManagerFile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(webManagerFile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(webManagerFile);
        }

        // GET: Admin/WebManagerFiles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebManagerFile webManagerFile = db.ManagerFile.Find(id);
            if (webManagerFile == null)
            {
                return HttpNotFound();
            }
            return View(webManagerFile);
        }

        // POST: Admin/WebManagerFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WebManagerFile webManagerFile = db.ManagerFile.Find(id);
            db.ManagerFile.Remove(webManagerFile);
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
