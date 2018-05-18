using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Music_Web.Areas.Admin.Models.DataModel
{
    [Table("WebBusiness")]
    public class WebBusiness
    {
        [Key]
        [Display(Name = "Mã chức năng")]
        [Column(TypeName = "varchar")]
        [MaxLength(64)]
        public string BusinessId { get; set; }

        [Required(ErrorMessage = "Hãy nhập tên chức năng")]
        [Display(Name = "Tên chức năng")]
        [MaxLength(256)]
        public string BusinessName { get; set; }

        public virtual ICollection<WebPermission> WebPermissions { get; set; }
    }
}