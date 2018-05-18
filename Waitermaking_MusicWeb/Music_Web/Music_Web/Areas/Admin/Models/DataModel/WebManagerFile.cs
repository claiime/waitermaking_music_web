using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
namespace Music_Web.Areas.Admin.Models.DataModel
{
    [Table("ManagerFile")]
    public class WebManagerFile
    {
        [Key]
        [Display(Name = "Mã số")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdSong { get; set; }

       
        [Display(Name = "Mã gốc")]
        public string FileID { get; set; }
        
        [Display(Name = "Tên bài hát")]
        public string NameSong { get; set; }
        
        [Display(Name = "Mã sau mã hóa")]
        public string FileIDAf { get; set; }
        
        [Display(Name = "Tên sau mã hóa")]
        public string NameSongAf { get; set; }
    }
}