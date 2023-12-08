using MeherEstateDevelopers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using System.IO;
using Newtonsoft.Json;
using System.Web.Http.Results;
using System.Security.Cryptography;
using System.Text;

namespace MeherEstateDevelopers.Controllers
{
    [LogAction]
    [Authorize]
    public class HomeController : Controller
    {
        private Grand_CityEntities db = new Grand_CityEntities();
        public ActionResult Index()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var change = db.Users.Find(userid).FirstChangePassword;
            if (User.IsInRole("Administrator"))
            {
                if (change != true)
                    return RedirectToAction("ChangePassword", "Account");
                else
                    return RedirectToAction("Administrator");
            }
            else if (User.IsInRole("StaffAdmin"))
            {
                if (change != true)
                    return RedirectToAction("ChangePassword", "Account");
                else
                    return RedirectToAction("StaffAdmin");
            }
            else if (User.IsInRole("Staff"))
            {
                if (change != true)
                    return RedirectToAction("ChangePassword", "Account");
                else
                    return RedirectToAction("Staff");
            }
            else if (User.IsInRole("Guest"))
            {
                return RedirectToAction("Guest");
            }
            else
            {
                return View();
            }
        }
        public ActionResult AllUsers()
        {
            var res = db.Sp_Get_Users().ToList();
            return View(res);
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [Authorize(Roles = "Administrator")]
        public ActionResult Administrator()
        {
            ViewBag.Blocks = new SelectList(db.RealEstate_Blocks.ToList(), "Id", "Block_Name");
            var res = long.Parse(User.Identity.GetUserId());
            return View();
        }
        [Authorize(Roles = "CEO")]
        public ActionResult CEO()
        {
            return View();
        }
        [Authorize(Roles = "CEO")]
        public ActionResult CEOHive()
        {
            return View();
        }
        [Authorize(Roles = "Staff")]
        public ActionResult Staff()
        {
            return View();
        }

        [Authorize(Roles = "StaffAdmin")]
        public ActionResult StaffAdmin()
        {
            var userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_UserGroup(userid).ToList();
            return View(res);
        }
        [Authorize(Roles = "Guest")]
        public ActionResult Guest()
        {
            ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Department.ToString()), "Id", "Name");
            return View();
        }
        [Authorize(Roles = "Administrator")]
        public ActionResult DashTiles(string Type)
        {
            ViewBag.Dash = Type;
            // To Cache Image Tiles
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetExpires(DateTime.Now.AddHours(1));
            Response.Cache.SetMaxAge(TimeSpan.FromHours(1));
            Response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");

            return PartialView();
        }
        public ActionResult StaffDashTiles(string Type)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);

            ViewBag.Dash = Type;
            ViewBag.Comp_Id = comp.Id;


            return PartialView();
        }

        public ActionResult CEODashTiles(string Type)
        {
            ViewBag.Dash = Type;
            return PartialView();
        }

        public ActionResult EmployeeProfileImage(string empCode)
        {
            ViewBag.empId = empCode;

            string UserImagePath = "/Repository/SAG Employees Data/SAG Employees Images/";
            string path = Path.Combine(UserImagePath, empCode + ".jpg");
            if (!System.IO.File.Exists(HttpContext.Server.MapPath(path)))
            {
                path = Path.Combine(UserImagePath, "nophoto.gif");
            }
            ViewBag.dp_path = path;
            ViewBag.code = empCode;
            return PartialView();
        }

        public ActionResult GetUserImage(string Id, string Ref)
        {
            ViewBag.Id = Id;
            ViewBag.ImgDiv = Ref;
            return PartialView();
        }

        [HttpPost]
        public JsonResult SaveProfileImage(string empCode)
        {
            try
            {
                if(string.IsNullOrEmpty(empCode) || string.IsNullOrWhiteSpace(empCode))
                {
                    return Json(false);
                }
                string UserImagePath = "/Repository/SAG Employees Data/SAG Employees Images/";
                string path = Path.Combine(UserImagePath, empCode + ".jpg");
                int index = 0;
                if (System.IO.File.Exists(HttpContext.Server.MapPath(path)))
                {
                    System.IO.File.Delete(HttpContext.Server.MapPath(path));
                }
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
                    index++;
                    if (hpf.ContentLength == 0)
                        continue;
                    var imageName = Path.GetExtension(hpf.FileName);
                    hpf.SaveAs(HttpContext.Server.MapPath(path));
                }
                return Json(true);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }
        [Authorize(Roles = "Audit Dashboard, Administrator")]
        public ActionResult AuditorsHome()
        {

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Auditours Report ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            var res = db.Sp_Get_PlotsShortSummary().SingleOrDefault();
            var res1 = db.Sp_Get_FilesShortSummary().SingleOrDefault();
            var res2 = db.Sp_Get_OverDueFilesReport().FirstOrDefault();
            var res3 = (from x in db.File_Form
                       where x.Block == "Sher Alam"
                       group x by new { x.Status } into g
                       select new Stat{ Status = g.Key.Status ,Total= g.Count() }).ToList();
            SherAlamStats a = new SherAlamStats() { 
                Cancelled_Application = res3.Where(x=> x.Status == 0).Select(x=> x.Total).FirstOrDefault(),
                Pending = res3.Where(x => x.Status == 2).Select(x => x.Total).FirstOrDefault(),
                Registered = res3.Where(x => x.Status == 1).Select(x => x.Total).FirstOrDefault(),
                Temporary_Cancelled = res3.Where(x => x.Status == 6).Select(x => x.Total).FirstOrDefault(),
            };

            var result = new AuditorHome_Report { Summary = res, FilesSummary = res1, OverDueFiles = res2, SherAlam = a };
            return PartialView(result);
        }
        // Department MIS Users 
        public JsonResult DepMisUser(int DepId)
        {
            var res = db.Sp_Get_DepMISUsers(DepId).ToList();
            return Json(res);
        }
     
        public ActionResult RequestsView()
        {
            var uid = User.Identity.GetUserId<long>();
            var res = db.Sp_Get_MIS_Requests(uid, null).ToList();
            var deps = db.Sp_Get_TimeAdjust_Manager(uid).ToList();
            foreach (var v in res)
            {
                if (v.ModuleType == "Plot_Discount")
                {
                    v.RequestDetails = JsonConvert.DeserializeObject<PlotDiscountRequest>(v.Details);
                    var plt = db.Plots.Where(x => x.Id == v.ModuleId).Select(x => new { x.Plot_Number, x.Block_Name }).FirstOrDefault();
                    v.RequestDetails.PlotNo = plt.Plot_Number + "  " + plt.Block_Name;
                }
                if (v.ModuleType == "Loan Waiver")
                {
                    v.WaiverDetails = JsonConvert.DeserializeObject<LoanWaiverRequest>(v.Details);
                }
                if (v.ModuleType == "Departmental_Time_Adjustment")
                {
                    var timereq = JsonConvert.DeserializeObject<DepartmentalTimeAdjustmentRequestModel>(v.Details);
                    if (deps.Contains(Convert.ToInt32(timereq.DepId)))
                        v.TimeAdjustmentDetail = JsonConvert.DeserializeObject<DepartmentalTimeAdjustmentRequestModel>(v.Details);
                }
                else if (v.ModuleType == "PlotSwap")
                {
                    v.PlotSwapsRequests = JsonConvert.DeserializeObject<SwapPlotRequest>(v.Details);
                }
                else if ( v.ModuleType == "PlotAdjustment")
                {
                    v.PlotAdjRequests = JsonConvert.DeserializeObject<AdjPlotRequest>(v.Details);
                }
                else if (v.ModuleType == "Reinstate_With_Fine")
                {
                    v.FileReinstate = JsonConvert.DeserializeObject<FileReinstateRequest>(v.Details);
                }
            }
            return PartialView(res);
        }

        public ActionResult AllRequests()
        {

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed MIS Requests ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View();
        }
        
        public JsonResult ApproveRequest(long req)
        {
            var uid = User.Identity.GetUserId<long>();
            var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
            var reqData = db.MIS_Requests.Where(x => x.Id == req).FirstOrDefault();
            PlotDiscountRequest pdr = JsonConvert.DeserializeObject<PlotDiscountRequest>(reqData.Details);
            Discount d = new Discount
            {
                Module_Id = (long)reqData.ModuleId,
                Module = Modules.PlotManagement.ToString(),
                Discount_Amount = pdr.DiscountAmount,
                Total_Amount = pdr.DiscountAmount,
                Remarks = pdr.DescriptionText
            };
            db.Discounts.Add(d);
            db.SaveChanges();
            reqData.Status = "Approved";
            pdr.StatusChangedAt = DateTime.Now;
            pdr.Manager_Id = uid;
            pdr.ManagerName = uname;
            reqData.Details = JsonConvert.SerializeObject(pdr);
            db.MIS_Requests.Attach(reqData);
            db.Entry(reqData).Property(x => x.Status).IsModified = true;
            db.Entry(reqData).Property(x => x.Details).IsModified = true;
            db.SaveChanges();
            return Json(true);
        }
        
        public JsonResult RejectRequest(long req)
        {
            var uid = User.Identity.GetUserId<long>();
            var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
            var reqData = db.MIS_Requests.Where(x => x.Id == req).FirstOrDefault();
            PlotDiscountRequest pdr = JsonConvert.DeserializeObject<PlotDiscountRequest>(reqData.Details);
            pdr.Manager_Id = uid;
            pdr.ManagerName = uname;
            reqData.Status = "Rejected";
            reqData.Details = JsonConvert.SerializeObject(pdr);
            db.MIS_Requests.Attach(reqData);
            db.Entry(reqData).Property(x => x.Status).IsModified = true;
            db.Entry(reqData).Property(x => x.Details).IsModified = true;
            db.SaveChanges();
            return Json(true);
        }

        public JsonResult DeleteRequest(long req)
        {
            var uid = User.Identity.GetUserId<long>();
            var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
            var reqData = db.MIS_Requests.Where(x => x.Id == req).FirstOrDefault();
            PlotDiscountRequest pdr = JsonConvert.DeserializeObject<PlotDiscountRequest>(reqData.Details);
            pdr.Manager_Id = uid;
            pdr.ManagerName = uname;
            reqData.Status = "Request_Deleted";
            reqData.Details = JsonConvert.SerializeObject(pdr);
            db.MIS_Requests.Attach(reqData);
            db.Entry(reqData).Property(x => x.Status).IsModified = true;
            db.Entry(reqData).Property(x => x.Details).IsModified = true;
            db.SaveChanges();
            return Json(true);
        }
        
        //public ActionResult blockWiseCollectionResult(long? Id, string propertyType)
        //{
        //    ViewBag.blockName = db.RealEstate_Blocks.Where(x => x.Id == Id).Select(x => x.Block_Name).FirstOrDefault();
        //    var res = db.Sp_Get_SaleReport(Id, propertyType).ToList();
        //    return View(res);
        //}

        public ActionResult AllCustomersPortal()
        {
            var res = db.Sp_Get_AllCustomer_Portal().ToList();
            return PartialView(res);
        }
        
        public JsonResult GetPassword(string Password)
        {
            var password = this.Deccrypt(Password);
            return Json(password);
        }

        public string Deccrypt(string cipherText)
        {
            string EncryptionKey = "abc123";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {

                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        
        public JsonResult DeleteUser(string email, string cnic, string phone)
        {
            var res = db.Sp_Delete_CustomerUser(email, cnic, phone);
            return Json(res);
        }
    }
}