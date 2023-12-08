using MeherEstateDevelopers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
namespace MeherEstateDevelopers.Controllers
{
    [LogAction]
    [Authorize]
    public class ActivityController : Controller
    {
        Grand_CityEntities db = new Grand_CityEntities();
        // GET: Activity
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MyActivity()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            List<ActivitiesList> res = new List<ActivitiesList>();
            var res1 = db.Sp_Get_MyActivities_List(null, null, userid).Select(x => new ActivitiesList
            {
                Activity_Type = x.Activity_Type,
                DateTime = x.DateTime,
                Description = x.Description ,
                Name = x.Name,
                Module = x.Activity
            }).ToList();
            res.AddRange(res1); 
            db.Sp_Add_Activity(userid, "View My Activity Page ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

            return PartialView(res);
        }

        //public ActionResult ManagerLevlActivityListshow(DateTime? fromd, DateTime? tdate)

        //{
        //    List<ActivitiesList> res = new List<ActivitiesList>();
        //    var res1 = db.Sp_Get_Activities_List(fromd, tdate).Select(x => new ActivitiesList
        //    {
        //        Activity_Type = x.Activity_Type,
        //        DateTime = x.DateTime,
        //        Description = x.Description + " " + x.Plot_No,
        //        Name = x.Name,
        //        Activity = x.Activity,
        //        Module = "Plots"
        //    }).ToList();
        //    res.AddRange(res1);
        //    var res2 = db.SP_Get_Files_Log(fromd, tdate).Select(x => new ActivitiesList
        //    {
        //        Activity_Type = x.Type,
        //        DateTime = x.Datetime,
        //        Description = x.ExtraInformation + " " + x.FileFormNumber,
        //        Name = x.Name,
        //        Activity = x.CRUD,
        //        Module = "Files"
        //    }).ToList();
        //    res.AddRange(res2);
        //    return View(res);
        //}
        //public ActionResult SearchActivitReport(DateTime? From, DateTime? To)
        //{

        //    List<ActivitiesList> res = new List<ActivitiesList>();
        //    var res1 = db.Sp_Get_Activities_List(From, To).Select(x => new ActivitiesList
        //    {
        //        Activity_Type = x.Activity_Type,
        //        DateTime = x.DateTime,
        //        Description = x.Description + " " + x.Plot_No,
        //        Name = x.Name,
        //        Module = "Plots"
        //    }).ToList();
        //    res.AddRange(res1);
        //    var res2 = db.SP_Get_Files_Log(From, To).Select(x => new ActivitiesList
        //    {
        //        Activity_Type = x.Type,
        //        DateTime = x.Datetime,
        //        Description = x.ExtraInformation + " " + x.FileFormNumber,
        //        Name = x.Name,
        //        Module = "Files"
        //    }).ToList();
        //    res.AddRange(res2);
        //    return PartialView(res);
        //}
        public ActionResult SearchMyActivitReport(DateTime? From, DateTime? To)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            List<ActivitiesList> res = new List<ActivitiesList>();
            var res1 = db.Sp_Get_MyActivities_List(From, To, userid).Select(x => new ActivitiesList
            {
                Activity_Type = x.Activity_Type,
                DateTime = x.DateTime,
                Description = x.Description + " " + x.Plot_No,
                Name = x.Name,
                Module = x.Activity
            }).ToList();
            res.AddRange(res1);
           
            return PartialView(res);
        }
        public void Comments()
        {
            var res = db.Plots_Comments.Where(x => x.Comment.Contains("Mark Signed Allotment Letter with id")).ToList();
            foreach (var item in res)
            {
                var c = item.Comment.Replace("Mark Signed Allotment Letter with id:"," ").Trim();
                var a = Convert.ToInt64(c);
                    var b = db.AllotmentLetters.SingleOrDefault(x=> x.Id == a);
                item.Comment = "Marked Allotment Letter signed of Name: " + b.Name;
                item.Plot_Id = b.Plot_Id;
                db.SaveChanges();
            }
        }

        public void acti()
        {
            var res = db.Activities.Where(x => x.Description.Contains("Mark Signed Allotment Letter with id")).ToList();
            foreach (var item in res)
            {
                var c = item.Description.Replace("Mark Signed Allotment Letter with id:", " ").Trim();
                var a = Convert.ToInt64(c);
                var b = db.AllotmentLetters.SingleOrDefault(x => x.Id == a);
                item.Plot_Id = b.Plot_Id;
                db.SaveChanges();
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}