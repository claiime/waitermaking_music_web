using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Music_Web.Areas.Admin.Models.DataModel
{
    [Table("WebUser")]
    public class WebUser
    {
        [Key]
        [Display(Name = "Mã số")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        //[RegularExpression("([a-zA-Z0-9 .&'-]+)", ErrorMessage = "Tên chỉ gồm chữ và số")]
        [Required(ErrorMessage = "Hãy nhập tên đăng nhập")]
        [StringLength(64, ErrorMessage = " Tên đăng nhập phải trong khoảng 3-64 kí tự", MinimumLength = 3)]
        [Column(TypeName = "varchar")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }


        // [RegularExpression]
        [Required(ErrorMessage = "Hãy nhập mật khẩu")]
        [MaxLength(256)]
        [DataType(DataType.Password)]
        [Column(TypeName = "varchar")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Hãy nhập họ và tên")]
        [MaxLength(64)]
        [Display(Name = "Tên đầy đủ")]
        public string FullName { get; set; }


        [Display(Name = "Ngày tạo")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        //public DateTime? Date { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\b\d{3,4}[-. ]?\d{3}[-. ]?\d{3,4}\b$", ErrorMessage = "Không đúng định dạng")]
        [Required(ErrorMessage = "Hãy nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        [Column(TypeName = "varchar")]
        [MaxLength(50)]
        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Email không đúng dịnh dạng")]
        [MaxLength(256)]
        [DataType(DataType.EmailAddress)]
        [Column(TypeName = "varchar")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [MaxLength(256)]
        [Column(TypeName = "varchar")]
        [Display(Name = "Ảnh đại diện")]
        public string Avatar { get; set; }


        [Display(Name = "Là quản trị")]
        public bool IsAdmin { get; set; }


        [Display(Name = "Kích hoạt")]
        public bool Allowed { get; set; }


        [Display(Name = "Giới tính")]
        [MaxLength(50)]
        public string GioiTinh { get; set; }

        //Thuộc tính Navigation
        public ICollection<WebGrantPermission> WebGrantPermissions { get; set; }
        public ICollection<WebAlbumUser> WebAlbums { get; set; }
    }
}