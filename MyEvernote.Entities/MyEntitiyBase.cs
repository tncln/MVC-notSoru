using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Entities
{
    public class MyEntitiyBase
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity )]
        public int Id { get; set; }

        [Required, DisplayName("Kayıt Tarihi")]
        public DateTime CreatedOn { get; set; }

        [Required,DisplayName("Güncelleme Tarihi")]
        public DateTime ModifiedOn { get; set; }

        [Required,StringLength(30), DisplayName("Güncelleyen Kullanıcı")]
        public string ModifiedUsername { get; set; }

    }
}
