using MeherEstateDevelopers.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeherEstateDevelopers.Filters;
using static MeherEstateDevelopers.MvcApplication;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class FileHandlingController : Controller
    {
        // GET: FileHandling
        private const int ThumbSize = 80;
        [NoDirectAccess] public ActionResult Index()
        {
            return View();
        }

        [NoDirectAccess] public ActionResult NewVersion(string FileName, string FolderPath)
        {
            string Name = FileName.Split('.')[0];
            var folder = GetUploadFolder(FolderPath + "/Versions" );
            var FileDetails = GetFile(FileName, FolderPath);
            var version = folder.GetDirectories().Count() + 1;
            ViewBag.Version = version;
            ViewBag.NewPath = FolderPath + "/Versions/" + version;
            ViewBag.MainPath = FolderPath ;
            ViewBag.FileName = FileDetails.Name;
            
            return PartialView();
        }

        [HttpGet]
        [NoDirectAccess] public ActionResult UploadNewVersion(string FolderPath, IEnumerable<string> names = null)
        {
            FileHandlingController FH = new FileHandlingController();
            var files = FH.Files(FolderPath);
            return Json(new
            {
                files
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [NoDirectAccess] public ActionResult UploadNewVersion(List<string> Names,string NewPath,string MainPath,int Version,string FileName)
        {
            var files = Request.Files
                .Cast<string>()
                .Select(k => Request.Files[k])
                .ToArray();
            var names = new List<string>();

            FileHandlingController FH = new FileHandlingController();
            foreach (var file in files)
            {
                var name = FH.SaveFile(file, MainPath);
                names.Add(name);
            }
            // Move File into Verions Folder
            var nfolder = FH.GetFolder(NewPath);
            FH.MoveFile(MainPath + "/" + FileName, NewPath + "/" + FileName);

            IEnumerable<string> jsonObject = FH.FileNames(MainPath, names);
            return UploadNewVersion(MainPath, jsonObject);
        }



        public DirectoryInfo GetFolder(string Path)
        {
            return GetUploadFolder(Path);
        }
        private static DirectoryInfo GetDirecotry(string Path)
        {

            var serverPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Repository/" + Path + "/");
            var di = new DirectoryInfo(serverPath);
            return di;
        }
        private static DirectoryInfo GetUploadFolder(string Path)
        {

            var serverPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Repository/" + Path + "/");
            var di = new DirectoryInfo(serverPath);
            if (!di.Exists)
                di.Create();

            return di;
        }
        private static FileInfo GetFile(string name, string FolderPath)
        {
            var folder = GetUploadFolder(FolderPath);
            var file = folder.GetFiles(name).Single();
            return file;
        }
        [HttpPost]
        [NoDirectAccess] public ActionResult DeleteFile(string name, string FolderPath)
        {
            var file = GetFile(name, FolderPath);
            file.Delete();
            return Json($"{name} was deleted");
        }
        public string SaveFile(HttpPostedFileBase file, string FolderPath)
        {
            return SaveFileToDisk(file, FolderPath);
        }
        private static string SaveFileToDisk(HttpPostedFileBase file, string FolderPath)
        {
            var folder = GetUploadFolder(FolderPath);
            var temp = folder.FullName + "\\" + file.FileName;
            file.SaveAs(temp);
            return file.FileName;
        }
        [NoDirectAccess] public ActionResult DownloadUrl(string name, string FolderPath)
        {
            string serverPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Repository/" + FolderPath + "/");
            serverPath = Path.Combine(serverPath, name);
            return File(serverPath, System.Net.Mime.MediaTypeNames.Application.Octet, name);
        }
        public List<string> FileNames(string FolderPath, IEnumerable<string> names = null)
        {
            return getName(FolderPath, names);
        }
        private List<string> getName(string FolderPath, IEnumerable<string> names = null)
        {
            var files = this.Files(FolderPath, names);
            var namesForward = new List<string>();
            foreach (var file in files)
            {
                namesForward.Add(file.name);
            }

            return namesForward;
        }
        [NoDirectAccess] public ActionResult GetFile(string name, string FolderPath, bool thumbnail = false)
        {
            var file = GetFile(name, FolderPath);
            var contentType = MimeMapping.GetMimeMapping(file.Name);

            return thumbnail
                ? Thumb(file, contentType)
                : File(file.FullName, contentType);
        }
        private ActionResult Thumb(FileInfo file, string contentType)
        {
            if (contentType.StartsWith("image"))
            {
                try
                {
                    using (var img = Image.FromFile(file.FullName))
                    {
                        var thumbHeight = (int)(img.Height * (ThumbSize / (double)img.Width));

                        using (var thumb = img.GetThumbnailImage(ThumbSize, thumbHeight, () => false, IntPtr.Zero))
                        {
                            var ms = new MemoryStream();
                            thumb.Save(ms, img.RawFormat);
                            ms.Position = 0;
                            return File(ms, contentType);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // todo log exception
                }
            }

            return Redirect($"https://placehold.it/{ThumbSize}?text={Url.Encode(file.Extension)}");
        }
        public void MoveFile(string FilePath, string NewFilePath)
        {
            var O = Path.Combine(HttpRuntime.AppDomainAppPath, "Repository/" + FilePath);
            var N = Path.Combine(HttpRuntime.AppDomainAppPath, "Repository/" + NewFilePath);
            //var O = Server.MapPath("~/Repository/" + FilePath);
            //var N = Server.MapPath("~/Repository/" + NewFilePath);
            System.IO.File.Move(O, N);
        }
        public IEnumerable<FileActions> Files(string FolderPath, IEnumerable<string> names = null)
        {
            var folder = GetDirecotry(FolderPath);

            var files = from file in folder.GetFiles()
                        where names == null || names.Contains(file.Name, StringComparer.OrdinalIgnoreCase)
                        select new FileActions
                        {
                            deleteType = "POST",
                            name = file.Name,
                            size = file.Length,
                            url = "/FileHandling/GetFile?Name=" + file.Name + "&FolderPath=" + FolderPath,
                            thumbnailUrl = "/FileHandling/GetFile?Name=" + file.Name + "&FolderPath=" + FolderPath + "&thumbnail=" + true,
                            //deleteUrl = "/FileHandling/DeleteFile?Name=" + file.Name + "&FolderPath=" + FolderPath,
                            deleteUrl = "",
                            downUrl = "/FileHandling/DownloadUrl?Name=" + file.Name + "&FolderPath=" + FolderPath,
                            path =  FolderPath,

                        };

            return files;
        }
    }

}