using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeherEstateDevelopers.Models;
using System.IO;
using Microsoft.AspNet.Identity;
using MeherEstateDevelopers.Filters;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class RentalController : Controller
    {
        // GET: Rental
        private Grand_CityEntities db = new Grand_CityEntities();
        public ActionResult Index()
        {
            var res = db.Sp_Get_PlotRentals().ToList();
            return View(res);
        }
        public ActionResult AddRental()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Access Page of Rental", "Update", Modules.PlotManagement.ToString(), ActivityType.Page_Access.ToString(), null);
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            return View();
        }
        public ActionResult AddRentalDetails(long Plot_Id)
        {
            var res1 = db.Sp_Get_PlotData(Plot_Id).FirstOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(Plot_Id).ToList();
            var res = new LatestPlotDetailData
            {
                PlotData = res1,
                PlotOwners = res2
            };
            ViewBag.TransactionId = new Helpers().RandomNumber();
            ViewBag.Plot = Plot_Id;
            return PartialView(res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddRentalDetail(Plot_Rental r)
        {
            if(r is null)
            {
                return Json(null);
            }
            bool mult = db.Plot_Rental.Any(x => x.TransactionId == r.Id);
            if(mult)
            {
                return Json(null);
            }
            string temp = string.Empty;
            var uid = User.Identity.GetUserId<long>();
            var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
            r.CreateBy = uid;
            r.CreatedByName = uname;
            r.Rental_DateTime = DateTime.Now;
            r.Serial_No = "REN-" + db.Sp_Get_ReceiptNo("Plot Rentals").FirstOrDefault();
            if (!string.IsNullOrEmpty(r.Img))
            {
                temp = r.Img;
                r.Img = string.Empty;
            }
            db.Plot_Rental.Add(r);
            long d = db.SaveChanges();
            db.Sp_Add_PlotComments(r.Plot_Id, r.Floor + " is rented to: " + r.Name , uid, ActivityType.Rental.ToString());
            var renid = db.Plot_Rental.Where(x => x.TransactionId == r.TransactionId).Select(x=>x.Id).FirstOrDefault();
            var filePathOriginal = Server.MapPath("/Repository/Rental Images/");
            if(!Directory.Exists(filePathOriginal))
            {
                Directory.CreateDirectory(filePathOriginal);
            }
            new Helpers().SaveBase64Image(temp, filePathOriginal + "/" + renid.ToString() + ".png", string.Empty);
            //if (renteePic != null)
            //{
            //    var filename = renteePic.FileName;
            //    var filePathOriginal = Server.MapPath("/Repository/Rental Images/");
            //    if (!Directory.Exists(filePathOriginal))
            //    {
            //        Directory.CreateDirectory(filePathOriginal);
            //    }
            //    string[] prsed = renteePic.FileName.Split('.');
            //    string savedFileName = Path.Combine(filePathOriginal, r.Id.ToString() + "." + prsed[prsed.Length - 1]);
            //    renteePic.SaveAs(savedFileName);
            //}
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Added New Details in Plot Rental " + r.Serial_No, "Create", "Activity_Record", ActivityType.Rental.ToString(), d);
            return Json(r.Id);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public JsonResult UpdateRentalDetail(Plot_Rental r)
        //{
        //    if (r is null)
        //    {
        //        return Json(null);
        //    }
        //    var ren = db.Plot_Rental.Where(x => x.TransactionId == r.TransactionId).FirstOrDefault();
             
        //    string temp = string.Empty;
        //    var uid = User.Identity.GetUserId<long>();
        //    var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
        //    r.CreateBy = uid;
        //    r.CreatedByName = uname;
        //    r.Rental_DateTime = DateTime.Now;
        //    r.Serial_No = ren.Serial_No;
        //    if (!string.IsNullOrEmpty(r.Img))
        //    {
        //        temp = r.Img;
        //        r.Img = string.Empty;
        //    }
        //    db.Sp_Update_Rental(r.Name,r.Father_Husband,r.CNIC_NICOP,r.Mobile_1,r.Mobile_2,r.Phone_Office,r.Email,r.Postal_Address,r.Residential_Address,r.Occupation,r.Nationality,r.Date_Of_Birth,r.Rental_DateTime.ToString(),ren.Rental_Close_DateTime.ToString(), r.Previous_Rental_Info,r.Current_Rental_Info,r.Residence_Time,r.Qualification,r.Total_Family_Memebers,r.Gents,r.Ladies,r.Children,r.Car_Arms_Passport_No,r.Floor,r.Plot_Id, r.CreateBy, ren.TransactionId);
        
        //    db.Sp_Add_PlotComments(r.Plot_Id, r.Floor + " Rental is Edited And Rented To : " + r.Name , uid, ActivityType.Rental.ToString());
        //    var filePathOriginal = Server.MapPath("/Repository/Rental Images/");
        //    if (!Directory.Exists(filePathOriginal))
        //    {
        //        Directory.CreateDirectory(filePathOriginal);
        //    }
        //    new Helpers().SaveBase64Image(temp, filePathOriginal + "/" + ren.Id.ToString() + ".png", string.Empty);
        //    //if (renteePic != null)
        //    //{
        //    //    var filename = renteePic.FileName;
        //    //    var filePathOriginal = Server.MapPath("/Repository/Rental Images/");
        //    //    if (!Directory.Exists(filePathOriginal))
        //    //    {
        //    //        Directory.CreateDirectory(filePathOriginal);
        //    //    }
        //    //    string[] prsed = renteePic.FileName.Split('.');
        //    //    string savedFileName = Path.Combine(filePathOriginal, r.Id.ToString() + "." + prsed[prsed.Length - 1]);
        //    //    renteePic.SaveAs(savedFileName);
        //    //}
        //    long userid = long.Parse(User.Identity.GetUserId());
        //    db.Sp_Add_Activity(userid, "Updated Details in Plot Rental " + r.Serial_No, "Update", "Activity_Record", ActivityType.Rental.ToString(), userid);
        //    return Json(ren.Id);
        //}
        public ActionResult RentalInfo(long id)
        {
            var res = db.Plot_Rental.Where(x => x.Id == id).FirstOrDefault();
            var pltOwner = db.Sp_Get_PlotLastOwner(res.Plot_Id).ToList();
            return View(new RentalInfoView { PlotOwners = pltOwner, RentalDetails = res });
        }
        public ActionResult EditRental(long id)
        {
            var res = db.Plot_Rental.Where(x => x.Id == id).FirstOrDefault();
            var res1 = db.Sp_Get_PlotData(res.Plot_Id).FirstOrDefault();
            var res2 = db.Sp_Get_PlotLastOwner(res.Plot_Id).ToList();

            ViewBag.TransactionId = res.TransactionId;
            ViewBag.Plot = res.Plot_Id;
            ViewBag.id = res.Id;
            return PartialView(new RentalInfoViewEdit { PlotOwners = res2, RentalDetails = res,PlotData = res1 });
           
        }
        public ActionResult Rental_NOC(long Plot_Id)
        {
            return View();
        }
        public ActionResult Rental_List()
        {
            return View();
        }
        public ActionResult RentalDetails()
        {
            return PartialView();
        }
        public ActionResult RentalNOC(long id)
        {
            var res = db.Plot_Rental.Where(x => x.Id == id).FirstOrDefault();
            var uid = User.Identity.GetUserId<long>();
            if(res.Rental_Close_DateTime is null)
            {
                res.Rental_Close_DateTime = DateTime.Now;
                db.Plot_Rental.Attach(res);
                db.Entry(res).Property(x => x.Rental_Close_DateTime).IsModified = true;
                var plt = db.Plots.Where(x => x.Id == res.Plot_Id).FirstOrDefault();
                plt.Rental = true;

                db.Plots.Attach(plt);
                db.Entry(plt).Property(x => x.Remarks).IsModified = true;
                db.SaveChanges();
                db.Sp_Add_PlotComments(res.Plot_Id, "Generated Rental NOC. of" + res.Name, uid, ActivityType.Rental.ToString());
            }
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Generated Rental NOC. of" + res.Name, "Create", "Activity_Record", ActivityType.Rental.ToString(), userid);
            return View(res);
        }

        public JsonResult RemoveRental(long id)
        {
            var res = db.Plot_Rental.Where(x => x.Id == id).FirstOrDefault();
            
            if(res is null)
            {
                return Json(false);
            }

            var uid = User.Identity.GetUserId<long>();
            if (res.Rental_Close_DateTime != null)
            {
                var plt = db.Plots.Where(x => x.Id == res.Plot_Id).FirstOrDefault();
                plt.Remarks = string.Empty;

                db.Plots.Attach(plt);
                db.Entry(plt).Property(x => x.Remarks).IsModified = true;
                db.SaveChanges();
                db.Sp_Add_PlotComments(res.Plot_Id, "Remove Rental status of property.", uid, ActivityType.Rental.ToString());
            }
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Remove Rental status of property. " , "Delete", "Activity_Record", ActivityType.Rental.ToString(), res.Plot_Id);

            return Json(false);
        }
    }
}