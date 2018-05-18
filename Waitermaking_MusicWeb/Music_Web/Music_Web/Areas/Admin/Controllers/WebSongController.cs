using Music_Web.Areas.Admin.Models.BusinessModel;
using Music_Web.Models;
using Music_Web.Models.Waitermak;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace Music_Web.Areas.Admin.Controllers
{
    [AuthorizeBusiness]
    public class WebSongController : Controller
    {
        public static String folderIdNhacgoc = "1XNhsy68LFxJcS-Wbpfy7dn9o02g_PtHf";
        public static String folderIdWaitermak = "1BYORqPY2t7ose0xUbjPwSSUGZIi-Bpn9";
        public static String folderUserUp = "1HwOimOh6QJqn4qhd8faX4cEQo5Hhwkhx";

        [HttpGet]
        public ActionResult Index()
        {

            return View(GoogleDriveFilesRepository.GetDriveFiles());
        }

        [HttpGet]
        public ActionResult GetContainsInFolderRootMusic(int? page = 1)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 8;
            return View(GoogleDriveFilesRepository.GetContainsInFolder(folderIdNhacgoc).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult GetContainsInFolderWaitermak(int? page = 1,string kq="")
        {
            int pageNumber = (page ?? 1);
            int pageSize = 8;
            ViewBag.Kq =kq.ToString();
            return View(GoogleDriveFilesRepository.GetContainsInFolder(folderIdWaitermak).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult GetContainsInFolderUserUp(int? page = 1)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 8;
            return View(GoogleDriveFilesRepository.GetContainsInFolder(folderUserUp).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult ExtractFile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ExtractFile(string fileID)
        {
            string key = "N14DCAT082";//key extract file
            string namefile;//tên file 
            string downloadPath;//vị trí file user tải về để tiến hành extract
            string kq;//kq sau extract

            namefile = GoogleDriveFilesRepository.FindFile(folderIdWaitermak, fileID).Name;//tìm tên bài hát
            downloadPath = GoogleDriveFilesRepository.DownloadGoogleFile(fileID, namefile.Substring(0, namefile.Length - 4) + "-userup-extract.wav");//server tải ve lưu để , extract message
            kq = WaitermakHelper.callExtract(downloadPath, key);//truyền vị trí file và key để extract

            var uri = new Uri(downloadPath, UriKind.Absolute);//xóa file đã down về
            System.IO.File.Delete(uri.LocalPath);
            return RedirectToAction("GetContainsInFolderWaitermak", new { kq = kq });//truyền kq hiển thị
        }
        [HttpPost]
        public ActionResult UploadFileRoot(HttpPostedFileBase file)
        {
            GoogleDriveFilesRepository.FileUploadInFolderUser(folderIdNhacgoc, file);
            return RedirectToAction("GetContainsInFolderRootMusic");
        }
        public ActionResult DeleteFolderRoot(string id="")
        {
            GoogleDriveFilesRepository.Delete(id);
            return RedirectToAction("GetContainsInFolderRootMusic");
        }
        public ActionResult DeleteFolderWai(string id = "")
        {
            GoogleDriveFilesRepository.Delete(id);
            return RedirectToAction("GetContainsInFolderWaitermak");
        }



    }
}
