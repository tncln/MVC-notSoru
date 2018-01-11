using MyEvernote.BusinessLayer;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ValueObjects;
using MyEvernote.Web.Filters;
using MyEvernote.Web.Models;
using MyEvernote.Web.ViewModels;
using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MyEvernote.Web.Controllers
{
    [Exc]
    public class HomeController : Controller
    {
        private NoteManager noteManager = new NoteManager();
        private CategoryManager categoryManager = new CategoryManager();
        private EvernoteUserManager evernoteUserManager = new EvernoteUserManager();
        // GET: Home
        public ActionResult Index()
        {
            //CategoryController üzerinden gelen view talebi
            //if (TempData["mm"]!=null)
            //{
            //    return View(TempData["mm"] as List<Note>);
            //}
           

            return View(noteManager.ListQueryable().Where(y=>y.IsDraft==false).OrderByDescending(x => x.ModifiedOn).ToList());
            //return View(nm.GetAllNoteQueryable().OrderByDescending(x => x.ModifiedOn).ToList());
        }

        public ActionResult ByCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            Category cat = categoryManager.Find(x=>x.Id==id.Value);

            if (cat == null)
            {
                return HttpNotFound();
                //return RedirectToAction("Index", "Home");
            }

            return View("Index", cat.Notes.OrderByDescending(x=>x.ModifiedOn ).ToList());
        }
        public ActionResult MostLiked()
        {
            return View("Index", noteManager.ListQueryable().OrderByDescending(x=>x.LikeCount).ToList());
        }
        public ActionResult About()
        {
            return View();
        }
        [Auth]
        public ActionResult ShowProfile()
        {
          
            BusinessLayerResult<EvernoteUser> res = evernoteUserManager.GetUserById(CurrentSession.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errornotifyObj = new ErrorViewModel()
                {
                    Title = "Hata oluştu",
                    Items = res.Errors
                };

                return View("Error", errornotifyObj);
            }

            return View(res.Result);
        }
        [Auth]
        public ActionResult EditProfile()
        {
            BusinessLayerResult<EvernoteUser> res = evernoteUserManager.GetUserById(CurrentSession.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errornotifyObj = new ErrorViewModel()
                {
                    Title = "Hata oluştu",
                    Items = res.Errors
                };

                return View("Error", errornotifyObj);
            }

            return View(res.Result);
        }
        [Auth]
        [HttpPost]
        public ActionResult EditProfile(EvernoteUser model,HttpPostedFileBase ProfileImage)
        {
            ModelState.Remove("ModifiedUsername");
            if (ModelState.IsValid)
            {
                
                if (ProfileImage != null &&
               (ProfileImage.ContentType == "image/jpeg" ||
               ProfileImage.ContentType == "image/jpg" ||
               ProfileImage.ContentType == "image/png"))
                {
                    string filename = $"user_{model.Id}.{ProfileImage.ContentType.Split('/')[1]}";

                    ProfileImage.SaveAs(Server.MapPath($"~/images/{filename}"));
                    model.ProfileImageFilename = filename;
                }
              
                BusinessLayerResult<EvernoteUser> res = evernoteUserManager.UpdateProfile(model);

                if (res.Errors.Count > 0)
                {
                    ErrorViewModel errorNotifyObj = new ErrorViewModel()
                    {
                        Title = "Profil Güncellenemedi.",
                        Items = res.Errors,
                        RedirectingUrl = "/Home/EditProfile"
                    };

                    return View("Error", errorNotifyObj);
                }
                CurrentSession.Set<EvernoteUser>("login",res.Result);
                return RedirectToAction("ShowProfile");

            }
            return View(model);
        }
        [Auth]
        public ActionResult DeleteProfile()
        {
            BusinessLayerResult<EvernoteUser> res = evernoteUserManager.RemoveUserById(CurrentSession.User.Id);

            if (res.Errors.Count>0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Items = res.Errors,
                    Title="Profile Silinemedi",
                    RedirectingUrl="/Home/ShowProfile"
                };
                return View("Error", errorNotifyObj);
            }
            Session.Clear();

            return RedirectToAction("Index");
        }
        

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                BusinessLayerResult<EvernoteUser> res = evernoteUserManager.LoginUser(model);
                if (res.Errors.Count > 0)
                {
                    if (res.Errors.Find(x => x.Code == ErrorMessageCode.UserIsNotActive) != null)
                    {
                        ViewBag.SetLink = "E-Posta Gönder";
                    }
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(model);
                }

                CurrentSession.Set<EvernoteUser>("login",res.Result);
                return RedirectToAction("Index");
            }

            return View(model);
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                BusinessLayerResult<EvernoteUser> res= evernoteUserManager.RegisterUser(model);

                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));

                   

                    return View(model);
                }
                OkViewModel notifyObj = new OkViewModel()
                {
                    Title = "Kayıt Başarılı",
                    RedirectingUrl="/Home/Login",
                    
                };
                notifyObj.Items.Add("Lütfen E-posta adresinize gönderdiğimiz aktivasyon linkine tıklayarak hesabınızı aktive ediniz. Hesabınızı aktive etmeden not ekleyemez ve beğenme yapamazsınız..");
                return View("Ok",notifyObj);
            }
            return View(model);
        }
        
        public ActionResult UserActivate(Guid id)
        { 
            BusinessLayerResult<EvernoteUser> res = evernoteUserManager.ActivateUser(id);
            if (res.Errors.Count>0)
            {
                ErrorViewModel errornotifyObj = new ErrorViewModel() {
                    Title="Geçersiz İşlem",
                    Items=res.Errors
                };
                
                return View("Error",errornotifyObj);
            }
            OkViewModel oknotifyObj = new OkViewModel() {
                Title="Hesap Aktifleştirildi..",
                RedirectingUrl="/Home/Login"
            };
            oknotifyObj.Items.Add("Hesabınız Aktifleştirildi. Artık Not paylaşabilir ve Beğenebilirsiniz..");

            return View("Ok",oknotifyObj);
        }
       
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult HasError()
        {
            return View();
        }
    }
}