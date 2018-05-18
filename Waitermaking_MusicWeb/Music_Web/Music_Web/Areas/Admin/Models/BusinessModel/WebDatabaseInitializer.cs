using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Music_Web.Areas.Admin.Models.DataModel;
using Music_Web.Areas.Admin.Models.BusinessModel;

namespace Music_Web.Areas.Admin.Models.BussinessModel
{
    public class WebDatabaseInitializer : DropCreateDatabaseIfModelChanges<WebDbContext>
    {
        protected override void Seed(WebDbContext context)
        {
            var admin = new WebUser()
            {
                UserName = "thly",
                Password = "7c98ab5678fd6851fea8b2f6de975646",
                FullName = " Thiên Lý",
                Avatar = "/Areas/Admin/Image/avatar.png",
                Email = "admin@gmail.com",
                PhoneNumber = "01234556778",
                Date = DateTime.Now,
                IsAdmin = true,
                Allowed = true,
                GioiTinh = "nữ"

            };
            context.User.Add(admin);
            var user01 = new WebUser()
            {
                UserName = "user01",
                Password = "2338fc896c38cff5e9db38ee021e35a",
                FullName = "Nguyễn Văn A",
                Avatar = "/Areas/Admin/Image/avatar.png",
                Email = "VanA@gmail.com",
                IsAdmin = false,
                Date = DateTime.Now,
                PhoneNumber = "01234556778",
                Allowed = true,
                GioiTinh = "nam"

            };
            context.User.Add(user01);
            context.SaveChanges();
        }
    }
}