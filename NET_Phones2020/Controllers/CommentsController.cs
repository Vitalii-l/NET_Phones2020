using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NET_Phones2020.Models;
using System.Globalization;

namespace NET_Phones2020.Controllers
{
    public class CommentsController : Controller
    {
        private PhonesEntities db = new PhonesEntities();

        // GET: Comments
        [ChildActionOnly]
        // Если представление дочернее, то оно не может быть вызвано в адресной строке
        // Для того, чтобы вывести комментарии для конкретного продукта, надо знать
        // его Id - передадим Id в метод Index из ссылки на странице Phones/Details
        public PartialViewResult Index(int PhoneID)
        {
            var comments = db.Comments.Where(c => c.PhonesId == PhoneID).OrderByDescending(m => m.DatePublish);
            ViewBag.PhoneId = PhoneID;
            return PartialView(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        [Authorize]
        public PartialViewResult Create(int PhoneID)
        {
            ViewBag.PhonesId = PhoneID;
            return PartialView();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public PartialViewResult Index(Comment comment, int PhoneID)
        {
            // The comment comes from the currently authentificated user
            comment.UserName = User.Identity.Name;
            comment.DatePublish = DateTime.Now;
            comment.PhonesId = PhoneID;

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
            }
            var comments = db.Comments.Where(c => c.PhonesId == PhoneID).OrderByDescending(m => m.DatePublish);
            ViewBag.PhoneId = PhoneID;
            return PartialView("Index", comments.ToList());
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.PhonesId = new SelectList(db.Phones, "Id", "Name", comment.PhonesId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PhonesId,UserName,Comments,DatePublish")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PhonesId = new SelectList(db.Phones, "Id", "Name", comment.PhonesId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        [Authorize(Roles ="admin")]
        public ActionResult Delete(int? id) //, int PhoneID
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) // comment.PhonesId
        {
            Comment comment = db.Comments.Find(id);
            ViewBag.PhoneId = comment.PhonesId;
            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Details", "Phones", new { id = ViewBag.PhoneId});
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
