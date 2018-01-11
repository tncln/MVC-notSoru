using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyEvernote.Entities.ValueObjects
{
    public class LoginViewModel
    {
        [DisplayName("Kullanıcı Adı"), Required(ErrorMessage = "{0} Alanı Boş Geçilemez."), StringLength(25, ErrorMessage = "{0} Max {1} Karakter olmalı..")]
        public string UserName { get; set; }
        [DisplayName("Şifre"), Required(ErrorMessage = "{0} alanı boş geçilemez."),DataType(DataType.Password), StringLength(25, ErrorMessage = "{0} Max {1} Karakter olmalı..")]
        public string Password { get; set; }
    }
}