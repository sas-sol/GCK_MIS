using MeherEstateDevelopers.Filters;
using MeherEstateDevelopers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Xml.Linq;
using static MeherEstateDevelopers.MvcApplication;

namespace MeherEstateDevelopers.Controllers
{
    [LogAction]
    [Authorize]
    public class ArrearsSalariesController : Controller
    {
        // GET: ArrearsSalaries
        private Grand_CityEntities db = new Grand_CityEntities();

        // SALARY INDEX PAGE
       [NoDirectAccess] public ActionResult SalaryIndex()
        {
            var res = db.Sp_Get_Employee().ToList();
            return PartialView(res);
        }
        // salary table show by month and year search
        public JsonResult GenerateiontableSalariesSlip(long[] EmpIds)
        {
            var ResEmp = db.Employees.Where(x => EmpIds.Contains(x.Id)).ToList();
            List<string> Names = new List<string>();
            foreach (var item in ResEmp)
            {
                var res = db.Sp_Add_Arrears(item.Id, item.Name, item.Department_Designation, item.Employee_Code, item.CNIC, item.Basic_Salary, item.Date_Of_Joining, item.Email).SingleOrDefault();
                if (res != 1)
                {
                    Names.Add(item.Name);
                }
            }
            if (Names != null)
            {
                return Json(Names);
            }
            return Json(true);
        }
        [HttpPost]
        public JsonResult Status(string status)
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            if (status == "Approved")
            {
                var res = db.Sp_Get_Arrears("FinanceApproved", null, null).ToList();
                //AccountHandlerController de = new AccountHandlerController();
                //var salaryHead = de.SalaryExpense();
                //var netSalaryHead = de.NetSalary();
                //foreach (var person in res)
                //{
                //    var receivableAccount = de.OtherDeductions(person.Employee_code);
                //    List<GeneralEntryDetailsModel> Details = new List<GeneralEntryDetailsModel>();
                //    var tranId = new Helpers().RandomNumber();
                //    int j = 0;

                //    GeneralEntryDetailsModel geItems = new GeneralEntryDetailsModel();
                //    //Gross Salary Dr
                //    geItems.Narration = "Gross Arrears of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
                //    geItems.HeadCode = salaryHead.Code;
                //    geItems.HeadName = salaryHead.Head;
                //    geItems.HeadId = salaryHead.Id;
                //    geItems.TransactionId = tranId;
                //    geItems.Line = ++j;
                //    geItems.Debit = person.Arrears_Amount + person.Allownces;
                //    geItems.Credit = 0;
                //    Details.Add(geItems);
                //    if (person.Deductions > 0)
                //    {
                //        GeneralEntryDetailsModel geItems6 = new GeneralEntryDetailsModel();
                //        geItems6.Narration = "Other Deductions of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
                //        geItems6.HeadCode = receivableAccount.Code;
                //        geItems6.HeadName = receivableAccount.Head;
                //        geItems6.HeadId = receivableAccount.Id;
                //        geItems6.TransactionId = tranId;
                //        geItems6.Line = ++j;
                //        geItems6.Debit = 0;
                //        geItems6.Credit = person.Deductions;
                //        Details.Add(geItems6);
                //    }
                //    if (person.Grand_Total > 0)
                //    {
                //        GeneralEntryDetailsModel geItems8 = new GeneralEntryDetailsModel();
                //        geItems8.Narration = "Net Salary of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
                //        geItems8.HeadCode = netSalaryHead.Code;
                //        geItems8.HeadName = netSalaryHead.Head;
                //        geItems8.HeadId = netSalaryHead.Id;
                //        geItems8.TransactionId = tranId;
                //        geItems8.Line = ++j;
                //        geItems8.Debit = 0;
                //        geItems8.Credit = person.Grand_Total;
                //        Details.Add(geItems8);
                //    }
                //    if (User.IsInRole("Head Cashier"))
                //    {
                //        var JournalEnt = new XElement("JournalEntries", Details.Select(x => new XElement("Entries",
                //        new XAttribute("Naration", x.Narration),
                //        new XAttribute("Head", (x.HeadName == null) ? "" : x.HeadCode + " - " + x.HeadName),
                //        new XAttribute("Head_Name", x.HeadName),
                //        new XAttribute("Head_Code", x.HeadCode),
                //        new XAttribute("Head_Id", x.HeadId),
                //        new XAttribute("Line", x.Line),
                //        new XAttribute("Qty", 0),
                //        new XAttribute("UOM", ""),
                //        new XAttribute("Rate", 0),
                //        new XAttribute("Debit", x.Debit),
                //        new XAttribute("Credit", x.Credit),
                //        new XAttribute("GroupId", x.TransactionId)
                //        ))).ToString();
                //        db.Sp_Add_Journal_Entries(JournalEnt, Userid);
                //    }
                //    else
                //    {
                //        var GeneralEnt = new XElement("GeneralEntries", Details.Select(x => new XElement("Entries",
                //        new XAttribute("Naration", x.Narration),
                //        new XAttribute("Head", (x.HeadName == null) ? "" : x.HeadCode + " - " + x.HeadName),
                //        new XAttribute("Head_Name", x.HeadName),
                //        new XAttribute("Head_Code", x.HeadCode),
                //        new XAttribute("Head_Id", x.HeadId),
                //        new XAttribute("Line", x.Line),
                //        new XAttribute("Qty", 0),
                //        new XAttribute("UOM", ""),
                //        new XAttribute("Rate", 0),
                //        new XAttribute("Debit", x.Debit),
                //        new XAttribute("Credit", x.Credit),
                //        new XAttribute("GroupId", x.TransactionId)
                //        ))).ToString();
                //        db.Sp_Add_General_Entries(GeneralEnt, Userid);
                //    }
                    
                //}
            }

