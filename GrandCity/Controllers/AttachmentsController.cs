using MeherEstateDevelopers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeherEstateDevelopers.Models;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]

    public class AttachmentsController : Controller
    {
        // GET: Attachments

        private readonly Grand_CityEntities db = new Grand_CityEntities();
        public string Module { get; set; }
        public string Type { get; set; }
        public string Filename { get; set; }
        public string Path { get; set; }
        public int Reference_Id{ get; set; }
        public AttachmentsController(Modules module, Types type, string filename, string Path, int Ref_Id)
        {
            this.Module = module.ToString();
            this.Type = type.ToString();
            this.Filename = filename;
            this.Path = Path;
            this.Reference_Id = Ref_Id;
        }
        public void UploadAttachment(HttpContextBase File)
        {

        }
    }
}