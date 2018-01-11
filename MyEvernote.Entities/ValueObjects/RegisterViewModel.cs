using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyEvernote.Entities.ValueObjects
{
    public class RegisterViewModel
    {
        [DisplayName("Kullanıcı Adı"), 
            Required(ErrorMessage = "{0} Alanı Boş Geçilemez."), 
            StringLength(25, ErrorMessage = "{0} Max {1} Karakter olmalı..")]
        public string UserName { get; set; }

        [DisplayName("Eposta"), 
            Required(ErrorMessage = "{0} Alanı Boş Geçilemez."),
            StringLength(70, ErrorMessage = "{0} Max {1} Karakter olmalı.."), 
            EmailAddress(ErrorMessage = "{0} Alanı için geçerli bir E-posta adresi giriniz..")]
        public string Email { get; set; }

        [DisplayName("Şifre"), 
            Required(ErrorMessage = "{0} alanı boş geçilemez."), 
            DataType(DataType.Password), 
            StringLength(25, ErrorMessage = "{0} Max {1} Karakter olmalı..")]
        public string Password { get; set; }

        [DisplayName("Şifre Tekrar"), 
            Required(ErrorMessage = "{0} alanı boş geçilemez."), 
            DataType(DataType.Password), StringLength(25, 
            ErrorMessage = "{0} Max {1} Karakter olmalı.."),
            Compare("Password",ErrorMessage ="{0} {1} uyuşmuyor")]
        public string RePassword { get; set; }
    }
}