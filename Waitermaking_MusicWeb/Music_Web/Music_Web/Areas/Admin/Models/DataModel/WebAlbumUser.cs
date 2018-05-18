using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Music_Web.Areas.Admin.Models.DataModel
{
    [Table("WebAlbumUser")]
    public class WebAlbumUser
    {
        [Key]
        [Display(Name = "Mã số Album")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Mã người dùng")]
        [ForeignKey("WebUser")]
        public int UserId { get; set; }

        /*
        [Display(Name = "Mã bài hat")]
        [ForeignKey("WeBaihat")]
        public string SoName{ get; set; }*/

        [Display(Name = "Mã bài hát")]
        [Column(TypeName = "ntext")]
        public string IdSong { get; set; }

        [Display(Name = "Ngày tải")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime ngayTai{ get; set; }
        
        
        //navigation
        public virtual WebUser WebUser { get; set; }
       // public virtual googledrive       //ket noi googledrive APi
    }
}