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
    public class WebGrantPermissionsController : Controller
    {
        private WebDbContext db = new WebDbContext();

        // GET: Admin/WebGrantPermissions
        public ActionResult Index(int id)
        {
            var listcontrol = db.Business.AsEnumerable();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in listcontrol)
            {
                items.Add(new SelectListItem() { Text = item.BusinessName, Value = item.BusinessId });
            }
            ViewBag.items = items;
            //Danh sách quyền đã được cấp
            var listgranted = from g in db.GrantPermission
                              join p in db.Permission on g.PermissionId equals p.PermissionId
                              where g.UserId == id
                              select new SelectListItem() { Value = p.PermissionId.ToString(), Text = p.Description };

            ViewBag.listgranted = listgranted;
            //lưu id session
            Session["usergrant"] = id;
            //Tìm  lấy người dùng lưu ra view, dễ nhìn
            var usergrant = db.User.Find(id);
            ViewBag.usergrant = usergrant.UserName + "(" + usergrant.FullName + ")";
            return View();

            //var grantPermission = db.GrantPermission.Include(s => s.WebPermission).Include(s => s.WebUser);
            // return View(grantPermission.ToList());
        }

        //lấy danh sách quyền đang được cấp cho ngưởi dùng
        public JsonResult getPermissions(string id, int usertemp)
        {
            //lấy tất cả permission của user và controller
            var listgranted = (from g in db.GrantPermission
                               join p in db.Permission on g.PermissionId equals p.PermissionId
                               where g.UserId == usertemp && p.BusinessId == id
                               select new PermissionAction { PermissionId = p.PermissionId, PermissionName = p.PermissionName, Description = p.Description, IsGranted = true }).ToList();
            //Lấy tất cả permission của business hien tại
            var listpermission = from p in db.Permission
                                 where p.BusinessId == id
                                 select new PermissionAction { PermissionId = p.PermissionId, PermissionName = p.PermissionName, Description = p.Description, IsGranted = false };
            //lấy cá id của permision đã gá cho người dùng
            var listpermissionId = listgranted.Select(p => p.PermissionId);
            //so sanh kiem tra permisionId nao của business chưa có trong listgrant thì đưa vào(isgrant =false)

            foreach (var item in listpermission)
            {
                if (!listpermissionId.Contains(item.PermissionId))
                    listgranted.Add(item);
            }
            return Json(listgranted.OrderBy(x => x.Description), JsonRequestBehavior.AllowGet);
        }

        //Cập nhật quyền cho người dùng
        public string updatePermission(int id, int usertemp)
        {
            string msg = "";
            var grant = db.GrantPermission.Find(id, usertemp);
            if (grant == null)
            {

                WebGrantPermission g = new WebGrantPermission() { PermissionId = id, UserId = usertemp, Description = "" };
                db.GrantPermission.Add(g);
                msg = "<div class='alert alert-danger'>Đã cấp thành công</div>";
            }
            else
            {
                db.GrantPermission.Remove(grant);
                msg = "<div class='alert alert-danger'>Đã hủy thành công</div>";
            }
            db.SaveChanges();
            return msg;
        }






        // GET: Admin/WebGrantPermissions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebGrantPermission webGrantPermission = db.GrantPermission.Find(id);
            if (webGrantPermission == null)
            {
                return HttpNotFound();
            }
            return View(webGrantPermission);
        }

        // GET: Admin/WebGrantPermissions/Create
        public ActionResult Create()
        {
            ViewBag.PermissionId = new SelectList(db.Permission, "PermissionId", "PermissionName");
            ViewBag.UserId = new SelectList(db.User, "UserId", "UserName");
            return View();
        }

        // POST: Admin/WebGrantPermissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PermissionId,UserId,Description")] WebGrantPermission webGrantPermission)
        {
            if (ModelState.IsValid)
            {
                db.GrantPermission.Add(webGrantPermission);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PermissionId = new SelectList(db.Permission, "PermissionId", "PermissionName", webGrantPermission.PermissionId);
            ViewBag.UserId = new SelectList(db.User, "UserId", "UserName", webGrantPermission.UserId);
            return View(webGrantPermission);
        }

        // GET: Admin/WebGrantPermissions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebGrantPermission webGrantPermission = db.GrantPermission.Find(id);
            if (webGrantPermission == null)
            {
                return HttpNotFound();
            }
            ViewBag.PermissionId = new SelectList(db.Permission, "PermissionId", "PermissionName", webGrantPermission.PermissionId);
            ViewBag.UserId = new SelectList(db.User, "UserId", "UserName", webGrantPermission.UserId);
            return View(webGrantPermission);
        }

        // POST: Admin/WebGrantPermissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PermissionId,UserId,Description")] WebGrantPermission webGrantPermission)
        {
            if (ModelState.IsValid)
            {
                db.Entry(webGrantPermission).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PermissionId = new SelectList(db.Permission, "PermissionId", "PermissionName", webGrantPermission.PermissionId);
            ViewBag.UserId = new SelectList(db.User, "UserId", "UserName", webGrantPermission.UserId);
            return View(webGrantPermission);
        }

        // GET: Admin/WebGrantPermissions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebGrantPermission webGrantPermission = db.GrantPermission.Find(id);
            if (webGrantPermission == null)
            {
                return HttpNotFound();
            }
            return View(webGrantPermission);
        }

        // POST: Admin/WebGrantPermissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WebGrantPermission webGrantPermission = db.GrantPermission.Find(id);
            db.GrantPermission.Remove(webGrantPermission);
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