            db.Sp_Update_ArrearsStatus(status);
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Updated Arrereas Salary status to " + status, "Update", "Activity_Record", ActivityType.Arreares_Salaries.ToString(), userid);
            return Json(true);
        }
        // GET All SALARIES SLIP
       [NoDirectAccess] public ActionResult AllSalaries()
        {
            var res = db.Sp_Get_Arrears(null, null, null).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Salaries Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return PartialView(res);
        }
        // Hr inproecss salaris show
       [NoDirectAccess] public ActionResult InProcessSalariesSlip()
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_Arrears("Inprocess", null, null).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed In Process Salaries", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return PartialView(res);
        }

       [NoDirectAccess] public ActionResult FinanceSalarySlipPrint()
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            var uname = db.Users.Where(x => x.Id == Userid).Select(x => x.Name).FirstOrDefault();
            var res = db.Sp_Get_Arrears("Approved", null, null).ToList();
            foreach(var v in res)
            {
                MarkActivity(new EmployeeHistory
                {
                    AddedBy_Id = Userid,
                    AddedBy_Name = uname,
                    AddedOn = DateTime.Now,
                    Comment = "Arrear of amount " + v.Grand_Total + " has been approved.",
                    CommentType = ActivityType.HR_Arrears.ToString(),
                    EmpId = v.Emp_Id,
                    EmpName = v.Employee_Name,
                    GroupId = new Helpers().RandomNumber()
                });
            } 
            return View(res);
        }
       [NoDirectAccess] public ActionResult FinanceSalariesSlip(int? Month, int? Year)
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_Arrears("Approved",Month, Year).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Finance Salaries Slip Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return PartialView(res);
        }
       [NoDirectAccess] public ActionResult FinanceApproved()
        {
            
            long Userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_Arrears("FinanceApproved",null, null).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Finance Approved Salaries Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return PartialView(res);
        }
        // remarks updation
        [HttpPost]
       [NoDirectAccess] public ActionResult UpdateDescription(long? Id, string Remarks)
        {
            var res = db.Sp_Update_Arrears_Description(Id, Remarks);
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "UPdated Remarks to " + Remarks, "Update", "Activity_Record", ActivityType.Arreares_Salaries.ToString(), Id);
            return View(true);
        }
        //
        [HttpPost]
        public JsonResult ArrearsAmount(long? Id, decimal? Amount)
        {
            db.Sp_Update_Arrears_Amount(Id, Amount);
            var res = db.Arrears.Where(x => x.Id == Id).Select(x => x.Grand_Total).SingleOrDefault();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "UPdated Arreares Amount to " + Amount, "Update", "Activity_Record", ActivityType.Arreares_Salaries.ToString(), Id);
            return Json(res);
        }
        [HttpPost]
        public JsonResult ReverseStatus(long Id, string Status)
        {
            if (Status == "")
            {
                Status = null;
            }
            long Userid = long.Parse(User.Identity.GetUserId());
            var uname = db.Users.Where(x => x.Id == Userid).Select(x => x.Name).FirstOrDefault();
            var res = db.Arrears.Where(x => x.Id == Id).FirstOrDefault();
            db.Sp_Update_ArrearsStatus_Parameter(Status, Id);

            if (Status == "Approved")
            {
                //AccountHandlerController de = new AccountHandlerController();
                //var salaryHead = de.SalaryExpense();
                //var netSalaryHead = de.NetSalary();
                //var receivableAccount = de.OtherDeductions(res.Employee_code);
                //List<GeneralEntryDetailsModel> Details = new List<GeneralEntryDetailsModel>();
                //var tranId = new Helpers().RandomNumber();
                //int j = 0;
                //GeneralEntryDetailsModel geItems = new GeneralEntryDetailsModel();
                ////Gross Salary Dr
                //geItems.Narration = "Gross Arrears of " + res.Employee_Name + " - " + res.Employee_code + " for the month " + res.Salary_Month.Value.ToString("MM/yyyy");
                //geItems.HeadCode = salaryHead.Code;
                //geItems.HeadName = salaryHead.Head;
                //geItems.HeadId = salaryHead.Id;
                //geItems.TransactionId = tranId;
                //geItems.Line = ++j;
                //geItems.Debit = res.Arrears_Amount + res.Allownces;
                //geItems.Credit = 0;
                //Details.Add(geItems);
                //if (res.Deductions > 0)
                //{
                //    GeneralEntryDetailsModel geItems6 = new GeneralEntryDetailsModel();
                //    geItems6.Narration = "Other Deductions of " + res.Employee_Name + " - " + res.Employee_code + " for the month " + res.Salary_Month.Value.ToString("MM/yyyy");
                //    geItems6.HeadCode = receivableAccount.Code;
                //    geItems6.HeadName = receivableAccount.Head;
                //    geItems6.HeadId = receivableAccount.Id;
                //    geItems6.TransactionId = tranId;
                //    geItems6.Line = ++j;
                //    geItems6.Debit = 0;
                //    geItems6.Credit = Math.Abs(Convert.ToDecimal(res.Deductions));
                //    Details.Add(geItems6);
                //}
                //if (res.Grand_Total > 0)
                //{
                //    GeneralEntryDetailsModel geItems8 = new GeneralEntryDetailsModel();
                //    geItems8.Narration = "Net Arrears of " + res.Employee_Name + " - " + res.Employee_code + " for the month " + res.Salary_Month.Value.ToString("MM/yyyy");
                //    geItems8.HeadCode = netSalaryHead.Code;
                //    geItems8.HeadName = netSalaryHead.Head;
                //    geItems8.HeadId = netSalaryHead.Id;
                //    geItems8.TransactionId = tranId;
                //    geItems8.Line = ++j;
                //    geItems8.Debit = 0;
                //    geItems8.Credit = res.Grand_Total;
                //    Details.Add(geItems8);
                //}
                //var GeneralEnt = new XElement("GeneralEntries", Details.Select(x => new XElement("Entries",
                //        new XAttribute("Naration", x.Narration),
                //        new XAttribute("Head", (x.HeadName == null) ? "" : x.HeadCode + " - " + x.HeadName),
                //        new XAttribute("Head_Name", x.HeadName),
                //        new XAttribute("Head_Code", x.HeadCode),
                //        new XAttribute("Head_Id", x.HeadId),
                //        new XAttribute("Line", x.Line),
                //        new XAttribute("Qty", 0),
                //        new XAttribute("UOM", ""),
                //        new XAttribute("Rate", 0),
                //        new XAttribute("Debit", x.Debit),
                //        new XAttribute("Credit", x.Credit),
                //        new XAttribute("GroupId", x.TransactionId)
                //        ))).ToString();

                //db.Sp_Add_General_Entries(GeneralEnt, Userid);
            }
            MarkActivity(new EmployeeHistory
            {
                AddedBy_Id = Userid,
                AddedBy_Name = uname,
                AddedOn = DateTime.Now,
                Comment = "Status of arrear of amount has been changed to " + Status,
                CommentType = ActivityType.HR_Arrears.ToString(),
                EmpId = res.Emp_Id,
                EmpName = res.Employee_Name,
                GroupId = new Helpers().RandomNumber()
            });

            return Json(true);
        }
        [HttpPost]
        public JsonResult RemoveArrear(long Id)
        {
            db.Sp_Delete_Arrear(Id);
            return Json(true);
        }
       [NoDirectAccess] public ActionResult PayArrearsView(long? SalariesId)
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            var uname = db.Users.Where(x => x.Id == Userid).Select(x => x.Name).FirstOrDefault();
            var info = db.Sp_Get_Employee_UserId(Userid).SingleOrDefault();
            db.Sp_Add_Arrear_Distributor(info.Name, Userid, SalariesId);
            var res = db.Sp_Get_Arrears_Parameter(SalariesId).SingleOrDefault();
            var res1 = db.Sp_Get_Arrear_Salary_Details(SalariesId).ToList();
            var jvNo = db.Sp_Add_Journal_Voucher().FirstOrDefault();
            if (res.Status != "Received")
            {
                {
                    bool headcashier = true;
                    try
                    {
                        AccountHandlerController de = new AccountHandlerController();
                        de.DisburseArrearSalary(res.Grand_Total, res.Employee_Name, res.Employee_code, "Cash", "", null, "", new Helpers().RandomNumber(), Userid, jvNo, 1, headcashier, COA_Mapper_Modules.Human_Resource.ToString(), null);
                    }
                    catch (Exception ex)
                    {
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    }
                }
                db.Sp_Update_ArrearsStatus_Parameter("Received", SalariesId);
                MarkActivity(new EmployeeHistory
                {
                    AddedBy_Name = uname,
                    AddedBy_Id = Userid,
                    AddedOn = DateTime.Now,
                    Comment = "Arrear of amount " + string.Format("{0:n0}", res.Grand_Total) + " has been received by the employee.",
                    CommentType = ActivityType.HR_Arrears.ToString(),
                    EmpId = res.Emp_Id,
                    EmpName = res.Employee_Name,
                    GroupId = new Helpers().RandomNumber()
                });
            }
            var res2 = new ArrearSalaryDetail { Details = res, ArrearDetail = res1 };
            return View(res2);
        }
        // Get Salary Distributor
       [NoDirectAccess] public ActionResult DistributorDetail(int? Month, int? Year)
        {
            var res = db.Sp_Get_Arrear_Distributor(Month, Year).ToList();
            return PartialView(res);
        }
       [NoDirectAccess] public ActionResult FinanceFinalArrSlipSearch(int Month, int Year)
        {
            ViewBag.Mo = Month;
            ViewBag.Yr = Year;
            return PartialView();
        }
       [NoDirectAccess] public ActionResult DailyDistributorDetail(int? Month, int? Year)
        {
            var res = db.Sp_Get_Arrear_Distributor(Month, Year).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Daily Distribution Page", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return PartialView(res);
        }

       [NoDirectAccess] public ActionResult DistrbutorAndGenerate()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Distributor Page ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return PartialView();
        }
       [NoDirectAccess] public ActionResult SalariesDetail(long? Id)
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            ViewBag.userid = Userid;
            ViewBag.Salaryid = Id;
            var res = db.Sp_Get_Arrear_Salary_Details(Id).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Salaries Details For " + res.Select(x=>x.Description).FirstOrDefault(), "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Id);
            return PartialView(res);

        }
        // deduction and taxes
       [NoDirectAccess] public ActionResult GetAllownceAndBonus(long Id)
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            ViewBag.userid = Userid;
            var res = db.Sp_Get_Arrear_Salary_Details(Id).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Allowance And Bonuses Details ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Id);
            return PartialView(res);
        }
        public JsonResult RemoveAllowncesAndBonus(long Id, long SalaryId)
        {
            db.Sp_Delete_ArrearSalary_Detail_Parameter(Id);
            var ABS = db.Arrear_Salaries_Details.Where(x => x.Salaries_id == SalaryId && x.Type == "AllowncesAndBonus").Select(x => x.Amount).Sum();
            var GrandTotal = db.Arrears.Where(x => x.Id == SalaryId).Select(x => x.Grand_Total).SingleOrDefault();
            var result = new { ABSResult = ABS, GrandSum = GrandTotal };
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Removed Allowance And Bonus " , "Update", "Activity_Record", ActivityType.Arreares_Salaries.ToString(), Id);
            return Json(result);

        }
        public JsonResult AddAllowncesAndBonus(Salaries_Details de)
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_ArrearSalary_Details_Parameter(de.Description, de.Amount, de.Salaries_id, de.Type, Userid);
            var ABS = db.Arrear_Salaries_Details.Where(x => x.Salaries_id == de.Salaries_id && x.Type == "AllowncesAndBonus").Select(x => x.Amount).Sum();
            var GrandTotal = db.Arrears.Where(x => x.Id == de.Salaries_id).Select(x => x.Grand_Total).SingleOrDefault();
            var result = new { ABSResult = ABS, GrandSum = GrandTotal };
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Added Allowance and Bonus ", "Update", "Activity_Record", ActivityType.Arreares_Salaries.ToString(), de.Salaries_id);
            return Json(result);
        }

        // allowncs and bonus
       [NoDirectAccess] public ActionResult GetDeductionAndTaxes(long? Id)
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            ViewBag.userid = Userid;
            var res = db.Sp_Get_Arrear_Salary_Details(Id).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Deduction and Taxes Page ", "Read", "Activity_Record", ActivityType.Arreares_Salaries.ToString(), Id);
            return PartialView(res);
        }
        public JsonResult AddDeductionAndTaxes(Salaries_Details de)
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_ArrearSalary_Details_Parameter(de.Description, (de.Amount * (-1)), de.Salaries_id, de.Type, Userid);
            var DT = db.Arrear_Salaries_Details.Where(x => x.Salaries_id == de.Salaries_id && x.Type == "DeductionAndTaxes").Select(x => x.Amount * (-1)).Sum();
            var GrandTotal = db.Arrears.Where(x => x.Id == de.Salaries_id).Select(x => x.Grand_Total).SingleOrDefault();
            var result = new { DTResult = DT, GrandSum = GrandTotal };
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Added Deductions and Taxes", "Update", "Activity_Record", ActivityType.Arreares_Salaries.ToString(), de.Salaries_id);
            return Json(result);
        }
        public JsonResult RemoveDeductionAndTaxes(long Id, long SalaryId)
        {
            db.Sp_Delete_ArrearSalary_Detail_Parameter(Id);
            var DT = db.Arrear_Salaries_Details.Where(x => x.Salaries_id == SalaryId && x.Type == "DeductionAndTaxes").Select(x => x.Amount * (-1)).Sum();
            var GrandTotal = db.Arrears.Where(x => x.Id == SalaryId).Select(x => x.Grand_Total).SingleOrDefault();
            var result = new { DTResult = DT, GrandSum = GrandTotal };
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Removed Deductions And Taxes ", "Read", "Activity_Record", ActivityType.Arreares_Salaries.ToString(), SalaryId);
            return Json(result);

        }

       [NoDirectAccess] public ActionResult ArrearsManagement()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Arrears Management Page ", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);
            return View();
        }

        private void MarkActivity(EmployeeHistory eh)
        {
            if (eh is null)
                return;
            db.EmployeeHistories.Add(eh);
            db.SaveChanges();
        }
       [NoDirectAccess] public ActionResult ArchiveArrears()
        {
            return PartialView();
        }
       [NoDirectAccess] public ActionResult SearchArrears(DateTime From, string Status)
        {
            var res = db.Sp_Get_ArrearsReport(From, Status).ToList();
            if (res == null || res.Count <= 0)
            {
                res = db.Sp_Get_ArrearsReport(From.AddMonths(-1), Status).ToList();
            }
            return PartialView(res);
        }
    }
}