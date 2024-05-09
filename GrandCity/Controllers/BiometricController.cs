using MeherEstateDevelopers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using static MeherEstateDevelopers.MvcApplication;

namespace MeherEstateDevelopers.Controllers
{
    public class BiometricController : Controller
    {
        private Grand_CityEntities db = new Grand_CityEntities();
        // GET: Biometric
        [NoDirectAccess] public ActionResult BiometricVerification(long Id, string module)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Biometric Verification Page For  " + module, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
      
            if (module == "PlotManagement")
            {
                //var res = db.Sp_Get_PlotLastOwner()
                return PartialView();
            }
            else if (module == "FileManagement")
            {
                var res = db.Sp_Get_FileLastOwner(Id).Select(x => new BiometricVerification { CNIC_Uparsed = x.CNIC_NICOP, Id = x.Id, isMatched = false, Name = x.Name }).ToList();
                res.ForEach(x => x.CNIC = GetCNIC_As_Number(x.CNIC_Uparsed));
                foreach (var v in res)
                {
                    var found = db.Owners_FingerPrints.Where(x => x.OwnerId == v.CNIC).FirstOrDefault();
                    
                    v.SavedTemplate = found.Fp_PrintCode;
                }
                return PartialView(res);
            }

            return PartialView();
        }

        public JsonResult FingerPrintUpdater(string img, string patt, int ftype, long ownId)
        {
            try
            {
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                if(db.Owners_FingerPrints.Any(x=>x.OwnerId == ownId))
                {
                    return Json(new { Status = false, Msg = "Biometric record of this owner already exists. Cannot overwrite the record." });
                }
                Owners_FingerPrints fp = new Owners_FingerPrints
                {
                    Added_ByName = uname,
                    AddedOn_Date = DateTime.Now,
                    AddedBy_Id = uid,
                    OwnerId = ownId,
                    Fp_PrintCode = patt,
                    Finger_Num = ftype,
                    Fp_Image = img
                };

                db.Owners_FingerPrints.Add(fp);
                db.SaveChanges();
                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "UPdated Biometric Data" , "Read", "Activity_Record", ActivityType.BioMetric.ToString(), ownId);

                //string pth = Server.MapPath("/Repository/Plot Owners Finger Prints/");
                //if (!Directory.Exists(pth))
                //{
                //    Directory.CreateDirectory(pth);
                //}
                //new Helpers().SaveBase64Image(img, pth + "/" + ownId.ToString() + "_" + ftype.ToString() + ".png", string.Empty);
                return Json(new { Status = true, Msg = "Biometric record of owner has been saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Msg = "Failed to save biometric record of this owner." });
            }
        }

        [NoDirectAccess] public ActionResult BioAuthSetting(long Id, string module, string tp)
        {
            ViewBag.modId = Id;
            ViewBag.modTyp = module;
            ViewBag.Typ = tp;
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Biometric Auth Settings Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

            return View();
        }

        [NoDirectAccess] public ActionResult FingerPrint(long Id, string module, string tp)
        {
            if (module == "FileManagement")
            {
                if(string.IsNullOrEmpty(tp))
                {
                    var res = db.Sp_Get_FileLastOwner(Id).Select(x => new BiometricOwnerDetails { CNIC_Uparsed = x.CNIC_NICOP, Id = x.Id, Name = x.Name }).ToList();
                    res.ForEach(x => x.CNIC = GetCNIC_As_Number(x.CNIC_Uparsed));
                    return PartialView(res);
                }
                else
                {
                    var res = db.Files_Transfer_Request.Where(x=>x.Group_Tag == Id).Select(x => new BiometricOwnerDetails { CNIC_Uparsed = x.CNIC_NICOP, Id = x.Id, Name = x.Name }).ToList();
                    res.ForEach(x => x.CNIC = GetCNIC_As_Number(x.CNIC_Uparsed));
                    return PartialView(res);
                }
            }
            else if (module == "PlotManagement")
            {
                if(string.IsNullOrEmpty(tp))
                {
                    var res = db.Sp_Get_PlotLastOwner(Id).Select(x => new BiometricOwnerDetails { CNIC_Uparsed = x.CNIC_NICOP, Id = x.Id, Name = x.Name }).ToList();
                    res.ForEach(x => x.CNIC = GetCNIC_As_Number(x.CNIC_Uparsed));
                    return PartialView(res);
                }
                else
                {
                    var res = db.Plots_Transfer_Request.Where(x=>x.GroupTag == Id).Select(x => new BiometricOwnerDetails { CNIC_Uparsed = x.CNIC_NICOP, Id = x.Id, Name = x.Name }).ToList();
                    res.ForEach(x => x.CNIC = GetCNIC_As_Number(x.CNIC_Uparsed));
                    return PartialView(res);
                }
            }

            return PartialView();
        }

        private long GetCNIC_As_Number(string cnic)
        {
            string[] s_parsed = cnic.Split('-');
            string newS = "";
            foreach (var v in s_parsed)
            {
                newS += v;
            }

            return Convert.ToInt64(newS);
        }
    }
}