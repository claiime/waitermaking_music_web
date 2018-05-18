using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Web;
using Music_Web.Areas.Admin.Models.DataModel;
using Google.Apis.Download;

namespace Music_Web.Models
{
    public class GoogleDriveFilesRepository
    {
        public static string[] Scopes = { Google.Apis.Drive.v3.DriveService.Scope.Drive };
        public static Google.Apis.Drive.v3.DriveService GetService_v3()
        {
            UserCredential credential;
            using (var stream = new FileStream(HttpContext.Current.Server.MapPath("~/GoogleDriveApi/client_secret.json"), FileMode.Open, FileAccess.Read))
            {
                String FolderPath = HttpContext.Current.Server.MapPath("~/GoogleDriveApi");
                String FilePath = Path.Combine(FolderPath, "DriveServiceCredentials-foldermusic.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(FilePath, true)).Result;
            }

            //Create Drive API service.
            Google.Apis.Drive.v3.DriveService service = new Google.Apis.Drive.v3.DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "ReadFileFolder",
            });

            return service;
        }

        public static Google.Apis.Drive.v2.DriveService GetService_v2()
        {
            UserCredential credential;
            using (var stream = new FileStream(HttpContext.Current.Server.MapPath("~/GoogleDriveApi/client_secret.json"), FileMode.Open, FileAccess.Read))
            {
                String FolderPath = HttpContext.Current.Server.MapPath("~/GoogleDriveApi");
                String FilePath = Path.Combine(FolderPath, "DriveServiceCredentials-foldermusic.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(FilePath, true)).Result;
            }

            //Create Drive API service.
            Google.Apis.Drive.v2.DriveService service = new Google.Apis.Drive.v2.DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "ReadFileFolder",
            });
            return service;
        }
        public static void Delete(string fileid)
        {
            Google.Apis.Drive.v3.DriveService service = GetService_v3();
            service.Files.Delete(fileid).Execute();
            
        }
        public static List<WebSong> GetContainsInFolder(String folderId)
        {
            List<string> ChildList = new List<string>();
            Google.Apis.Drive.v2.DriveService ServiceV2 = GetService_v2();
            ChildrenResource.ListRequest ChildrenIDsRequest = ServiceV2.Children.List(folderId);
            do
            {
                ChildList children = ChildrenIDsRequest.Execute();

                if (children.Items != null && children.Items.Count > 0)
                {
                    foreach (var file in children.Items)
                    {
                        ChildList.Add(file.Id);
                    }
                }
                ChildrenIDsRequest.PageToken = children.NextPageToken;

            } while (!String.IsNullOrEmpty(ChildrenIDsRequest.PageToken));

            //Get All File List
            List<WebSong> AllFileList = GetDriveFiles();
            List<WebSong> Filter_FileList = new List<WebSong>();

            foreach (string Id in ChildList)
            {
                Filter_FileList.Add(AllFileList.Where(x => x.Id == Id).FirstOrDefault());
            }
            return Filter_FileList;
        }

        public static void CreateFolder(string FolderName)
        {
            Google.Apis.Drive.v3.DriveService service = GetService_v3();

            var FileMetaData = new Google.Apis.Drive.v3.Data.File();
            FileMetaData.Name = FolderName;
            FileMetaData.MimeType = "application/vnd.google-apps.folder";

            Google.Apis.Drive.v3.FilesResource.CreateRequest request;

            request = service.Files.Create(FileMetaData);
            request.Fields = "id";
            var file = request.Execute();
            Console.WriteLine("Folder ID: " + file.Id);
        }
        public static String FileUploadInFolder(string folderId,string path)
        {
            var file = new Google.Apis.Drive.v3.Data.File();
         
                Google.Apis.Drive.v3.DriveService service = GetService_v3();
            
                var FileMetaData = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = Path.GetFileName(path),
                    MimeType = MimeMapping.GetMimeMapping(path),
                    Parents = new List<string>
                    {
                        folderId
                    }
                };
              Google.Apis.Drive.v3.FilesResource.CreateMediaUpload request;
                using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
                {
                    request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
                    request.Fields = "id";
                    request.Upload();
                }
                var file1 = request.ResponseBody;
            return file1.Id;
            }
        public static String FileUploadInFolderUser(string folderId, HttpPostedFileBase file)
        {
       
            if (file != null && file.ContentLength > 0)
            {
                Google.Apis.Drive.v3.DriveService service = GetService_v3();

                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/GoogleDriveFiles"),
                Path.GetFileName(file.FileName));
                file.SaveAs(path);

                var FileMetaData = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = Path.GetFileName(file.FileName),
                    MimeType = MimeMapping.GetMimeMapping(path),
                    Parents = new List<string>
                    {
                        folderId
                    }
                };

                Google.Apis.Drive.v3.FilesResource.CreateMediaUpload request;
                using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
                {
                    request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
                    request.Fields = "id";
                    request.Upload();
                }
                var file1 = request.ResponseBody;
                var uri = new Uri(path, UriKind.Absolute);
                System.IO.File.Delete(uri.LocalPath);
                return file1.Id;
            }
            else
            {
                return "";

            }
            
        }

        public static List<WebSong> GetDriveFiles()
        {
            Google.Apis.Drive.v3.DriveService service = GetService_v3();

            // Define parameters of request.
            Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest = service.Files.List();
            FileListRequest.Fields = "nextPageToken, files(createdTime, id, name, size, version, trashed, parents)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            List<WebSong> FileList = new List<WebSong>();

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    WebSong File = new WebSong
                    {
                        Id = file.Id,
                        Name = file.Name,
                        Size = file.Size,
                        Version = file.Version,
                        CreatedTime = file.CreatedTime,
                        Parents = file.Parents
                    };
                    FileList.Add(File);
                }
            }
            return FileList;
        }
        //Download file from Google Drive by fileId.
        public static WebSong FindFile(String folderId,String mabaihat)
        {
            List<string> ChildList = new List<string>();
            Google.Apis.Drive.v2.DriveService ServiceV2 = GetService_v2();
            ChildrenResource.ListRequest ChildrenIDsRequest = ServiceV2.Children.List(folderId);
            do
            {
                ChildList children = ChildrenIDsRequest.Execute();

                if (children.Items != null && children.Items.Count > 0)
                {
                    foreach (var file in children.Items)
                    {
                        ChildList.Add(file.Id);
                    }
                }
                ChildrenIDsRequest.PageToken = children.NextPageToken;

            } while (!String.IsNullOrEmpty(ChildrenIDsRequest.PageToken));

            //Get All File List
            List<WebSong> AllFileList = GetDriveFiles();
            List<WebSong> Filter_FileList = new List<WebSong>();
            WebSong baihat=new WebSong();
            foreach (string Id in ChildList)
            {
                if (Id.Equals(mabaihat))
                {
                    baihat = AllFileList.Where(x => x.Id == Id).FirstOrDefault();
                }
                
            }
            return baihat;
        }

        //Download file from Google Drive by fileId.

        //Download file from Google Drive by fileId.
        public static string DownloadGoogleFile(string fileId,String FileName)//filename là tên muốn đặt khi lưu
        {
            Google.Apis.Drive.v2.DriveService service = GetService_v2();

            string FolderPath = System.Web.HttpContext.Current.Server.MapPath("/GoogleDriveFiles/");
            FilesResource.GetRequest request = service.Files.Get(fileId);

          //  string FileName = request.Execute().Name;
            string FilePath = System.IO.Path.Combine(FolderPath, FileName);

            MemoryStream stream1 = new MemoryStream();

            // Add a handler which will be notified on progress changes.
            // It will notify on each chunk download and when the
            // download is completed or failed.

            request.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case DownloadStatus.Downloading:
                        {
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                    case DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete.");
                            SaveStream(stream1, FilePath);
                            break;
                        }
                    case DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            break;
                        }
                }
            };
            request.Download(stream1);
            return FilePath;
        }

        // file save to server path
        private static void SaveStream(MemoryStream stream, string FilePath)
        {
            using (System.IO.FileStream file = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                stream.WriteTo(file);
            }
        }
    }

}