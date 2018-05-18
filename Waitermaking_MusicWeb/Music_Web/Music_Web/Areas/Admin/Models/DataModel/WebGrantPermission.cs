using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Music_Web.Areas.Admin.Models.DataModel
{
    [Table("WebGrantPermission")]
    public class WebGrantPermission
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("WebPermission")]
        [Display(Name = "Mã quyền hạn")]
        [Required]
        public int PermissionId { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("WebUser")]
        [Display(Name = "Mã người dùng")]
        [Required]
        public int UserId { get; set; }

        [Display(Name = "Mô tả")]
        [MaxLength(256)]
        public string Description { get; set; }
        // //navigation
        public virtual WebPermission WebPermission { get; set; }
        public virtual WebUser WebUser { get; set; }
    }
}