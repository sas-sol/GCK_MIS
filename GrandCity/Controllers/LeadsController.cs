using MeherEstateDevelopers.Filters;
using System;
using System.Linq;
using System.Web.Mvc;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using static MeherEstateDevelopers.MvcApplication;

namespace MeherEstateDevelopers.Controllers
{
    [LogAction]
    [Authorize]
    public class LeadsController : Controller
    {
        Grand_CityEntities db = new Grand_CityEntities();
        [NoDirectAccess] public ActionResult CreateLeads()
        {
            long userid = User.Identity.GetUserId<long>();
            db.Sp_Add_Activity(userid, "Accessed Create Lead Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            ViewBag.Block = new SelectList(db.Sp_Get_Block().ToList(), "Block_Name", "Block_Name");
            ViewBag.Project = new SelectList(db.RealEstate_Projects.Where(x => x.Type == ProjectCategory.Building.ToString() || x.Type == ProjectCategory.General.ToString()).ToList(), "Project_Name", "Project_Name");

            return PartialView();
        }
        
        public JsonResult CreateDealNumberForDeals()
        {
            var res = db.Property_Deal.Where(x => x.DealNumber == null).ToList();
            foreach (var v in res)
            {
                db.Sp_Update_SAM_Deals_DealNumber(v.Id);
            }
            return Json(true);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateLeads(Lead L, string Block, string Description)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Created New Lead With description " + Description + " For " + Block + " Block", "Create", "Activity_Record", ActivityType.Lead.ToString(), userid);

            var res2 = db.Sp_Add_Lead(L.Address, userid, Block, L.Mobile_1, L.Name, L.Father_Husband, L.Offered_Price, L.Source, L.Plot_Size, L.LeadStatus, L.Project, null).FirstOrDefault();

            var res3 = db.Sp_Add_LeadFollowup(Description, res2, userid, "Text");
            return Json(new Return { Status = true, Msg = "Lead created successfully" });

        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreatePremiumLeads(Lead L, string Description)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var project = "SA Premium Homes";
            var res = db.Sp_Add_Lead(L.Address, userid, null, L.Mobile_1, L.Name, L.Father_Husband, null, L.Source, null, L.LeadStatus, project, null).FirstOrDefault();
            var res3 = db.Sp_Add_LeadFollowup(Description, res, null, "Text");
            return Json(true);
        }

        [NoDirectAccess] public ActionResult CompanyLeads(string Company, DateTime? From, DateTime? To)
        {
            long userid = User.Identity.GetUserId<long>();
            db.Sp_Add_Activity(userid, "Accessed " + Company + " Company Lead Page ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

            if (Company == "")
            {
                Company = null;
            }
            var res = db.Sp_Get_Leads_Search(From, To).ToList();
            return PartialView(res);
        }
        
        [NoDirectAccess] public ActionResult SAMLead()
        {
            ViewBag.Project = "Grand City";
            var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Sales Executive")).ToList();
            ViewBag.LeadsUser = new SelectList(All, "Id", "Name");
            long userid = User.Identity.GetUserId<long>();
            db.Sp_Add_Activity(userid, "Accessed SAM Leads Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return View();
        }
        
        [NoDirectAccess] public ActionResult SAMSearch()
        {
            long userid = User.Identity.GetUserId<long>();
            return PartialView();
        }
        
        [NoDirectAccess] public ActionResult MyLeadSearch()
        {
            long userid = User.Identity.GetUserId<long>();
            return PartialView();
        }
        
        [NoDirectAccess] public ActionResult MyLeads()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            //var res = db.Sp_Get_Lead_User(userid).ToList();
            var res = db.Sp_Get_Lead_Search_Comp(userid, null, null, null).ToList();

            db.Sp_Add_Activity(userid, "Accessed My Leads ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            ViewBag.user = userid;
          
            var namee = db.Users.Where(x => x.Id == userid).Select(x => x.Name).FirstOrDefault();
            ViewBag.namee = namee;
            return View(res);
        }
        
        [NoDirectAccess] public ActionResult LeadDetails(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_Lead(Id).SingleOrDefault();
            if (res1 == null)
            {
                return RedirectToAction("MyLeads");
            }
            db.Sp_Add_Activity(userid, "Accessed Leads Details Page of Lead " + res1.Name, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Id);
            var res4 = Enum.GetValues(typeof(LeadsStatus)).Cast<LeadsStatus>().Select(e => new { Value = e, Text = e.ToString().Replace("_", " ") }).ToList();
            ViewBag.LeadStatus = new SelectList(res4, "Value", "Text", res1.LeadStatus);
            var res7 = db.SAM_Receipts.Where(x => x.Lead_Id == Id && x.Cancel == null).ToList();
            var res8 = db.SAM_Voucher.Where(x => x.Lead_Id == Id && x.VoucherNo != null && x.Cancel == null).ToList();
            var res = new LeadsData { Lead = res1, Receipts = res7, Vouchers = res8 };
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Block_Name", "Block_Name");
            ViewBag.Floors = new SelectList(db.Commercial_FloorsPlan.Where(x => x.Project_Id == db.RealEstate_Projects.Where(y => y.Project_Name == res1.Project).Select(y => y.Id).FirstOrDefault()).ToList(), "Floor", "Floor");
            return View(res);
        }
        
        public JsonResult UpdateLeads(decimal? PrevaskPrice, decimal? PreOffPrice, string PreLeadStatus, Lead L,string Block, string Plot_Size, string Project, string Floors)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            
            db.Sp_Add_Activity(userid, "Updated Lead " + PreLeadStatus, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), L.Id);

            if (Project == "Grand City")
            {
                db.Sp_Update_Lead(L.Id, L.Name, L.Father_Husband, L.Mobile_1, L.Address, L.Offered_Price, L.LeadStatus, Block, Plot_Size);
            }
            else
            {
                var prj = db.RealEstate_Projects.Where(x => x.Project_Name == Project).Select(x => x.Id).FirstOrDefault();
                var flr = db.Commercial_FloorsPlan.Where(x => x.Project_Id == prj && x.Floor == Floors).Select(x => x.Id).FirstOrDefault();
                var res = db.Commercial_Rooms.Where(x => x.Project_Name == Project && x.Floor_Id == flr).FirstOrDefault();
                db.Sp_Update_Lead(L.Id, L.Name, L.Father_Husband, L.Mobile_1, L.Address, L.Offered_Price, L.LeadStatus, Floors, String.Format("{0:n0}", res.Area));
            }
            string Desc = "";
            if (PreLeadStatus != L.LeadStatus)
            {
                Desc = Desc + "Lead Status " + PreLeadStatus + " Change To : " + L.LeadStatus + " -- ";
            }
            if (!(string.IsNullOrEmpty(Desc) || string.IsNullOrWhiteSpace(Desc)))
            {
                var res4 = db.Sp_Add_LeadFollowup(Desc, L.Id, userid, "Status");
            }
            return Json(true);
        }
        
        [NoDirectAccess] public ActionResult LeadsPayments()
        {
            var res = db.Sp_Get_Leads_NewLeads().ToList();
            long userid = User.Identity.GetUserId<long>();
            db.Sp_Add_Activity(userid, "Accessed Leads Payment List Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

            return View(res);
        }
        
        [NoDirectAccess] public ActionResult LeadsVoucher()
        {
            //var res = db.Sp_Get_Leads_Resale().ToList();
            long userid = User.Identity.GetUserId<long>();
            db.Sp_Add_Activity(userid, "Accessed Leads Vouchers Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

            return View();
        }
        
        [NoDirectAccess] public ActionResult AddLeadReceipts(long Id)
        {
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            var res1 = db.Sp_Get_Lead(Id).SingleOrDefault();
            var res7 = db.SAM_Receipts.Where(x => x.Lead_Id == Id).ToList();
            var res = new LeadsData { Lead = res1 };
            long userid = User.Identity.GetUserId<long>();
            db.Sp_Add_Activity(userid, "Accessed Leads Receipts Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Id);

            return PartialView(res);
        }
        
        public JsonResult AssignindSpeedFestLeads(long[] Ids, long Userid)
        {
            foreach (var item in Ids)
            {
                db.Sp_Update_SpeedFestLead(item, Userid);
            }
            
            return Json(true);
        }
        
        [NoDirectAccess] public ActionResult SPELead()
        {
            var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Sales Executive") && x.Email.Contains("propertyexchange")).ToList();
            ViewBag.LeadsUser = new SelectList(All, "Id", "Name");
            long userid = User.Identity.GetUserId<long>();
            db.Sp_Add_Activity(userid, "Accessed SPE LEads Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

            return View();
        }
        
        [NoDirectAccess] public ActionResult LeadsToDeal(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Leads to Deal Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), Id);
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            ViewBag.NewLeadPlots = new SelectList(db.Plots.Where(x => x.Status == PlotsStatus.Available_For_Sale.ToString()).Select(x => new { Id = x.Id, Name = x.Plot_Number + " " + x.Sector + " - " + x.Type + " - " + x.Block_Name }), "Id", "Name");
            
            var res1 = db.Sp_Get_Lead(Id).SingleOrDefault();
            var res4 = Enum.GetValues(typeof(LeadsStatus)).Cast<LeadsStatus>().Select(e => new { Value = e, Text = e.ToString().Replace("_", " ") }).ToList();
            ViewBag.LeadStatus = new SelectList(res4, "Value", "Text", res1.LeadStatus);
            var res7 = db.SAM_Receipts.Where(x => x.Lead_Id == Id).ToList();
            var res8 = db.SAM_Voucher.Where(x => x.Lead_Id == Id && x.VoucherNo != null).ToList();
            var res = new LeadsData { Lead = res1, Receipts = res7, Vouchers = res8 };
            return PartialView(res);
        }
        
        [NoDirectAccess] public ActionResult DealsReport()
        {
            long userid = User.Identity.GetUserId<long>();
            db.Sp_Add_Activity(userid, "Accessed Deals Report Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            var res1 = db.Sp_Get_DealLedger().ToList();
           

            return View(res1);
        }
        
        public JsonResult CreateDealNumber()
        {
            var res = db.Leads.Where(x => x.Deal_Number == null).ToList();
            foreach (var v in res)
            {
                db.Sp_Update_SAM_Leads_DealNumber(v.Id);
            }
            return Json(true);
        }
		
        [NoDirectAccess] public ActionResult UploadLeads(string Project, string status)
        {
            ViewBag.Status = status;
            ViewBag.Project = Project;
            return PartialView();
        }
        
        [NoDirectAccess] public ActionResult LeadsDetails(int type, DateTime? From, DateTime? To, string LeadStatus, long? LeadUser, string Phone)
        {
            if (LeadStatus == "") { LeadStatus = null; }
            if (LeadUser == 0) { LeadUser = null; }
            long userid = User.Identity.GetUserId<long>();
            db.Sp_Add_Activity(userid, "Searched For SAMLeads from " + From + " To: " + To, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            string[] sources = { "Walkin", "UAN", "Social Media", "Zameen Leads", "Reference", "Other Deals" };
            var ty = sources[type];
            if (type <= 5)
            {

                if (From == null || To == null)
                {
                    DateTime date = DateTime.Now;
                    var res = db.Sp_Get_Lead_Search_Comp(LeadUser, date, date, LeadStatus).ToList();
                    res = res.Where(x => x.Source == sources[type]).ToList();
                    if (Phone != "" || Phone != null)
                    {
                        res = res.Where(x => x.Mobile_1.Contains(Phone)).ToList();
                    }
                    return PartialView(res);
                }
                else
                {
                    var res = db.Sp_Get_Lead_Search_Comp(LeadUser, From, To, LeadStatus).ToList();
                    res = res.Where(x => x.Source == sources[type]).ToList();
                    if (Phone != "" || Phone != null)
                    {
                        res = res.Where(x => x.Mobile_1.Contains(Phone)).ToList();
                    }
                    return PartialView(res);
                }
            }
            else
            {
                DateTime date = DateTime.Now;
                if (From == null || To == null)
                {
                    var res = db.Sp_Get_Lead_Search_Comp(LeadUser, date, date, LeadStatus).Where(x => x.Source == null).ToList();
                    res = res.Where(x => x.Source == sources[type]).ToList();
                    if (Phone != "" || Phone != null)
                    {
                        res = res.Where(x => x.Mobile_1.Contains(Phone)).ToList();
                    }
                    return PartialView(res);
                }
                else
                {
                    var res = db.Sp_Get_Lead_Search_Comp(LeadUser, From, To, LeadStatus).Where(x => x.Source == null).ToList();
                    res = res.Where(x => x.Source == sources[type]).ToList();
                    if (Phone != "" || Phone != null)
                    {
                        res = res.Where(x => x.Mobile_1.Contains(Phone)).ToList();
                    }
                    return PartialView(res);
                }
            }
        }
        
        [NoDirectAccess] public ActionResult MyLeadsDetails(int type, DateTime? From, DateTime? To, string LeadStatus, long? LeadUser, string Phone)
        {
            if (LeadStatus == "") { LeadStatus = null; }
            if (LeadUser == 0) { LeadUser = null; }
            long userid = User.Identity.GetUserId<long>();
            db.Sp_Add_Activity(userid, "Searched For SAMLeads from " + From + " To: " + To, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            string[] sources = { "Walkin", "UAN", "Social Media", "Zameen Leads", "Reference", "Other Deals" };
            var ty = sources[type];
            if (type <= 5)
            {

                if (From == null || To == null)
                {
                    var res = db.Sp_Get_Lead_Search_Comp(LeadUser, null, null, LeadStatus).ToList();
                    res = res.Where(x => x.Source == sources[type]).ToList();
                    if (Phone != "" || Phone != null)
                    {
                        res = res.Where(x => x.Mobile_1.Contains(Phone)).ToList();
                    }
                    return PartialView(res);
                }
                else
                {
                    var res = db.Sp_Get_Lead_Search_Comp(LeadUser, From, To, LeadStatus).ToList();
                    res = res.Where(x => x.Source == sources[type]).ToList();
                    if (Phone != "" || Phone != null)
                    {
                        res = res.Where(x => x.Mobile_1.Contains(Phone)).ToList();
                    }
                    return PartialView(res);
                }
            }
            else
            {
                if (From == null || To == null)
                {
                    var res = db.Sp_Get_Lead_Search_Comp(LeadUser, null, null, LeadStatus).Where(x => x.Source == null).ToList();
                    res = res.Where(x => x.Source == sources[type]).ToList();
                    if (Phone != "" || Phone != null)
                    {
                        res = res.Where(x => x.Mobile_1.Contains(Phone)).ToList();
                    }
                    return PartialView(res);
                }
                else
                {
                    var res = db.Sp_Get_Lead_Search_Comp(LeadUser, From, To, LeadStatus).Where(x => x.Source == null).ToList();
                    res = res.Where(x => x.Source == sources[type]).ToList();
                    if (Phone != "" || Phone != null)
                    {
                        res = res.Where(x => x.Mobile_1.Contains(Phone)).ToList();
                    }
                    return PartialView(res);
                }
            }
        }
        
        public JsonResult SaveLeadsList(List<LeadsFile>  AllLeads, string Project, string UnAssigned)
        {
            List<Lead> PrLeads = new List<Lead>();
            try
            {
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                long userid = long.Parse(User.Identity.GetUserId());
                if (UnAssigned == "Assigned")
                {
                    foreach (var v in AllLeads)
                    {
                        Lead L = new Lead
                        {
                            Name = v.Name,
                            Father_Husband = v.FatherName,
                            Mobile_1 = v.Mobile,
                            Address = v.Address,
                            Source = v.Source,
                            LeadStatus = LeadsStatus.Initial_Discussion.ToString(),
                            AssignedTo = uid,
                            Datetime = DateTime.Now,
                            Project = Project,

                        };
                        var res2 = db.Sp_Add_Lead(L.Address, userid, "", L.Mobile_1, L.Name, L.Father_Husband, L.Offered_Price, L.Source, L.Plot_Size, L.LeadStatus, L.Project, DateTime.Now).FirstOrDefault();
                        var res3 = db.Sp_Add_LeadFollowup(v.Remarks, res2, userid, "Text");
                    }
                }
                else if (UnAssigned == "UnAssigned")
                {
                    foreach (var v in AllLeads)
                    {
                        Lead L = new Lead
                        {
                            Name = v.Name,
                            Father_Husband = v.FatherName,
                            Mobile_1 = v.Mobile,
                            Address = v.Address,
                            Source = v.Source,
                            LeadStatus = LeadsStatus.Initial_Discussion.ToString(),
                            AssignedTo = 0,
                            Datetime = DateTime.Now,
                            Project = Project,

                        };
                        var res2 = db.Sp_Add_Lead(L.Address, 0, "", L.Mobile_1, L.Name, L.Father_Husband, L.Offered_Price, L.Source, L.Plot_Size, L.LeadStatus, L.Project, null).FirstOrDefault();
                        var res3 = db.Sp_Add_LeadFollowup(v.Remarks, res2, 0, "Text");
                    }
                }
                
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
        
        public JsonResult GetProjectWiseInventory(string Project, string Type)
        {
            if (Type == "New_Lead")
            {
                var res = db.Commercial_Rooms.Where(x => x.Project_Name == Project && x.Status == PlotsStatus.Available_For_Sale.ToString()).ToList();
                return Json(res);
            }
            else if (Type == "Resale" || Type == "Investment")
            {
                var res = db.Commercial_Rooms.Where(x => x.Project_Name == Project && x.Status == PlotsStatus.Registered.ToString()).ToList();
                return Json(res);
            }
            else
            {
                return Json(false);
            }
        }
        
        public JsonResult MEDSaveLeadsList(List<LeadsFile> AllLeads, string Project, string UnAssigned)
        {
            List<Lead> PrLeads = new List<Lead>();
            try
            {
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                long userid = long.Parse(User.Identity.GetUserId());
                if (UnAssigned == "Assigned")
                {
                    foreach (var v in AllLeads)
                    {
                        Lead L = new Lead
                        {
                            Name = v.Name,
                            Father_Husband = v.FatherName,
                            Mobile_1 = v.Mobile,
                            Address = v.Address,
                            Source = v.Source,
                            LeadStatus = LeadsStatus.Initial_Discussion.ToString(),
                            AssignedTo = uid,
                            Datetime = DateTime.Now,
                            Project = Project,
                        };
                        var res2 = db.Sp_Add_Lead(L.Address, userid, "", L.Mobile_1, L.Name, L.Father_Husband, L.Offered_Price, L.Source, L.Plot_Size, L.LeadStatus, L.Project, DateTime.Now).FirstOrDefault();
                        var res3 = db.Sp_Add_LeadFollowup(v.Remarks, res2, userid, "Text");
                    }
                }
                else if (UnAssigned == "UnAssigned")
                {
                    foreach (var v in AllLeads)
                    {
                        Lead L = new Lead
                        {
                            Name = v.Name,
                            Father_Husband = v.FatherName,
                            Mobile_1 = v.Mobile,
                            Address = v.Address,
                            Source = v.Source,
                            LeadStatus = LeadsStatus.Initial_Discussion.ToString(),
                            AssignedTo = 0,
                            Datetime = DateTime.Now,
                            Project = Project,
                        };
                        var res2 = db.Sp_Add_Lead(L.Address, 0, "", L.Mobile_1, L.Name, L.Father_Husband, L.Offered_Price, L.Source, L.Plot_Size, L.LeadStatus, L.Project, null).FirstOrDefault();
                        var res3 = db.Sp_Add_LeadFollowup(v.Remarks, res2, 0, "Text");
                    }
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        [NoDirectAccess] public ActionResult CreateUnassignLead()
        {
            long userid = User.Identity.GetUserId<long>();
            db.Sp_Add_Activity(userid, "Accessed Create Lead Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            ViewBag.Block = new SelectList(db.Sp_Get_Block().ToList(), "Block_Name", "Block_Name");
            ViewBag.Project = new SelectList(db.RealEstate_Projects.Where(x => x.Type == ProjectCategory.Building.ToString() || x.Type == ProjectCategory.General.ToString()).ToList(), "Project_Name", "Project_Name");

            return PartialView();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateUnassignedLead(Lead L, string Block, string Description)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Created New Lead With description " + Description + " For " + Block + " Block", "Create", "Activity_Record", ActivityType.Lead.ToString(), userid);

            var res2 = db.Sp_Add_Lead(L.Address, 0, Block, L.Mobile_1, L.Name, L.Father_Husband, L.Offered_Price, "UAN", L.Plot_Size, L.LeadStatus, L.Project, null).FirstOrDefault();
            if (res2 == -1)
            {
                return Json(false); ;
            }
            var res3 = db.Sp_Add_LeadFollowup(Description, res2, userid, "Text");
            return Json(true);
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