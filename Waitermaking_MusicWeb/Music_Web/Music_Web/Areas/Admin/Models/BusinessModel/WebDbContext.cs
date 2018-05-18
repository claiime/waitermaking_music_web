using Music_Web.Areas.Admin.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Music_Web.Areas.Admin.Models.BusinessModel
{
    public class WebDbContext:DbContext
    {
        public WebDbContext() : base("name=WebDbContextConnectionString")
        {

        }
        public DbSet<WebUser> User { get; set; }
        public DbSet<WebPermission> Permission { get; set; }
        public DbSet<WebGrantPermission> GrantPermission { get; set; }
        public DbSet<WebBusiness> Business { get; set; }
        public DbSet<WebAlbumUser> AlbumUser { get; set; }
        public DbSet<WebManagerFile> ManagerFile { get; set; }

        public System.Data.Entity.DbSet<Music_Web.Areas.Admin.Models.DataModel.WebSong> WebSongs { get; set; }
    }
   
}