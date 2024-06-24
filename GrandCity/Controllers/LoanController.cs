using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using MeherEstateDevelopers.Filters;
using Newtonsoft.Json;
using Microsoft.Ajax.Utilities;
using System.Data.Entity;
using static MeherEstateDevelopers.MvcApplication;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class LoanController : Controller
    {
        // GET: Loan and advance
        private Grand_CityEntities db = new Grand_CityEntities();
        [NoDirectAccess] public ActionResult Index()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var info = db.Sp_Get_Employee_UserId(userid).SingleOrDefault();
            ViewBag.IsManager = db.Sp_Get_ManagerStatus(info.Id).Any();
            
            db.Sp_Add_Activity(userid, "Accessed  All Loan Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return View(info);
        }
        // Create
        [NoDirectAccess] public ActionResult Create(long? EmpId)
        {
            var info = db.Sp_Get_Employee_Parameter(EmpId).SingleOrDefault();
            var res = db.MIS_Modules_Configurations.Where(x => x.Module == MIS_Modules.HR.ToString()).Select(x => x.Config_For_Update).FirstOrDefault();
            var cofig = JsonConvert.DeserializeObject<HRConfiguration>(res);
            TimeSpan span = DateTime.Now - Convert.ToDateTime(info.Date_Of_Joining);
            DateTime zeroTime = new DateTime(1, 1, 1);
            int years = (zeroTime + span).Year - 1;
            ViewBag.MaxLoanApply = info.Basic_Salary * years;
            ViewBag.max_inst_limit = cofig.Max_Loan_Allowed;
            ViewBag.max_inst_count = cofig.Max_Installments;
            var taxres = db.MIS_Modules_Configurations.Where(x => x.Module == MIS_Modules.Salary_Tax.ToString()).FirstOrDefault();
            ViewBag.Tax = 0;
            if (taxres != null)
            {
                List<SalaryTaxEmployee> stm = JsonConvert.DeserializeObject<List<SalaryTaxEmployee>>(taxres.CurrentConfig);
                ViewBag.Tax = stm.Where(x => x.EmpCode == info.Employee_Code).Select(x => x.TaxAmount).FirstOrDefault();
            }
            var dat = DateTime.Now;
            ViewBag.loan = db.Loan_Installments.Where(x => x.Emp_Id == info.Id && x.Status == "Pending" && x.Date <= dat && x.IsWaivedOff == null && x.Loan_Id != -1).Select(x => x.Loan_Amt).Sum() ?? 0;
            ViewBag.salary = 0;
            if (dat.Day > 20)
            {
                ViewBag.salary = 1;
            }
            return PartialView(info);
        }
        // Loan For others
        [NoDirectAccess] public ActionResult LoanForOther()
        {
            var res = db.MIS_Modules_Configurations.Where(x => x.Module == MIS_Modules.HR.ToString()).Select(x => x.Config_For_Update).FirstOrDefault();
            var cofig = JsonConvert.DeserializeObject<HRConfiguration>(res);
            ViewBag.max_inst_limit = cofig.Max_Loan_Allowed;
            ViewBag.max_inst_count = cofig.Max_Installments;
            long userid = long.Parse(User.Identity.GetUserId());
            ViewBag.Employee = new SelectList(db.Sp_Get_Employee().Select(x => new { x.Id, Name = x.Name + " (" + x.Employee_Code + " )", x.Department }).ToList(), "Id", "Name", "Department", 1);
            
            return PartialView();
        }
        // Balance Details
        [NoDirectAccess] public ActionResult BalanceDetail()
        {
            return View();
        }
        [NoDirectAccess] public ActionResult Summary()
        {
            var res = db.Sp_Get_LoanSummary().ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Loan Summary Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult Payables(string Status)
        {
            ViewBag.status = Status;
            var res = db.Sp_Get_Payables_Detail(Status).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Loan Payables Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView(res);
        }
        // Archive Or Receivable
        [NoDirectAccess] public ActionResult ArchiveOrReceivable(string Status)
        {
            ViewBag.status = Status;
            var res = db.Sp_Get_Payable_ArchiveOrReceivable(Status).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Loan Archived And Reciveables Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView(res);
        }
        
        [NoDirectAccess] public ActionResult DirectLoan()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            ViewBag.Employee = new SelectList(db.Sp_Get_Employee().ToList(), "Id", "Name", "Department", 1);
            return View();
        }
        // loan table show
        [NoDirectAccess] public ActionResult LoanInstallmentsGeneration(int Installments, int Amt, string Reason, long EmpId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            EmpModel result = db.Sp_Get_Employee_Parameter(EmpId).Select(z => new EmpModel
            {
                Id = z.Id,
                Name = z.Name,
                Employee_Code = z.Employee_Code,
                Basic_Salary = z.Basic_Salary,
                Installment_Values = Installments,
                Date = DateTime.Now,
                Amount = Amt
            }).SingleOrDefault();
            return PartialView(result);
        }
        // loan creation
        [HttpPost]
        public JsonResult LoanCreation(long EmpId, string Reason, int? InstVal, decimal? Basic_salary, int? inst, string Option)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var uname = db.Users.Where(x => x.Id == userid).Select(x => x.Name).FirstOrDefault();
            var emp = db.Employees.Where(x => x.Id == EmpId).Select(x => x.Name).FirstOrDefault();
            var res1 = db.Sp_Add_Loan(Reason, userid, LoanStatus.Pending.ToString(), LoanStatus.Pending.ToString(), inst, InstVal, EmpId, Option, null, null, null, null).FirstOrDefault();
            MarkActivity(new EmployeeHistory
            {
                AddedBy_Id = userid,
                AddedBy_Name = uname,
                AddedOn = DateTime.Now,
                EmpId = EmpId,
                EmpName = emp,
                CommentType = ActivityType.HR_LoanApplied.ToString(),
                Comment = Option + " application has been received for amount : " + (InstVal).ToString() + ". Reason : " + Reason
            });
            return Json(res1);
        }
        //Direct Loan Creation
        [HttpPost]
        public JsonResult DirectLoanCreation(long EmpId, string Reason, int? InstVal, decimal? Basic_salary, int? inst, string Option)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var uname = db.Users.Where(x => x.Id == userid).Select(x => x.Name).FirstOrDefault();
            var emp = db.Employees.Where(x => x.Id == EmpId).Select(x => x.Name).FirstOrDefault();
            var InstallAmt = inst / InstVal;
            List<Loan_Installments> Linst = new List<Loan_Installments>();
            for (int i = 0; i < InstVal; i++)
            {
                DateTime date = new DateTime(DateTime.Now.AddMonths(i).Year, DateTime.Now.AddMonths(i).Month, 1);
                Loan_Installments li = new Loan_Installments()
                {
                    Loan_Amt = InstallAmt,
                    Date = date
                };
                Linst.Add(li);
            }
            var Installments = new XElement("Loan", Linst.Select(x => new XElement("LoanInst",
               new XAttribute("Date", x.Date),
               new XAttribute("Loan_Amt", x.Loan_Amt)
               ))).ToString();
            var res1 = db.Sp_Add_Loan(Reason, userid, LoanStatus.Pending.ToString(), LoanStatus.Pending.ToString(), inst, InstVal, EmpId, Option, null, null, null, null).FirstOrDefault();
            MarkActivity(new EmployeeHistory
            {
                AddedBy_Id = userid,
                AddedBy_Name = uname,
                AddedOn = DateTime.Now,
                EmpId = EmpId,
                EmpName = emp,
                CommentType = ActivityType.HR_LoanApplied.ToString(),
                Comment = Option + " application has been received for amount : " + (InstVal).ToString() + ". Reason : " + Reason
            });
            return Json(res1);
        }
        //popup for pendings
        [HttpPost]
        [NoDirectAccess] public ActionResult ManagerPendingLoanDetails(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_LoanDetails(Id).SingleOrDefault();
            var insts = db.Loan_Installments.Where(x => x.Loan_Id == res.Id).ToList();
            return PartialView(new LoanApprovalView { InstallmentsStructure = insts, LoanoverView = res });
        }
        //popup for pendings
        [HttpPost]
        [NoDirectAccess] public ActionResult HRPendingLoanDetails(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_LoanDetails(Id).SingleOrDefault();
            var insts = db.Loan_Installments.Where(x => x.Loan_Id == res.Id).ToList();
            return PartialView(new LoanApprovalView { InstallmentsStructure = insts, LoanoverView = res });
        }
        public JsonResult MangUpdateStatus(long Id, string Manger_Remarks, string Manger_Status)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var loan = db.LoanRequisitions.Where(x => x.Id == Id).FirstOrDefault();
            var uname = db.Users.Where(x => x.Id == userid).Select(x => x.Name).FirstOrDefault();
            var emp = db.Employees.Where(x => x.Id == loan.Emp_Id).Select(x => x.Name).FirstOrDefault();
            db.Sp_Update_LoanStatus_Manager(Id, Manger_Remarks, Manger_Status, userid);
            MarkActivity(new EmployeeHistory
            {
                AddedBy_Id = userid,
                AddedBy_Name = uname,
                AddedOn = DateTime.Now,
                EmpId = loan.Emp_Id,
                EmpName = emp,
                CommentType = ActivityType.HR_LoanStatusChanged.ToString(),
                Comment = "Manager changed status of loan application to " + Manger_Status + ". Remarks : " + Manger_Remarks
            });
            return Json(true);
        }
        public JsonResult HRUpdateStatus(long Id, string Manger_Remarks, string Manger_Status, string HR_Remarks, string HR_Status, bool Check)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var loan = db.LoanRequisitions.Where(x => x.Id == Id).FirstOrDefault();
            var uname = db.Users.Where(x => x.Id == userid).Select(x => x.Name).FirstOrDefault();
            var emp = db.Employees.Where(x => x.Id == loan.Emp_Id).Select(x => x.Name).FirstOrDefault();
            if (User.IsInRole("HR Loan Approval"))
            {
                db.Sp_Update_LoanStatus_HR(Id, HR_Remarks, HR_Status, userid);
                MarkActivity(new EmployeeHistory
                {
                    AddedBy_Id = userid,
                    AddedBy_Name = uname,
                    AddedOn = DateTime.Now,
                    EmpId = loan.Emp_Id,
                    EmpName = emp,
                    CommentType = ActivityType.HR_LoanStatusChanged.ToString(),
                    Comment = "HR changed status of loan application to " + HR_Status + ". Remarks : " + HR_Remarks
                });
            }
            if (Check)
            {
                db.Sp_Update_LoanStatus_Manager(Id, Manger_Remarks, Manger_Status, userid);
                MarkActivity(new EmployeeHistory
                {
                    AddedBy_Id = userid,
                    AddedBy_Name = uname,
                    AddedOn = DateTime.Now,
                    EmpId = loan.Emp_Id,
                    EmpName = emp,
                    CommentType = ActivityType.HR_LoanStatusChanged.ToString(),
                    Comment = "Manager changed status of loan application to " + Manger_Status + ". Remarks : " + Manger_Remarks
                });
            }
            return Json(true);
        }
        // Employee
        [NoDirectAccess] public ActionResult EmployeeApproved(long EmpId)
        {
            var res = db.Sp_Get_EmployeeLoan_Management(LoanStatus.Approved.ToString(), LoanStatus.Approved.ToString(), EmpId).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Approved Loan of Employees ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult EmployeePending(long EmpId)
        {
            var res = db.Sp_Get_EmployeeLoan_Management(LoanStatus.Pending.ToString(), LoanStatus.Pending.ToString(), EmpId).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Pending Loan of Employees ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult EmployeeRejected(long EmpId)
        {
            var res = db.Sp_Get_EmployeeLoan_Management(LoanStatus.Rejected.ToString(), LoanStatus.Rejected.ToString(), EmpId).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Rejected Loan of Employees ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView(res);
        }
        // manager
        [NoDirectAccess] public ActionResult ManagerPending()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var Empid = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
            var result = db.Sp_Get_LoanManagement_Manager(Empid, LoanStatus.Pending.ToString()).ToList();
            return PartialView(result);
        }
        [NoDirectAccess] public ActionResult ActingManagerPending()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var Empid = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
            var result = db.LoanRequisitions.Where(x => x.ManagerApproval == LoanStatus.Pending.ToString()).ToList();
            return PartialView(result);
        }
        [NoDirectAccess] public ActionResult ManagerApproved()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var Empid = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
            var result = db.Sp_Get_LoanManagement_Manager(Empid, LoanStatus.Approved.ToString()).ToList();
            return PartialView(result);
        }
        [NoDirectAccess] public ActionResult ManagerRejected()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var Empid = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
            var result = db.Sp_Get_LoanManagement_Manager(Empid, LoanStatus.Rejected.ToString()).ToList();
            return PartialView(result);
        }
        // HR
        [NoDirectAccess] public ActionResult HRPending()
        {
            var res = db.Sp_Get_LoanManagement_HR(LoanStatus.Pending.ToString()).ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult HRApproved()
        {
            var res = db.Sp_Get_LoanManagement_HR(LoanStatus.Approved.ToString()).ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult HRRejected()
        {
            var res = db.Sp_Get_LoanManagement_HR(LoanStatus.Rejected.ToString()).ToList();
            return PartialView(res);
        }
        // Other
        [NoDirectAccess] public ActionResult OtherPending()
        {
            var res = db.Sp_Get_LoanManagement_Other(LoanStatus.Pending.ToString()).ToList();
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult OtherApproved()
        {
            var res = db.Sp_Get_LoanManagement_Other(LoanStatus.Approved.ToString()).ToList();
            return PartialView("OtherPending", res);
        }
        [NoDirectAccess] public ActionResult OtherRejected()
        {
            var res = db.Sp_Get_LoanManagement_Other(LoanStatus.Rejected.ToString()).ToList();
            return PartialView("OtherPending", res);
        }
        //Loan Listing
        [NoDirectAccess] public ActionResult LoanList()
        {
            var res = db.Sp_Get_LoanOrAdvanceList(LoanStatus.Loan.ToString()).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Loan List ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult AdvanceList()
        {
            var res = db.Sp_Get_LoanOrAdvanceList(LoanStatus.Advance_Salary.ToString()).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Accessed Advance List ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
            return PartialView(res);
        }
        [NoDirectAccess] public ActionResult LoanVoucherDetails(long Id)
        {
            var res = db.Sp_Get_LoanDetails(Id).SingleOrDefault();
            return PartialView(res);
        }
        public JsonResult LoanAdvanceVoucher(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var uname = db.Users.Where(x => x.Id == userid).Select(x => x.Name).FirstOrDefault();
            var res1 = db.Sp_Get_LoanDetails(Id).SingleOrDefault();
            if (res1.Paid_Status != true)
            {
                var res2 = db.Sp_Get_Employee_Parameter(res1.Emp_Id).SingleOrDefault();
                using (var Transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        string Remarks = (res1.Type == LoanStatus.Loan.ToString()) ?
                        res1.Type + " for Domestic use of Amount : " + string.Format("{0:n0}", res1.Amount) + " For Total " + res1.Installments + " Installments & Rs: " + string.Format("{0:n0}", Math.Round(Convert.ToDecimal(res1.Amount / res1.Installments))) + "Monthly Installment" :
                        res1.Type + " Salary for the Month" + string.Format("{0: MMMM yyyy}", DateTime.Now);
                        var res = db.Sp_Add_Voucher(res2.Department_Designation, res1.Amount, GeneralMethods.NumberToWords(Convert.ToInt32(res1.Amount)),
                            null, null, null, null, res2.Mobile_1, Remarks, res2.Father_Name, res1.Id, res1.Type, res2.Name, "Cash", null,
                              "", userid, res1.Type, userid, null, comp.Id).FirstOrDefault();
                        db.Sp_Update_LoanStatus(res1.Id);
                        MarkActivity(new EmployeeHistory
                        {
                            AddedBy_Id = userid,
                            AddedBy_Name = uname,
                            AddedOn = DateTime.Now,
                            Comment = "Loan of amount " + string.Format("{0:n0}", res1.Amount) + " and " + res1.Installments + " installments has been received by the employee.",
                            EmpId = res2.Id,
                            EmpName = res2.Name,
                            GroupId = new Helpers().RandomNumber(),
                            CommentType = ActivityType.HR_LoanStatusChanged.ToString()
                        });
                        bool headcashier = false;
                        if (User.IsInRole("Head Cashier"))
                        {
                            headcashier = true;
                        }
                        try
                        {
                            AccountHandlerController de = new AccountHandlerController();
                            de.Loan_Generate_Loan(res1.Amount, res1.Type, res2.Name, res2.CNIC, res2.Mobile_1, "Cash", null, null, null, "Loan received", new Helpers().RandomNumber(), userid, res.Receipt_No, 1, headcashier);
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        }
                        Transaction.Commit();
                        var fres = new { VoucherId = res.Receipt_Id, Token = userid };
                        return Json(fres);
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        var fres = new { VoucherId = false, Token = "Voucher couldn't Generated. Please try later." };
                        return Json(fres);
                    }
                }
            }
            else
            {
                var fres = new { VoucherId = false, Token = "Voucher Already Generated" };
                return Json(fres);
            }
        }
        [NoDirectAccess] public ActionResult LoanDetails(long Id)
        {
            var res1 = db.Sp_Get_LoanDetails(Id).SingleOrDefault();
            var res2 = db.Sp_Get_LoanInstallments(Id).ToList();
            var res = new LoansDetails { Loan = res1, Installments = res2 };
            return PartialView(res);
        }
        public JsonResult SaveLoanApplication(InstallmentStatusModel[] installments, int plan, string rzn, int loanAmt, long emp, string tp)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var uname = db.Users.Where(x => x.Id == userid).Select(x => x.Name).FirstOrDefault();
            var ManagerData = db.Employees.Where(x => x.loginId == userid).Select(x => new { x.Id, x.Name }).FirstOrDefault();
            var isManager = db.Employees.Any(x => x.Id == emp && (x.Reporting_To == ManagerData.Id || x.Reporting_To_2 == ManagerData.Id));
            var empName = db.Employees.Where(x => x.Id == emp).Select(x => x.Name).FirstOrDefault();
            if (installments is null)
            {
                return Json(new { Status = false, Msg = "Error!! Installments structure not found. Loan application not saved" });
            }
            else if (installments.Length < (plan - 1))
            {
                return Json(new { Status = false, Msg = "Error!! Installments structure is missing some data. Please try again." });
            }
            if (plan == -2)
                plan = installments.Length;
            if (plan > 0)
            {
                //equal installments plan
                long? loanId = null;
                if (User.IsInRole("HR Manager") || User.IsInRole("Administrator"))
                {
                    loanId = db.Sp_Add_Loan(rzn, userid, LoanStatus.Pending.ToString(), LoanStatus.Pending.ToString(), loanAmt, plan, emp, tp, null, null, userid, uname).FirstOrDefault();
                }
                else if (isManager)
                {
                    loanId = db.Sp_Add_Loan(rzn, userid, LoanStatus.Pending.ToString(), LoanStatus.Pending.ToString(), loanAmt, plan, emp, tp, userid, uname, null, null).FirstOrDefault();
                }
                else
                {
                    loanId = db.Sp_Add_Loan(rzn, userid, LoanStatus.Pending.ToString(), LoanStatus.Pending.ToString(), loanAmt, plan, emp, tp, null, null, null, null).FirstOrDefault();
                }
                if (loanId is null)
                {
                    return Json(new { Status = false, Msg = "Error!! Unable to process loan request at the moment. Please try again." });
                }
                //yahan pe us loan ki per month entries krni hain
                foreach (var v in installments)
                {
                    string[] dt_Spltd = v.Status.Split('/');
                    int mon = ConvertToMonth(dt_Spltd[0]);
                    int yr = int.Parse(dt_Spltd[1]);
                    Loan_Installments li = new Loan_Installments
                    {
                        Date = new DateTime(yr, mon, 1),
                        Emp_Id = emp,
                        Loan_Amt = v.Id,
                        Loan_Id = loanId,
                        Paid_Amt = 0,
                        Status = LoanStatus.Pending.ToString()
                    };
                    db.Loan_Installments.Add(li);
                    db.SaveChanges();
                }
            }
            else if (plan == -1)
            {
                //custom plan
                //lets populate the lona installments list first so that we can perform check easily
                List<Loan_Installments> processed_installments = new List<Loan_Installments>();
                foreach (var v in installments)
                {
                    string[] dt_Spltd = v.Status.Split('/');
                    int mon = int.Parse(dt_Spltd[0]);
                    int yr = int.Parse(dt_Spltd[1]);
                    DateTime __temp_Dt = new DateTime(yr, mon, 1);
                    if (processed_installments.Any(x => x.Date.Value.Month == __temp_Dt.Month && x.Date.Value.Year == __temp_Dt.Year))
                    {
                        return Json(new { Status = false, Msg = "Error!! Two installments found on the same month. Please check your installment months and try again." });
                    }
                    processed_installments.Add(new Loan_Installments
                    {
                        Date = __temp_Dt,
                        Emp_Id = emp,
                        Loan_Amt = v.Id,
                        Paid_Amt = 0,
                        Status = LoanStatus.Pending.ToString()
                    });
                }
                if (processed_installments.Sum(x => x.Loan_Amt) != loanAmt)
                {
                    return Json(new { Status = false, Msg = "Error!! Installments amounts should sum to original loan amount. Please check loan amounts and try again." });
                }
                long? loanId = null;
                if (User.IsInRole("HR Manager") || User.IsInRole("Administrator"))
                {
                    loanId = db.Sp_Add_Loan(rzn, userid, LoanStatus.Pending.ToString(), LoanStatus.Pending.ToString(), loanAmt, plan, emp, "Loan", null, null, userid, uname).FirstOrDefault();
                }
                else if (isManager)
                {
                    loanId = db.Sp_Add_Loan(rzn, userid, LoanStatus.Pending.ToString(), LoanStatus.Pending.ToString(), loanAmt, plan, emp, "Loan", userid, uname, null, null).FirstOrDefault();
                }
                else
                {
                    loanId = db.Sp_Add_Loan(rzn, userid, LoanStatus.Pending.ToString(), LoanStatus.Pending.ToString(), loanAmt, plan, emp, "Loan", null, null, null, null).FirstOrDefault();
                    if (loanId != null)
                    {
                        //notification Generation
                        var empInfo = db.Employees.Where(x => x.Id == emp).FirstOrDefault();
                        string noti_text = empInfo.Name + " has applied for company loan for amount : " + loanAmt + " with " + plan + " installments.";
                        //Notifier.DispatchNotification(noti_text, new List<NotificationReceivers> { new NotificationReceivers { notifyTo = NotifyTo.HR_Manager, DataInfo = "Role" }, new NotificationReceivers { notifyTo = NotifyTo.ReportingManager, DataInfo = "Role", ExtraId = emp } }, User.Identity.GetUserId<long>(), MISModuleType.Human_Resource, (long)loanId, string.Empty, new Helpers().RandomNumber(), emp, empName);
                    }
                }
                if (loanId is null)
                {
                    return Json(new { Status = false, Msg = "Error!! Unable to process loan request at the moment. Please try again." });
                }
                processed_installments.ForEach(x => x.Loan_Id = loanId);
                db.Loan_Installments.AddRange(processed_installments);
                db.SaveChanges();
            }
            MarkActivity(new EmployeeHistory
            {
                AddedBy_Id = userid,
                AddedBy_Name = uname,
                AddedOn = DateTime.Now,
                EmpId = emp,
                EmpName = empName,
                CommentType = ActivityType.HR_LoanApplied.ToString(),
                Comment = tp + " application has been received for amount : " + (loanAmt).ToString() + ". Reason : " + rzn
            });
            return Json(new { Status = true, Msg = "Loan application has been placed successfully." });
        }
        private int ConvertToMonth(string mon)
        {
            if (mon == "Jan")
            {
                return 1;
            }
            else if (mon == "Feb")
            {
                return 2;
            }
            else if (mon == "Mar")
            {
                return 3;
            }
            else if (mon == "Apr")
            {
                return 4;
            }
            else if (mon == "May")
            {
                return 5;
            }
            else if (mon == "Jun")
            {
                return 6;
            }
            else if (mon == "Jul")
            {
                return 7;
            }
            else if (mon == "Aug")
            {
                return 8;
            }
            else if (mon == "Sep")
            {
                return 9;
            }
            else if (mon == "Oct")
            {
                return 10;
            }
            else if (mon == "Nov")
            {
                return 11;
            }
            else if (mon == "Dec")
            {
                return 12;
            }
            return 1;
        }
        [NoDirectAccess] public ActionResult EmployeeLoanHistory(int Id)
        {
            var loanReqs = db.LoanRequisitions.Where(x => x.Emp_Id == Id).ToList();
            List<long> loanIds = loanReqs.Select(x => (long)x.Id).ToList();
            var installments = db.Loan_Installments.Where(x => loanIds.Contains((long)x.Loan_Id)).ToList();
            return PartialView(new EmployeeProfileLoanView { Requisitions = loanReqs, Installments = installments });
        }
        [NoDirectAccess] public ActionResult UpdateLoanStructure(long Id)
        {
            var loanData = db.LoanRequisitions.Where(x => x.Id == Id).FirstOrDefault();
            var inst = db.Loan_Installments.Where(x => x.Loan_Id == Id).ToList();
            return PartialView(new LoanView { LoanInfo = loanData, Installments = inst });
        }
        public JsonResult UpdateStructure(LoanInstallmentModel[] instalments, long loanId)
        {
            try
            {
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                var loanData = db.LoanRequisitions.Where(x => x.Id == loanId).FirstOrDefault();
                DateTime currDate = DateTime.Now;
                if (instalments.Sum(x => x.Amount) != loanData.Amount)
                {
                    return Json(new { Msg = "Amounts donot sum up to original loan amount.", Status = false });
                }
                foreach (var v in instalments)
                {
                    string[] prsed = v.InstallmentMonth.Split('/');
                    DateTime dt = new DateTime(int.Parse(prsed[1]), int.Parse(prsed[0]), 1);
                    v.InstMonth = dt;
                }
                foreach (var v in instalments.Where(x => x.IsPaid == false))
                {
                    if (instalments.Where(x => x.InstMonth.Value.Month == v.InstMonth.Value.Month && x.InstMonth.Value.Year == v.InstMonth.Value.Year).Count() > 1)
                    {
                        return Json(new { Msg = "Multiple instalments with same month found. Please assure that no two instalments have the same month.", Status = false });
                    }
                    else if (v.IsPaid == false && (v.InstMonth.Value.Month < currDate.Month && v.InstMonth.Value.Year <= currDate.Year))
                    {
                        return Json(new { Msg = "Cannot set a past date for a non paid instalment.", Status = false });
                    }
                }
                List<Loan_Installments> newStructure = new List<Loan_Installments>();
                foreach (var v in instalments.Where(x => x.IsPaid == false))
                {
                    if (v.Id == -1)
                    {
                        Loan_Installments li = new Loan_Installments
                        {
                            Date = v.InstMonth.Value,
                            Emp_Id = loanData.Emp_Id,
                            Loan_Amt = v.Amount,
                            Loan_Id = loanId,
                            Paid_Amt = 0,
                            Status = "Pending"
                        };
                        newStructure.Add(li);
                    }
                    else
                    {
                        var inst = db.Loan_Installments.Where(x => x.Id == v.Id).FirstOrDefault();
                        Loan_Installments li = new Loan_Installments
                        {
                            Date = v.InstMonth.Value,
                            Emp_Id = inst.Emp_Id,
                            Loan_Amt = v.Amount,
                            Loan_Id = loanId,
                            Paid_Amt = 0,
                            Status = "Pending"
                        };
                        newStructure.Add(li);
                    }
                    //inst.Loan_Amt = v.Amount;
                    //inst.Date = v.InstMonth.Value;
                    //db.Loan_Installments.Attach(inst);
                    //db.Entry(inst).Property(x => x.Date).IsModified = true;
                    //db.Entry(inst).Property(x => x.Loan_Amt).IsModified = true;
                    //db.SaveChanges();
                }
                //all existing pending loan structure has been deleted.
                db.Sp_Delete_Pending_Loan_Installments(loanId);
                //save new structrue
                db.Loan_Installments.AddRange(newStructure);
                db.SaveChanges();
                MarkActivity(new EmployeeHistory
                {
                    AddedBy_Id = uid,
                    AddedBy_Name = uname,
                    AddedOn = DateTime.Now,
                    Comment = "Loan structure has been updated. Loan was applied on :" + loanData.CreatedAt.Value.ToShortDateString() + " and Loan amount was : " + string.Format("{0:n0}", loanData.Amount),
                    CommentType = ActivityType.HR_LoanStatusChanged.ToString(),
                    EmpId = loanData.Emp_Id,
                    EmpName = loanData.Employee_Name,
                    GroupId = new Helpers().RandomNumber()
                });
                return Json(new { Msg = "Installments structure has been updated successfuly.", Status = true });
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
        [NoDirectAccess] public ActionResult LoanReport()
        {
            var loans = db.LoanRequisitions.Where(x => x.HrApproval.Contains("Ap") && x.ManagerApproval.Contains("Ap")).ToList();
            var filteredIds = loans.Select(x => x.Id).ToList();
            var installments = db.Loan_Installments.Where(x => filteredIds.Contains((long)x.Loan_Id)).ToList();
            return View(new LoanReportView { InstallmentsData = installments, LoansData = loans });
        }
        [NoDirectAccess] public ActionResult LoanWaiveOff(long instId, long display)
        {
            var loanInstData = db.Loan_Installments.Where(x => x.Id == instId).FirstOrDefault();
            var loanReqs = db.LoanRequisitions.Where(x => x.Id == loanInstData.Loan_Id).FirstOrDefault();
            var loanInstsAll = db.Loan_Installments.Where(x => x.Id == instId).FirstOrDefault();
            var EmpDetails = db.Sp_GetEmpCompleteDesigs(loanReqs.Emp_Id).ToList();
            ViewBag.display = display;
            ViewBag.LoanAmount = loanInstData.Loan_Amt;
            ViewBag.LoanInstDate = loanInstData.Date;
            return PartialView(new LoanCompleteDetails { EmployeeDetails = EmpDetails, InstallmentsInfo = loanInstsAll, LoanData = loanReqs });
        }
        public JsonResult SaveLoanWaiveRequest(long instId, string reason)
        {
            var uid = User.Identity.GetUserId<long>();
            var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
            var loanInstData = db.Loan_Installments.Where(x => x.Id == instId).FirstOrDefault();
            var loanReqData = db.LoanRequisitions.Where(x => x.Id == loanInstData.Loan_Id).FirstOrDefault();
            var empDets = db.Employees.Where(x => x.Id == loanReqData.Emp_Id).Select(x => new { x.Employee_Code, x.Basic_Salary, x.Date_Of_Joining }).FirstOrDefault();
            try
            {
                LoanWaiverRequest lwr = new LoanWaiverRequest
                {
                    DescriptionText = reason,
                    EmployeeCode = empDets.Employee_Code,
                    EmployeeName = loanReqData.Employee_Name,
                    LoanTotalAmount = (decimal)loanReqData.Amount,
                    Urgency = UrgencyStatus.Normal,
                    WaiverAmount = (decimal)loanInstData.Loan_Amt,
                    Inst_Id = instId
                };
                MIS_Requests rqst = new MIS_Requests
                {
                    CreatedAt = DateTime.Now,
                    CreatedBy_Id = uid,
                    CreatedBy_Name = uname,
                    ModuleId = instId,
                    ModuleType = "Loan Waiver",
                    Status = "Pending",
                    Type = "Loans",
                    Details = JsonConvert.SerializeObject(lwr)
                };
                db.MIS_Requests.Add(rqst);
                db.SaveChanges();
                MarkActivity(new EmployeeHistory
                {
                    AddedBy_Id = uid,
                    AddedBy_Name = uname,
                    AddedOn = DateTime.Now,
                    Comment = "Loan waiver request of Amount " + string.Format("{0:n0}", loanInstData.Loan_Amt) + " Submitted",
                    CommentType = "Text",
                    EmpId = loanInstData.Emp_Id,
                    EmpName = lwr.EmployeeName,
                    GroupId = new Helpers().RandomNumber(),
                });
                return Json(new SimpleReturnMessage { Msg = "Loan waiver request has been submitted successfully", Status = true, _Log = string.Empty });
            }
            catch (Exception ex)
            {
                return Json(new SimpleReturnMessage { Msg = "Failed to save waiver request. Please try again.", Status = false, _Log = "Message : " + ex.Message });
            }
        }
        public JsonResult ApproveRequest(long reqId)
        {
            try
            {
                var reqData = db.MIS_Requests.Where(x => x.Id == reqId).FirstOrDefault();
                LoanWaiverRequest lwr = JsonConvert.DeserializeObject<LoanWaiverRequest>(reqData.Details);
                var instData = db.Loan_Installments.Where(x => x.Id == reqData.ModuleId).FirstOrDefault();
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                if (User.IsInRole("Finance Manager"))
                {
                    lwr.FinanceManagerApproval = true;
                    lwr.FinanceManagerName = uname;
                    lwr.FinanceManager_Id = uid;
                    lwr.MgrStatusChangedAt = DateTime.Now;
                    reqData.Details = JsonConvert.SerializeObject(lwr);
                    db.MIS_Requests.Attach(reqData);
                    db.Entry(reqData).Property(x => x.Details).IsModified = true;
                    db.SaveChanges();
                }
                else if (User.IsInRole("HR Manager") || User.IsInRole("Administrator"))
                {
                    lwr.HrMgrApproval = true;
                    lwr.HrMgrName = uname;
                    lwr.HRMgr_Id = uid;
                    lwr.HrStatusChangedAt = DateTime.Now;
                    reqData.Details = JsonConvert.SerializeObject(lwr);
                    db.MIS_Requests.Attach(reqData);
                    db.Entry(reqData).Property(x => x.Details).IsModified = true;
                    db.SaveChanges();
                }
                MarkActivity(new EmployeeHistory
                {
                    AddedBy_Id = uid,
                    AddedBy_Name = uname,
                    AddedOn = DateTime.Now,
                    Comment = "Loan waiver request of Amount " + string.Format("{0:n0}", instData.Loan_Amt) + " has been approved. Total loan amount : " + string.Format("{0:n0}", lwr.LoanTotalAmount),
                    CommentType = "Text",
                    EmpId = instData.Emp_Id,
                    EmpName = lwr.EmployeeName,
                    GroupId = new Helpers().RandomNumber(),
                });
                if (lwr.HrMgrApproval == true && lwr.FinanceManagerApproval == true)
                {
                    reqData.Status = "Voucher";
                    instData.Reason = lwr.DescriptionText;
                    db.Loan_Installments.Attach(instData);
                    db.Entry(instData).Property(x => x.Reason).IsModified = true;
                    db.SaveChanges();
                    MarkActivity(new EmployeeHistory
                    {
                        AddedBy_Id = 0,
                        AddedBy_Name = "MIS",
                        AddedOn = DateTime.Now,
                        Comment = "Loan waiver request of Amount " + string.Format("{0:n0}", instData.Loan_Amt) + " has been approved by all authorities and the Voucher is Ready For Print.",
                        CommentType = "Text",
                        EmpId = instData.Emp_Id,
                        EmpName = lwr.EmployeeName,
                        GroupId = new Helpers().RandomNumber(),
                    });
                }
                return Json(new SimpleReturnMessage { Msg = "Status has been updated successfully", Status = true, _Log = string.Empty });
            }
            catch (Exception ex)
            {
                return Json(new SimpleReturnMessage { Msg = "Failed to update status due to some error. Please try again.", Status = false, _Log = "Message : " + ex.Message + " --------------%%%%%%%%%%%%---------- Inner Exception " + ex.InnerException });
            }
        }
        [NoDirectAccess] public ActionResult PrintLoanVoucher()
        {
            var uid = User.Identity.GetUserId<long>();
            var res = db.Sp_Get_MIS_Requests(uid, "Voucher").ToList();
            foreach (var v in res)
            {
                if (v.ModuleType == "Loan Waiver")
                {
                    v.WaiverDetails = JsonConvert.DeserializeObject<LoanWaiverRequest>(v.Details);
                }
            }
            return PartialView(res);
        }
        public JsonResult GenerateLoanVoucher(long reqId)
        {
            var reqData = db.MIS_Requests.Where(x => x.Id == reqId).FirstOrDefault();
            LoanWaiverRequest lwr = JsonConvert.DeserializeObject<LoanWaiverRequest>(reqData.Details);
            var instData = db.Loan_Installments.Where(x => x.Id == reqData.ModuleId).FirstOrDefault();
            var emp = db.Employees.Where(x => x.Id == instData.Emp_Id).FirstOrDefault();
            var uid = User.Identity.GetUserId<long>();
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(uid);
            var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
            Helpers h = new Helpers();
            var TransactionId = h.RandomNumber();
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var voch = db.Sp_Add_Voucher(null, instData.Loan_Amt, GeneralMethods.NumberToWords(Convert.ToInt32(instData.Loan_Amt)), null, null, null, null, emp.Mobile_1,
                        string.Join(" , ", lwr.DescriptionText, instData.Date.Value.ToLongDateString()), emp.Father_Name, reqId, Modules.LoanManagement.ToString(), emp.Name, "Cash", null, null, TransactionId, "Loan Installment Voucher", uid, null, comp.Id).SingleOrDefault();
                    bool headcashier = false;
                    if (User.IsInRole("Head Cashier"))
                    {
                        headcashier = true;
                    }
                    try
                    {
                        AccountHandlerController de = new AccountHandlerController();
                        de.Loan_Generate_Loan(instData.Loan_Amt, emp.Employee_Code, emp.Name, emp.CNIC, emp.Mobile_1, "Cash", null, null, null, "Loan received", TransactionId, uid, voch.Receipt_No, 1, headcashier);
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    }
                    reqData.Status = "Approved";
                    instData.IsWaivedOff = true;
                    instData.Status = "Paid";
                    db.Loan_Installments.Attach(instData);
                    db.Entry(instData).Property(x => x.IsWaivedOff).IsModified = true;
                    db.Entry(instData).Property(x => x.Status).IsModified = true;
                    db.SaveChanges();
                    MarkActivity(new EmployeeHistory
                    {
                        AddedBy_Id = 0,
                        AddedBy_Name = "MIS",
                        AddedOn = DateTime.Now,
                        Comment = "Loan waiver request of Amount " + string.Format("{0:n0}", instData.Loan_Amt) + " has been approved by all authorities and The amount is Waived Off.",
                        CommentType = "Text",
                        EmpId = instData.Emp_Id,
                        EmpName = lwr.EmployeeName,
                        GroupId = new Helpers().RandomNumber(),
                    });
                    Transaction.Commit();
                    return Json(new { Status = true, VoucherNo = voch.Receipt_Id, Token = TransactionId });
                }
                catch (Exception e)
                {
                    Transaction.Rollback();
                    return Json(new Return { Status = false, Msg = "Error Occured" });
                }
            }
        }
        public JsonResult RejectRequest(long reqId)
        {
            try
            {
                var reqData = db.MIS_Requests.Where(x => x.Id == reqId).FirstOrDefault();
                LoanWaiverRequest lwr = JsonConvert.DeserializeObject<LoanWaiverRequest>(reqData.Details);
                var instData = db.Loan_Installments.Where(x => x.Id == reqData.ModuleId).FirstOrDefault();
                var uid = User.Identity.GetUserId<long>();
                var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
                if (User.IsInRole("Finance Manager"))
                {
                    lwr.FinanceManagerApproval = false;
                    lwr.FinanceManagerName = uname;
                    lwr.FinanceManager_Id = uid;
                    lwr.MgrStatusChangedAt = DateTime.Now;
                    reqData.Details = JsonConvert.SerializeObject(lwr);
                    reqData.Status = "Rejected";
                    db.MIS_Requests.Attach(reqData);
                    db.Entry(reqData).Property(x => x.Details).IsModified = true;
                    db.SaveChanges();
                }
                if (User.IsInRole("HR Manager") || User.IsInRole("Administrator"))
                {
                    lwr.HrMgrApproval = false;
                    lwr.HrMgrName = uname;
                    lwr.HRMgr_Id = uid;
                    lwr.HrStatusChangedAt = DateTime.Now;
                    reqData.Status = "Rejected";
                    reqData.Details = JsonConvert.SerializeObject(lwr);
                    db.MIS_Requests.Attach(reqData);
                    db.Entry(reqData).Property(x => x.Details).IsModified = true;
                    db.SaveChanges();
                }
                MarkActivity(new EmployeeHistory
                {
                    AddedBy_Id = uid,
                    AddedBy_Name = uname,
                    AddedOn = DateTime.Now,
                    Comment = "Loan waiver request of Amount " + string.Format("{0:n0}", instData.Loan_Amt) + " has been rejected. Total loan amount : " + string.Format("{0:n0}", lwr.LoanTotalAmount),
                    CommentType = "Text",
                    EmpId = instData.Emp_Id,
                    EmpName = lwr.EmployeeName,
                    GroupId = new Helpers().RandomNumber(),
                });
                return Json(new SimpleReturnMessage { Msg = "Status has been updated successfully", Status = true, _Log = string.Empty });
            }
            catch (Exception ex)
            {
                return Json(new SimpleReturnMessage { Msg = "Failed to update status due to some error. Please try again.", Status = false, _Log = "Message : " + ex.Message + " --------------%%%%%%%%%%%%---------- Inner Exception " + ex.InnerException });
            }
        }
        public JsonResult RevertSalaryDeductedLoanInstallment_CurrentMon(long salary)
        {
            var salData = db.Salaries.Where(x => x.Id == salary).FirstOrDefault();
            DateTime currDt = DateTime.Now;
            var uid = User.Identity.GetUserId<long>();
            var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
            if (salData.Status == "Step_1" && (salData.Salary_Month.Value.Month == currDt.Month && salData.Salary_Month.Value.Year == currDt.Year))
            {
                var res = db.Sp_Update_LoanAdv_Insts(salary).FirstOrDefault();
                MarkActivity(new EmployeeHistory
                {
                    AddedBy_Id = uid,
                    AddedBy_Name = uname,
                    AddedOn = DateTime.Now,
                    Comment = "Loan Installment structures have been updated systematically due to negative salary amount. A reversal of current month loans & advances installments action has been performed",
                    CommentType = "Text",
                    EmpId = salData.Emp_Id,
                    EmpName = salData.Employee_Name,
                    GroupId = new Helpers().RandomNumber()
                });
                return Json(res);
            }
            else
            {
                return Json(false);
            }
        }
        private void MarkActivity(EmployeeHistory eh)
        {
            if (eh is null)
                return;
            db.EmployeeHistories.Add(eh);
            db.SaveChanges();
        }
        [NoDirectAccess] public ActionResult GetLoanPhysicalDoc(long loanId)
        {
            var uid = User.Identity.GetUserId<long>();
            var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
            var loanData = db.LoanRequisitions.Where(x => x.Id == loanId).FirstOrDefault();
            var empDeps = db.Sp_GetEmpCompleteDesigs(loanData.Emp_Id).ToList();
            var empData = db.Employees.Where(x => x.Id == loanData.Emp_Id).FirstOrDefault();
            var oldLoanAdvances = db.LoanRequisitions.Where(x => x.Emp_Id == loanData.Emp_Id).ToList();
            List<long> loanIds = oldLoanAdvances.Select(x => x.Id).ToList();
            var oldLoanInstallments = db.Loan_Installments.Where(x => loanIds.Contains((long)x.Loan_Id)).ToList();
            var lastSalMon = db.Salaries.Where(x => x.Emp_Id == loanData.Emp_Id).OrderByDescending(x => x.Salary_Month).Select(x => x.Salary_Month).Take(1).FirstOrDefault();
            ViewBag.RequestedBy = db.Users.Where(x => x.Id == loanData.User_Id).Select(x => x.Name).FirstOrDefault();
            if (lastSalMon != null)
            {
                oldLoanInstallments.Where(x => x.Date.Value.Date <= lastSalMon.Value.Date).ToList().ForEach(x => x.Status = "Paid");
            }
            if (loanData.PhysicalDoc is null)
            {
                loanData.PhysicalDoc = 1;
                loanData.PhysicalDocNum = db.Sp_Get_ReceiptNo("Loan_Advance_Form").FirstOrDefault();
                db.LoanRequisitions.Attach(loanData);
                db.Entry(loanData).Property(x => x.PhysicalDoc).IsModified = true;
                db.Entry(loanData).Property(x => x.PhysicalDocNum).IsModified = true;
                db.SaveChanges();
            }
            else
            {
                loanData.PhysicalDoc += 1;
                db.LoanRequisitions.Attach(loanData);
                db.Entry(loanData).Property(x => x.PhysicalDoc).IsModified = true;
                db.SaveChanges();
            }
            MarkActivity(new EmployeeHistory
            {
                AddedBy_Id = uid,
                AddedBy_Name = uname,
                AddedOn = DateTime.Now,
                Comment = "Physical document for loan of amount : " + string.Format("{0:n0}", loanData.Amount) + " has been printed. Form# SAG-HR-Forms-" + loanData.PhysicalDocNum + ". Rev# " + loanData.PhysicalDoc,
                CommentType = "Text",
                EmpId = loanData.Emp_Id,
                EmpName = loanData.Employee_Name,
                GroupId = new Helpers().RandomNumber(),
            });
            return PartialView(new LoanAdvancePhysicalDocModel { Designations = empDeps, EmployeeDetails = empData, LoanData = loanData, PreviousLoanRequests = oldLoanAdvances, PreviousLoanInstallments = oldLoanInstallments });
        }
        [NoDirectAccess] public ActionResult LoanSettlement()
        {
            ViewBag.empsList = new SelectList(db.Employees.Select(x => new { id = x.Id, Name = x.Name + " ( " + x.Employee_Code + " )" + " ( " + x.CNIC + " )" }).ToList(), "Id", "Name");
            return View();
        }
        [NoDirectAccess] public ActionResult ActiveLoans(long empId)
        {
            var currDt = DateTime.Now.Date;
            var activeLoans = db.Loan_Installments.Where(x => x.Emp_Id == empId).ToList();
            var loanIds = activeLoans.Select(x => x.Loan_Id).Distinct().ToList();
            var loans = db.LoanRequisitions.Where(x => loanIds.Contains(x.Id)).ToList();
            return PartialView(new EmployeeProfileLoanView { Requisitions = loans, Installments = activeLoans });
        }
        public JsonResult SaveLoanSettlment(long[] installments)
        {
            var receiptno = db.Sp_Get_ReceiptNo("Normal").FirstOrDefault();
            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var uid = User.Identity.GetUserId<long>();
                    AccountHandlerController ah = new AccountHandlerController();
                    var comp = ah.Company_Attr(uid);
                    var installmentsData = db.Loan_Installments.Where(x => installments.Contains(x.Id)).ToList();
                    var empId = installmentsData.Select(y => y.Emp_Id).FirstOrDefault();
                    var empdata = db.Employees.Where(x => x.Id == empId).FirstOrDefault();
                    var res = db.Sp_Add_Receipt("", installmentsData.Sum(x => x.Loan_Amt), GeneralMethods.NumberToWords(Convert.ToInt32(installmentsData.Sum(x => x.Loan_Amt))), string.Empty, string.Empty, null, string.Empty, empdata.Mobile_1,
                        empdata.Father_Name, empdata.Id, empdata.Name, "Cash", null, "Grand City Kharian", 0, string.Empty, string.Empty, "LoanSettlement", uid, uid,
                        "Settlement for Installments " + string.Join(" , ", installmentsData.Select(x => x.Date.Value.ToString("MM/yyyy"))), null, "Loan", string.Empty, empdata.Employee_Code, string.Empty, string.Empty,
                        empdata.Id, new Helpers().RandomNumber(), "", receiptno, comp.Id).FirstOrDefault();
                    bool headcashier = false;
                    if (User.IsInRole("Head Cashier"))
                    {
                        headcashier = true;
                    }
                    try
                    {
                        AccountHandlerController de = new AccountHandlerController();
                        //de.Loan_Received_Installment(installmentsData.Sum(x => x.Loan_Amt), empdata.Id, empdata.Name, empdata.CNIC, empdata.Mobile_1, "Cash", null, null, null, "Loan Installment Paid", new Helpers().RandomNumber(), uid, res.Receipt_No, 1, headcashier);
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    }
                    foreach (var v in installmentsData)
                    {
                        v.Paid_Amt = v.Loan_Amt;
                        v.Status = "Paid";
                        v.Reason = "Amount has been settled on " + DateTime.Now;
                        db.Loan_Installments.Attach(v);
                        db.Entry(v).Property(x => x.Paid_Amt).IsModified = true;
                        db.Entry(v).Property(x => x.Status).IsModified = true;
                        db.Entry(v).Property(x => x.Reason).IsModified = true;
                        db.SaveChanges();
                    }
                    Transaction.Commit();
                    return Json(new { Status = true, ReceiptData = res });
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    return Json(new { Status = false, ReceiptData = new Sp_Add_Receipt_Result() });
                }
            }
        }
        [NoDirectAccess] public ActionResult DepartmentLoanReport()
        {
            var res = db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Department.ToString()).ToList();
            Comp_Dep_Desig cdd = new Comp_Dep_Desig();
            cdd.Id = -1;
            cdd.Parent_Id = 0;
            cdd.Name = "All Departments";
            cdd.Description = "";
            cdd.Type = "";
            cdd.Level = 2;
            res.Add(cdd);
            res = res.OrderBy(x => x.Id).ToList();
            ViewBag.Department = new SelectList(res, "Id", "Name");
            return View();
        }
        [NoDirectAccess] public ActionResult DepartmentalReportDetail(long[] dep, string EmpStatus, DateTime? start, DateTime? end)
        {
            if (dep.Any(x => x == -1))
            {
                var depsRes = db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Department.ToString()).Select(x => x.Id).ToList();
                var a = new XElement("Deps", depsRes.Select(x => new XElement("Id",
                   new XAttribute("Dep", x)
                   ))).ToString();
                var res = db.Sp_Get_DepartmentalLoanReport(a, EmpStatus, start, end).ToList();
                return PartialView(res);
            }
            else
            {
                var a = new XElement("Deps", dep.Select(x => new XElement("Id",
                   new XAttribute("Dep", x)
                   ))).ToString();
                //var dep = new XElement("Deps", new XElement("Id", new XAttribute("Dep", item.Dep_Id))).ToString();
                var res = db.Sp_Get_DepartmentalLoanReport(a, EmpStatus, start, end).ToList();
                return PartialView(res);
            }
        }
        //public void UpdateLoanDedNew()
        //{
        //    long[] arr = new long[] { 1051, 1053, 1057, 1059, 1060, 1067 };
        //    foreach(var v in arr)
        //    {
        //        var res = db.Loan_Installments.Where(x => x.Loan_Id == v).OrderBy(x => x.Id).ToList();
        //        for(int i = 1; i < res.Count(); i++)
        //        {
        //            var loanInst = res.ElementAt(i);
        //            loanInst.Status = "Pending";
        //            loanInst.Date = res.ElementAt(0).Date.Value.AddMonths(i);
        //            loanInst.Paid_Amt = 0;
        //            db.Loan_Installments.Attach(loanInst);
        //            db.Entry(loanInst).Property(x => x.Date).IsModified = true;
        //            db.Entry(loanInst).Property(x => x.Status).IsModified = true;
        //            db.Entry(loanInst).Property(x => x.Paid_Amt).IsModified = true;
        //            db.SaveChanges();
        //        }
        //    }
        //}
    }
}