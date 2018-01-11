using MyEvernote.DataAccessLayer.EntityFramework;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyEvernote.Common.Helpers;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.BusinessLayer.Abstract;

namespace MyEvernote.BusinessLayer
{
    public class EvernoteUserManager : ManagerBase<EvernoteUser>
    {


        public BusinessLayerResult<EvernoteUser> RegisterUser(RegisterViewModel data)
        {
            //Kullanıcı Username kontrolü 
            //Kullanıcı eposta kontrolü
            //Kayıt işlemi 
            //Aktivasyon Eposta gönderimi 
            EvernoteUser user = Find(x => x.Username == data.UserName || x.Email == data.Email);
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            if (user != null)
            {
                if (user.Username == data.UserName)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı Adı Kullanılıyor..");
                }
                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "E-posta adresi kayıtlı..");
                }
            }
            else
            {
                int dbResult = base.Insert(new EvernoteUser()
                {
                    Username = data.UserName,
                    ProfileImageFilename = "user_profile.gif",
                    Email = data.Email,
                    Password = data.Password,
                    ActivateGuid = Guid.NewGuid(),

                    IsActive = false,
                    IsAdmin = false

                });
                if (dbResult > 0)
                {
                    res.Result = Find(x => x.Email == data.Email && x.Username == data.UserName);
                    //activate maili..
                    string siteUri = ConfigHelper.Get<string>("SiteRootUri");
                    string activateUri = $"{siteUri}/Home/UserActivate/{res.Result.ActivateGuid}";
                    string body = $"Merhaba {res.Result.Surname}; <br><br> Hesabınızı aktifleştirmek için <a href='{activateUri}' target='_blank'> tıklayınız</a>";
                    MailHelper.SendMail(body, res.Result.Email, "Hesap Aktifleştirme");



                }
            }

            return res;

        }

        public BusinessLayerResult<EvernoteUser> GetUserById(int id)
        {
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = Find(x => x.Id == id);
            if (res.Result == null)
            {
                res.AddError(ErrorMessageCode.UserNotFound, "Kullanıcı Bulunamadı");
            }
            return res;
        }

        public BusinessLayerResult<EvernoteUser> LoginUser(LoginViewModel data)
        {

            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = Find(x => x.Username == data.UserName || x.Password == data.Password);

            if (res.Result != null)
            {
                if (!res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCode.UserIsNotActive, "Kullanıcı Aktifleştirilmedi..");
                    res.AddError(ErrorMessageCode.CheckYourEmail, "Lütfen E-posta adresinizi kontrol ediniz..");
                }

            }
            else
            {
                res.AddError(ErrorMessageCode.UsernameorPassWrong, "Kullanıcı Adı veya Şifre Hatalı!");
            }
            return res;
        }

        public BusinessLayerResult<EvernoteUser> UpdateProfile(EvernoteUser data)
        {
            EvernoteUser db_user = Find(x => x.Id != data.Id && (x.Username == data.Username || x.Email == data.Email));
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();

            if (db_user != null && db_user.Id != data.Id)
            {
                if (db_user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı Adı Kullanılıyor..");
                }
                if (db_user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "E-posta adresi kayıtlı..");
                }
                return res;
            }
            res.Result = Find(x => x.Id == data.Id);
            res.Result.Email = data.Email;
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Password = data.Password;
            res.Result.Username = data.Username;

            if (string.IsNullOrEmpty(data.ProfileImageFilename) == false)
            {
                res.Result.ProfileImageFilename = data.ProfileImageFilename;
            }
            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.UserCouldNotUpdate, "Profil Güncellenemedi.");
            }
            return res;
        }

        public BusinessLayerResult<EvernoteUser> RemoveUserById(int id)
        {

            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            EvernoteUser user = Find(x => x.Id == id);

            if (user != null)
            {
                if (Delete(user) == 0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotRemove, "Kullanıcı Silinemedi");
                    return res;
                }

            }
            else
            {
                res.AddError(ErrorMessageCode.UserCouldNotRemove, "Kullanıcı bulunamadı.");
            }
            return res;
        }

        public BusinessLayerResult<EvernoteUser> ActivateUser(Guid activateId)
        {
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = Find(x => x.ActivateGuid == activateId);
            if (res.Result != null)
            {
                if (res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCode.UserAlreadyActive, "Kullanıcı Aktif edilmiştir.");
                    return res;
                }
                res.Result.IsActive = true;
                Update(res.Result);
            }
            else
            {
                res.AddError(ErrorMessageCode.ActivateIdDoesNotExists, "Aktifleştirilecek kullanıcı bulunamadı.");
            }
            return res;
        }

        public new  BusinessLayerResult<EvernoteUser> Insert(EvernoteUser data)
        {
            EvernoteUser user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = data;
            if (user != null)
            {
                if (user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı Adı Kullanılıyor..");
                }
                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "E-posta adresi kayıtlı..");
                }
            }
            else
            {
                res.Result.ProfileImageFilename = "user_profile.gif";
                res.Result.ActivateGuid = Guid.NewGuid();
                if(base.Insert(res.Result) == 0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotInserted, "Kullanıcı Eklenemedi");
                }
               
            }

            return res;

        }

        public new BusinessLayerResult<EvernoteUser> Update(EvernoteUser data)
        {
            EvernoteUser db_user = Find(x => x.Id != data.Id && (x.Username == data.Username || x.Email == data.Email));
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = data;
            if (db_user != null && db_user.Id != data.Id)
            {
                if (db_user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı Adı Kullanılıyor..");
                }
                if (db_user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "E-posta adresi kayıtlı..");
                }
                return res;
            }
            res.Result = Find(x => x.Id == data.Id);
            res.Result.Email = data.Email;
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Password = data.Password;
            res.Result.Username = data.Username;
            res.Result.IsActive = data.IsActive;
            res.Result.IsAdmin = data.IsAdmin;
            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.UserCouldNotUpdate, "Kullanıcı Güncellenemedi.");
            }
            return res;
        }
    }
}
