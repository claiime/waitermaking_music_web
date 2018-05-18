using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
namespace Music_Web.Areas.Admin.Models.DataModel
{
    [Table("WebPermission")]
    public class WebPermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PermissionId { get; set; }

        [Required(ErrorMessage = "Hãy nhập tên quyền hạn")]
        [Display(Name = "Tên quyền hạn")]
        [Column(TypeName = "varchar")]
        [MaxLength(256)]
        public string PermissionName { get; set; }

        [Required(ErrorMessage = "Hãy nhập mô tả quyền hạn")]
        [Display(Name = "Mô tả")]
        [MaxLength(256)]
        public string Description { get; set; }

        [Required()]
        [Display(Name = "mã chức năng")]
        [ForeignKey("WebBusinesses")]
        [Column(TypeName = "varchar")]
        [MaxLength(64)]
        public string BusinessId { get; set; }

        //navigation
        public virtual WebBusiness WebBusinesses { get; set; }
        public virtual ICollection<WebGrantPermission> WebGrantPermissions { get; set; }


    }
}