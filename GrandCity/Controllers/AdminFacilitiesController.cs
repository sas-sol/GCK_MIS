using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using MeherEstateDevelopers.Filters;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class AdminFacilitiesController : Controller
    {
        // GET: AdminFacilities
        private Grand_CityEntities db = new Grand_CityEntities();
        public ActionResult Project()
        {
            return View();
        }
        public ActionResult GetPlan()
        {
            var res = db.Sp_Get_EntertaimentFeePlan().ToList();
            return PartialView(res);
        }
        public JsonResult RemovePlan(long Id)
        {
            var res = db.Sp_Delete_EntertaimentFeePlan(Id).SingleOrDefault();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Removed Plan ", "Delete", "Activity_Record", ActivityType.Admin_Facilities.ToString(), Id);
            if (res == 1)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
        //
        public ActionResult UpdatePlanData(long Id)
        {
            var res = db.AdminProjects.Where(x => x.Project_Id == Id).SingleOrDefault();
            return PartialView(res);
        }
        //
        public JsonResult UpdatePlan(long Id)
        {
            var res = db.Sp_Delete_EntertaimentFeePlan(Id).SingleOrDefault();
 long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Updated Plan  ", "Update", "Activity_Record", ActivityType.Admin_Facilities.ToString(), Id);
            if (res == 1)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        // Fee Structure
        public JsonResult AddProject(List<AdminProjects> AP)
        {
            long userid = long.Parse(User.Identity.GetUserId());
             
            db.Sp_Add_Activity(userid, "Added Prodject Facilitation Fee Structure ", "Create", "Activity_Record", ActivityType.Admin_Facilities.ToString(), userid);
            foreach (var item in AP)
            {
                var Id = db.Sp_Add_RealEstateProject(item.project_Name, item.Description, Types.Sports.ToString()).FirstOrDefault();
                if (Id != null)
                {
                    var installmentplan = new XElement("AdminFacilities", AP.Select(x => new XElement("FacilitiesFeeStructure",
                                               new XAttribute("Project_Id", Id),
                                               new XAttribute("Emp_Fee", x.Emp_Fee),
                                               new XAttribute("Emp_Mem_Fee", x.Emp_Mem_Fee),
                                               new XAttribute("Res_Fee", x.Res_Fee),
                                               new XAttribute("Res_Mem_Fee", x.Res_Mem_Fee),
                                               new XAttribute("Out_Fee", x.Out_Fee),
                                               new XAttribute("Out_Mem_Fee", x.Out_Mem_Fee),
                                               new XAttribute("Entry_Date", DateTime.UtcNow),
                                               new XAttribute("Userid", userid)
                                               ))).ToString();
                    db.Sp_Add_AdminProject(installmentplan);
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }

            }
            return Json(false);
        }
        public JsonResult GetProject()
        {
            db.Sp_Get_AdminProject().ToList();
            return Json(true);
        }
        public ActionResult Registration()
        {
            var res = db.RealEstate_Projects.Where(x => x.Type == Modules.Sports.ToString()).ToList();
            ViewBag.Block = new SelectList(db.Sp_Get_Block(), "Id", "Block_Name");
            ViewBag.City = new SelectList(Nomenclature.Cities(), "Name", "Name");
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            return View(res);
        }

        public JsonResult RegisterMember(RegisterMember M, long[] ids,long TransactionId)
        {
            if (ids != null)
            {
                long userid = long.Parse(User.Identity.GetUserId());
                AccountHandlerController ah = new AccountHandlerController();
                var comp = ah.Company_Attr(userid);
                if (M.CNIC == null || M.Name == null || M.Mobile_1 == null)
                {
                    return Json(false);
                }
                var id = db.Sp_Add_Members(M.Type, M.Name, M.Father_Name, M.CNIC, M.Date_Of_Birth, M.Address, M.Email, M.Mobile_1, M.Mobile_2,
                  M.City, M.Blood_Group, M.Religion, M.Marital_Status, M.Relationship, M.Emergency_Contact
                  , M.Date_Of_Joining, DateTime.UtcNow, userid, "Registered", M.Module_Id).FirstOrDefault();

                
                db.Sp_Add_Activity(userid, " Registered Member " + M.Name  , "Create", "Activity_Record", ActivityType.Admin_Facilities.ToString(), TransactionId);
                List<string> Names = new List<string>();
                List<long> ldsCollerter = new List<long>();
                List<long> IdChecker = new List<long>();
                List<decimal> sumcollerter = new List<decimal>();
                List<AdminProject> ad = new List<AdminProject>();
                if (id != null)
                {
                    for (var i = 0; i < ids.Length; i++)
                    {
                        ldsCollerter.Add(ids[i]);
                    }
                    var Type = db.RegisterMembers.Where(x => x.Id == id).Select(x => x.Type).SingleOrDefault();
                    if (Type == "Employee")
                    {
                        foreach (var item in ldsCollerter)
                        {
                            var cal = db.AdminProjects.Where(x => x.Project_Id == item).SingleOrDefault();
                            var ProjecName = db.RealEstate_Projects.Where(x => x.Id == item).Select(x => x.Project_Name).SingleOrDefault();
                            var subs = db.Sp_Add_Members_Subscription(id, item, ProjecName, cal.Emp_Fee).SingleOrDefault();
                            if (subs == 0)
                            {
                                IdChecker.Add(item);
                                Names.Add(ProjecName);
                            }
                            else
                            {
                                sumcollerter.Add(Convert.ToDecimal(cal.Emp_Mem_Fee));
                            }

                        }
                    }
                    else if (Type == "Residential")
                    {
                        foreach (var item in ldsCollerter)
                        {
                            var cal = db.AdminProjects.Where(x => x.Project_Id == item).SingleOrDefault();
                            var ProjecName = db.RealEstate_Projects.Where(x => x.Id == item).Select(x => x.Project_Name).SingleOrDefault();
                            var subs = db.Sp_Add_Members_Subscription(id, item, ProjecName, cal.Res_Fee).SingleOrDefault();
                            if (subs == 0)
                            {
                                IdChecker.Add(item);
                                Names.Add(ProjecName);
                            }
                            else
                            {
                                sumcollerter.Add(Convert.ToDecimal(cal.Res_Mem_Fee));
                            }

                        }
                    }
                    else if (Type == "Outsider")
                    {
                        foreach (var item in ldsCollerter)
                        {
                            var cal = db.AdminProjects.Where(x => x.Project_Id == item).SingleOrDefault();
                            var ProjecName = db.RealEstate_Projects.Where(x => x.Id == item).Select(x => x.Project_Name).SingleOrDefault();
                            var subs = db.Sp_Add_Members_Subscription(id, item, ProjecName, cal.Out_Fee).SingleOrDefault();
                            if (subs == 0)
                            {
                                IdChecker.Add(item);
                                Names.Add(ProjecName);
                            }
                            else
                            {
                                sumcollerter.Add(Convert.ToDecimal(cal.Out_Mem_Fee));
                            }

                        }
                    }
                    List<string> ReceiptCollector = new List<string>();
                    ldsCollerter.RemoveAll(x => IdChecker.Contains(x));
                    decimal calf = 0;
                    foreach (var item in ldsCollerter)
                    {
                        var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();

                        if (sumcollerter.Sum() > 0)
                        {
                            if (Type == "Employee")
                            {
                                calf = db.AdminProjects.Where(x => x.Project_Id == item).Select(x => x.Emp_Mem_Fee).SingleOrDefault().Value;
                            }
                            else if (Type == "Residential")
                            {
                                calf = db.AdminProjects.Where(x => x.Project_Id == item).Select(x => x.Res_Mem_Fee).SingleOrDefault().Value;
                            }
                            else if (Type == "Outsider")
                            {
                                calf = db.AdminProjects.Where(x => x.Project_Id == item).Select(x => x.Out_Mem_Fee).SingleOrDefault().Value;
                            }
                            var name = db.RealEstate_Projects.Where(x => x.Id == item).Select(x => x.Project_Name).SingleOrDefault();
                            var res3 = db.Sp_Add_Receipt(calf, GeneralMethods.NumberToWords(Convert.ToInt32(calf)), null, null, null, null, M.Mobile_1
                                                 , M.Father_Name, id, M.Name, "Cash", 0,
                                                 Modules.Sports.ToString(), 0, null, null, ReceiptTypes.Membership_Fee.ToString(), userid, userid, name, null,
                                                 Types.Sports.ToString(), null, null, null,null, id,TransactionId,"", receiptno, comp.Id).FirstOrDefault();
                            ReceiptCollector.Add(res3.Receipt_No);
                            //bool headcashier = false;
                            //if (User.IsInRole("Head Cashier"))
                            //{
                            //    headcashier = true;
                            //}
                            //try
                            //{
                            //    AccountHandlerController de = new AccountHandlerController();
                            //    de.Other_Recovery(calf, M.Name, M.Mobile_1, "", "Cash", null, null, null, "Membership Fee", TransactionId, userid, res3.Receipt_No, 1, "Society Amenities", headcashier);
                            //}
                            //catch (Exception ex)
                            //{
                            //    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, "AdminFacilities", "RegisterMember");
                            //}
                            
                        }
                        //}
                    }
                    if (ReceiptCollector.Any() || Names.Any())
                    {
                        var data = new { Id = ReceiptCollector, Token = userid, Name = Names };
                        return Json(data);
                    }
                    // if sum is 0 and no repeat
                    else if (!ReceiptCollector.Any() && !Names.Any())
                    {

                        return Json(2);
                    }

                    else
                    {
                        return Json(false);
                    }

                }
                else
                {
                    return Json(false);
                }
            }
            else
            {
                return Json(false);
            }
        }
        public JsonResult GetEmployee(string EmpCode)
        {
            var res = db.Sp_Get_EmployeeCode_Parameter(EmpCode).SingleOrDefault();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Viewes Employee Detail ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return Json(res);
        }
        public JsonResult GetPlotInformation(long Id)
        {
            var res = db.Sp_Get_PlotLastOwner(Id).FirstOrDefault();
            return Json(res);
        }
        public JsonResult MembershipFee(string Type, long Id)
        {
            var res = db.Sp_Get_MembershipFee(Type, Id).SingleOrDefault();
            return Json(res);
        }

        public ActionResult AmountPayModel(string Receiptid, decimal Amount, long Id)
        {
            ViewBag.ReceiptId = Receiptid;
            ViewBag.userid = long.Parse(User.Identity.GetUserId());
            ViewBag.Amt = Amount;
            ViewBag.Bank = new SelectList(Nomenclature.Banks(), "Name", "Name");
            return PartialView();
        }

        //[HttpPost]
        //public JsonResult Registration(AdminProject p)
        //{

        //    return Json(true);
        //}
        public ActionResult RegisterdMembers()
        {
            var res = db.RealEstate_Projects.Where(x => x.Type == Modules.Sports.ToString()).ToList();
            return View(res);
        }
        public ActionResult SegregatedMembers(long Id, string Type)
        {
            ViewBag.Project = Type;
            var res = db.Sp_Get_RegisterdMembers_Segregated(Id).ToList();
            return PartialView(res);
        }
        public ActionResult MembersDetails(long Id)
        {
            var res = db.RegisterMembers.Where(x => x.Id == Id).SingleOrDefault();
            return PartialView(res);
        }
        public void GenerateFee()
        {
            var userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Update_SalarySteps_Dates("Entertainment", DateTime.UtcNow);
            var res = db.Sp_Get_RegisterdMembers().ToList();
            foreach (var item in res)
            {
                var fee = db.Members_Subscription.Where(x => x.Mem_Id == item.Id && x.Proj_Id == item.Proid && x.Status == "Registered").Select(x => x.FeeAmount).SingleOrDefault();
                if (fee == null)
                {
                    continue;
                }
                db.Sp_Add_MembersFee(item.Id, item.Name, item.CNIC, item.Type, item.Register_Date, item.Proid, item.Project_Name, DateTime.UtcNow, fee, "Pending", userid);
            }
        }
        public ActionResult Detail()
        {
            var res = db.RealEstate_Projects.Where(x => x.Type == Types.Sports.ToString()).ToList();
            return View(res);
        }
        public ActionResult ProjectFee(long ProjectId)
        {
            db.Sp_Update_SalarySteps_Dates("Entertainment", DateTime.UtcNow);
            var res = db.Sp_Get_All_MembersFee(ProjectId).ToList();
            return PartialView(res);
        }
        public ActionResult CashCOunterDetail()
        {
            var res = db.RealEstate_Projects.Where(x => x.Type == Types.Sports.ToString()).ToList();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, " Accessed Cash Counter Details ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return View(res);
        }
        public ActionResult CashCounterProjectFee(long ProjectId)
        {
            db.Sp_Update_SalarySteps_Dates("Entertainment", DateTime.UtcNow);

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Cash Counter Project Fee  ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), ProjectId);
            var res = db.Sp_Get_All_MembersFee(ProjectId).ToList();
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            return PartialView(res);
        }
        public ActionResult MonthlyFeeParameter(long ProjectId, DateTime date)
        {

            var res = db.Sp_Get_Fee_Month_Parameter(ProjectId, date).ToList();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Checked Monthly Fee  " + date, "Read", "Activity_Record", ActivityType.Page_Access.ToString(), ProjectId);
            return PartialView(res);
        }
        public ActionResult MonthlyFeeParameterCashCounter(long ProjectId, DateTime date)
        {
            Helpers h = new Helpers();
            ViewBag.TransactionId = h.RandomNumber();
            var res = db.Sp_Get_Fee_Month_Parameter(ProjectId, date).ToList();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Monthly Fee Cash Counter", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), ProjectId);
            return PartialView(res);
        }
        public ActionResult MemberListingproject()
        {
            var res = db.RealEstate_Projects.Where(x => x.Type == Types.Sports.ToString()).ToList();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Members List ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return View(res);
        }
        public ActionResult MembersListing(long ProjectId)
        {
            var res = db.Sp_Get_Member_Parameter(ProjectId).ToList();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Member Listings", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), ProjectId);
            return PartialView(res);
        }
        public ActionResult DailyFeeDistrubution(long ProjectId)
        {
            var res = db.Sp_Get_MembersFee_Delivery(ProjectId).ToList();

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, " Accessed Daily Distribution Fee Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), ProjectId);

            return PartialView(res);
        }
        public ActionResult Memberdetail(long MemberId)
        {
            var res1 = db.RegisterMembers.Where(x => x.Id == MemberId).SingleOrDefault();
            var res2 = db.Members_Subscription.Where(x => x.Mem_Id == MemberId).ToList();
            var res3 = db.MembersFees.Where(x => x.Mem_Id == MemberId).ToList();
            var res4 = new AdminFacilitiesMembership { MemberDetails = res1, Subscription = res2, FeeStructure = res3 };

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Member Details", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), MemberId);
            return PartialView(res4);
        }
        public JsonResult UpdateFee(long Id, decimal? Amount)
        {
            if (Amount == null)
            {
                Amount = 0;
            }
            db.Sp_Update_SubscriptionFee(Id, Amount);

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Updated Fee to " + Amount, "Update", "Activity_Record", ActivityType.Admin_Facilities.ToString(), Id);
            return Json(true);
        }
        public JsonResult SubsUnsubs(long Id, string status)
        {
            db.Sp_Update_ProjectSubscription(Id, status);

            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Chnaged Member status to " + status, "Read", "Activity_Record", ActivityType.Admin_Facilities.ToString(), Id);
            return Json(true);
        }
        public JsonResult FeeReceipt(long FeeId, long TransactionId)
        {
            var userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var MFD = db.MembersFees.Where(x => x.Id == FeeId).SingleOrDefault();
            var MemberData = db.RegisterMembers.Where(x => x.Id == MFD.Mem_Id).SingleOrDefault();
            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Sp_Update_Fee_Status(userid, FeeId);
                    var ReceiptId = db.Sp_Add_Receipt(MFD.FeeAmount, GeneralMethods.NumberToWords(Convert.ToInt32(MFD.FeeAmount)), null, null, null, null, MemberData.Mobile_1
                                                                          , MemberData.Father_Name, MemberData.Id, MemberData.Name, "Cash", 0,
                                                                          MFD.Project_Name, 0, null, "", ReceiptTypes.Membership_Monthly_Fee.ToString(), userid, userid,
                                                                          MFD.Project_Name, null, MFD.Project_Name, null, null, null,null, MemberData.Id,TransactionId,"",receiptno, comp.Id).FirstOrDefault();
                    bool headcashier = false;
                    if (User.IsInRole("Head Cashier"))
                    {
                        headcashier = true;
                    }
                    try
                    {
                        AccountHandlerController de = new AccountHandlerController();
                        var res1 = de.Admin_Facilities_FeeReceived(MFD.FeeAmount, MemberData.Name, MemberData.Mobile_1 + ", " + MemberData.Mobile_2, MFD.Project_Name, TransactionId, userid, 1, ReceiptId.Receipt_No.ToString(), headcashier);
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, "AdminFacilities", "FeeReceipt");
                    }


                    db.Sp_Add_Activity(userid, "Generated Member Fee Receipt ", "Create", "Activity_Record", ActivityType.Admin_Facilities.ToString(), TransactionId);
                    Transaction.Commit();
                    var data1 = new { Status = "1", Receiptid = ReceiptId.Receipt_No, Token = userid };
                    return Json(data1);
                }
                catch (Exception e)
                {
                    Transaction.Rollback();
                    return Json(false);
                }
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