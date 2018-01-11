using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyEvernote.Entities;
using MyEvernote.BusinessLayer;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.Web.Filters;

namespace MyEvernote.Web.Controllers
{
    [Exc]
    [Auth]
    [AuthAdmin]
    public class EvernoteUserController : Controller
    {
        private EvernoteUserManager evernoteUserManager = new EvernoteUserManager();

        // GET: EvernoteUser
        public ActionResult Index()
        {
            return View(evernoteUserManager.List());
        }

        // GET: EvernoteUser/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvernoteUser evernoteUser = evernoteUserManager.Find(x=>x.Id==id);
            if (evernoteUser == null)
            {
                return HttpNotFound();
            }
            return View(evernoteUser);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EvernoteUser evernoteUser)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");
            if (ModelState.IsValid)
            {
                BusinessLayerResult<EvernoteUser> res = evernoteUserManager.Insert(evernoteUser);
                if (res.Errors.Count>0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(evernoteUser);
                }
               
                return RedirectToAction("Index");
            }

            return View(evernoteUser);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvernoteUser evernoteUser = evernoteUserManager.Find(x => x.Id == id);
            if (evernoteUser == null)
            {
                return HttpNotFound();
            }
            return View(evernoteUser);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EvernoteUser evernoteUser)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");
            if (ModelState.IsValid)
            {
                BusinessLayerResult<EvernoteUser> res = evernoteUserManager.Update(evernoteUser);
                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(evernoteUser);
                }
                return RedirectToAction("Index");
            }
            return View(evernoteUser);
        }

        // GET: EvernoteUser/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvernoteUser evernoteUser = evernoteUserManager.Find(x => x.Id == id);
            if (evernoteUser == null)
            {
                return HttpNotFound();
            }
            return View(evernoteUser);
        }

        // POST: EvernoteUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EvernoteUser evernoteUser = evernoteUserManager.Find(x => x.Id == id);
            evernoteUserManager.Delete(evernoteUser);

            return RedirectToAction("Index");
        }
    }
}
