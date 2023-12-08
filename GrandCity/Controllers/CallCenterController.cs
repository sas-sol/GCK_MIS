//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using MeherEstateDevelopers.Models;
//using Microsoft.AspNet.Identity;
//using MeherEstateDevelopers.Filters;

//namespace MeherEstateDevelopers.Controllers
//{
//    [Authorize]
//    [LogAction]
//    public class CallCenterController : Controller
//    {
//        // GET: CallCenter
//        private Grand_CityEntities db = new Grand_CityEntities();

//        public ActionResult OverdueInstallments()
//        {
//            var res = db.Sp_Get_RegisterFileDueAmount().ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Over Due Installments Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

//            return View(res);
//        }
//        public ActionResult GetFileFollowupData(long Id)
//        {
//            var res = db.Sp_Get_Followups(Id).ToList();

//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed File Follow UP's Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Id);
//            return PartialView(res);
//        }
//        [HttpPost]
//        public JsonResult AddFollowup(long File_Plot_Number,string Message, string Owner_Name)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
             
//            db.Sp_Add_Activity(userid, "Added New Follow Up on File " + File_Plot_Number + " " + Message , "Create", "Activity_Record", ActivityType.Call_Center.ToString(), userid);
//            int index = 0;
//            List<FollowupData> fd = new List<FollowupData>();
//            foreach (string file in Request.Files)
//            {
//                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
//                index++;
//                if (hpf.ContentLength == 0)
//                    continue;
//                var imageName = Path.GetExtension(hpf.FileName);
//                if (!Directory.Exists(Server.MapPath("~/Followups/" + File_Plot_Number + "")))
//                {
//                    Directory.CreateDirectory(Server.MapPath("~/Followups/" + File_Plot_Number + ""));
//                }
//                string savedFileName = Path.Combine(Server.MapPath("~/Followups/" + File_Plot_Number + "/"), hpf.FileName);
//                hpf.SaveAs(savedFileName);
//                db.Sp_Add_Followup(File_Plot_Number, hpf.FileName, Owner_Name, userid,"Img");
//                FollowupData f = new FollowupData() {
//                    File_plot_number = File_Plot_Number,
//                    Msg = hpf.FileName,
//                    Name = Owner_Name,
//                    Userid = userid,
//                    Type = "Img"
//                };
//                fd.Add(f);
//            }
//            if (!string.IsNullOrEmpty(Message))
//            {
//                db.Sp_Add_Followup(File_Plot_Number, Message, Owner_Name, userid,"Text");
//                FollowupData f = new FollowupData()
//                {
//                    File_plot_number = File_Plot_Number,
//                    Msg = Message,
//                    Name = Owner_Name,
//                    Userid = userid,
//                    Type = "Text"
//                };
//                fd.Add(f);
//            }
//            return Json(fd);
//        }
//    }
//}