
using Music_Web.Areas.Admin.Models.BusinessModel;
using Music_Web.Areas.Admin.Models.DataModel;
using Music_Web.Models;
using Music_Web.Models.Waitermak;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace Music_Web.Controllers
{
    public class HomeController : Controller
    {
        private WebDbContext db = new WebDbContext();
        public static String folderIdNhacgoc = "1XNhsy68LFxJcS-Wbpfy7dn9o02g_PtHf";
        public static String folderIdWaitermak = "1BYORqPY2t7ose0xUbjPwSSUGZIi-Bpn9";
        public static String folderUserUp = "1HwOimOh6QJqn4qhd8faX4cEQo5Hhwkhx";
        public static String fistsongID = GoogleDriveFilesRepository.GetContainsInFolder(folderIdNhacgoc)[0].Id;
        public ActionResult Index(int dem=1, string mabaihat="0",string fileDown= "")
        {         
            
            if (mabaihat.Equals("0"))
            {
                mabaihat = fistsongID;
            }
            WebSong baihat = GoogleDriveFilesRepository.FindFile(folderIdNhacgoc, mabaihat);//tìm bài hát trong googledrive với key truyền vào
            if (baihat == null)
            {
                //trả về trang báo lỗi
                Response.StatusCode = 404;
                return View("Loi404");
            }
            ViewBag.FileDown = fileDown.ToString();
            ViewBag.Dem = dem.ToString();
            return View(baihat);
        }
       
        public ActionResult MyAudio(string mabaihat,string name,string infoUser,string dem)
        {
            string idFileUp; //id của file sau khi waitermak tải trên googleapi
            string key = "N14DCAT082"; //key để hide thông tin
            string dowload = "";//vị trí file down về
            string fileWaitermak = "";//lấy tên file tải về, đặt lại tên sau khi waitermak
            dowload = GoogleDriveFilesRepository.DownloadGoogleFile(mabaihat, name);
            fileWaitermak = dowload.Substring(0, dowload.Length - 4) + "-waitermak.wav";
             WaitermakHelper.callHide(dowload, fileWaitermak,key,infoUser);//ẩn thông tin user
             //uplen tu googledrivefiles
             idFileUp = GoogleDriveFilesRepository.FileUploadInFolder(folderIdWaitermak,fileWaitermak); //up load tu path vào folder waitermaking
             //xóa file đã lưu và  file waitermak trên server                                                                                        //lay id file da up, xoa path return
                var uri = new Uri(dowload, UriKind.Absolute);
                System.IO.File.Delete(uri.LocalPath);
                var uriWaitermak = new Uri(fileWaitermak, UriKind.Absolute);
                System.IO.File.Delete(uriWaitermak.LocalPath);
            
            string filedown= "https://docs.google.com/uc?export=download&id=" + idFileUp.ToString();//trả về filedown(link hoặc ID file)
            return RedirectToAction("Index", new { dem=dem,mabaihat=mabaihat,fileDown=filedown });
            }
        public ActionResult LienHe()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
        public ActionResult Gioithieu(String kq="")
        {
            ViewBag.Message = "Your about page.";
            ViewBag.Kq = kq;
            return View();
        }
        public ActionResult List_BaiHat(int? page = 1)
        {

            int pageNumber = (page ?? 1);
            int pageSize = 8;
            ViewBag.Message = "Danh sách bài hát";
            List<WebSong> list = GoogleDriveFilesRepository.GetContainsInFolder(folderIdNhacgoc);//danh sach nhac goc
          
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult UserUpFile()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult UserUpFile( HttpPostedFileBase file)
        {
            string idfileUp = "";//id file đã up
            string key = "N14DCAT082";//key extract file
            string namefile;//tên file đã up
            string downloadPath;//vị trí file user tải về để tiến hành extract
            string kq;//kq sau extract
            idfileUp=GoogleDriveFilesRepository.FileUploadInFolderUser(folderUserUp, file);//id file user tải len trong googledrive
            namefile=GoogleDriveFilesRepository.FindFile(folderUserUp, idfileUp).Name;//tìm tên bài hát user đã up
            downloadPath = GoogleDriveFilesRepository.DownloadGoogleFile(idfileUp,namefile.Substring(0,namefile.Length-4)+"-userup.wav");//server tải ve lưu giống với tên mới , extract message
           kq = WaitermakHelper.callExtract(downloadPath, key);
            var urifileDown = new Uri(downloadPath, UriKind.Absolute);//xóa file sau khi extract
            System.IO.File.Delete(urifileDown.LocalPath);
            return RedirectToAction("Gioithieu", new { kq =kq});//truyền kq hiển thị
        }
    }
}