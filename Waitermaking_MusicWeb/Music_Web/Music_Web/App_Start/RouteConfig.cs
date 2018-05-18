using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Music_Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();

           
            routes.MapRoute(
               name: "Lienhe",
               url: "Trang-chu/Lien-he",
               defaults: new { controller = "Home", action = "LienHe" }
               );
            routes.MapRoute(
                name: "Dangky",
                url: "Thanh-vien/Dang-ky",
                defaults: new { controller = "Users", action = "DangKy" }
                );
            routes.MapRoute(
               name: "Dangnhap",
               url: "Thanh-vien/Dang-nhap",
               defaults: new { controller = "Users", action = "DangNhap" }
               );
            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces:new string[] {"Music_Web.Controllers"}
                );
        }
    }
}
