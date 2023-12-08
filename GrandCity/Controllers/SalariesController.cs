using MeherEstateDevelopers.Filters;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace MeherEstateDevelopers.Controllers
{
    [Authorize]
    [LogAction]
    public class SalariesController : Controller
    {
        // GET: Salaries
        string AccountingModule = COA_Mapper_Modules.Accounting.ToString();
        private MED_MISEntities db = new MED_MISEntities();

        /////////////////////////////////////////////////////////////////////////////////////

        // SALARY INDEX PAGE
        public ActionResult SalaryIndex()
        {
            return View();
        }
        // salary Generate and redirect to AllSalaries

        public JsonResult GenerateiontableSalariesSlip()
        {
            try
            {
                var salMon = DateTime.Now;
                long Userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Salary_Log("Generate Salary", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, null);

                db.Sp_Add_Activity(Userid, "Generated Salaries", "Create", "Activity_Record", ActivityType.Salaries.ToString(), Userid);
                db.Sp_Update_SalarySteps_Dates("Salary_Step_1", salMon);
                var ResEmp = db.Sp_Get_Employee_For_Salaries().ToList();
                var empDeps = db.Employee_Designations.ToList();
                List<string> NSG_Names = new List<string>();
                DateTime dateTime = salMon;
                var res = db.MIS_Modules_Configurations.Where(x => x.Module == MIS_Modules.HR.ToString()).FirstOrDefault();
                HRConfiguration config = new HRConfiguration();
                if (res != null)
                {
                    config = JsonConvert.DeserializeObject<HRConfiguration>(res.Config_For_Update);
                }
                config.FixedAllowances = config.FixedAllowances ?? new List<FixedAllowanceConfigModel>();
                var __tempDep_Holder = config.FixedAllowances.Select(x => x.DepId).ToList();
                var filteredEmps = empDeps.Where(x => __tempDep_Holder.Contains((long)x.Department_Id)).Select(x => new { x.Emp_Id, x.Department_Id }).ToList();
                // if date of joining and current month date are equal then count no days for salary
                foreach (var item in ResEmp)
                {
                    var counter = 0;
                    decimal? Advance = 0;
                    decimal? loanall = 0;
                    var extra_Days = 0;
                    List<Salaries_Details> sd = new List<Salaries_Details>();
                    if (item.Date_Of_Joining.Value.ToString("MM-yyyy") == salMon.ToString("MM-yyyy"))
                    {
                        DateTime nod = (item.Date_Of_Joining.Value);
                        double differenceInDays = nod.Day;
                        counter = Convert.ToInt32((30 - differenceInDays + 1));
                    }
                    else if (item.Date_Of_Joining < item.AddedOn)
                    {
                        var lastsal = db.Salaries.Where(x => x.Emp_Id == item.Id).Select(x => x.Grand_total).FirstOrDefault();
                        if (lastsal == null)
                        {
                            int maxdt = (new DateTime(salMon.Year, salMon.Month, 1).AddMonths(1).AddDays(-1)).Day;
                            DateTime nod = (item.Date_Of_Joining.Value);
                            double differenceInDays = nod.Day;
                            double daysstotal = maxdt - differenceInDays;
                            extra_Days = Convert.ToInt32((daysstotal));
                            counter = 30;
                        }
                        else
                        {
                            counter = 30;
                        }
                    }
                    else
                    {
                        counter = 30;
                    }
                    var res3 = db.Sp_Get_LoanAdvanceInstallment(item.Id).ToList();  // for Loan
                    foreach (var item3 in res3)    // Loan Comment
                    {
                        //Loan_Installments l = new Loan_Installments()
                        //{
                        //    Date = item3.Date,
                        //    Status = LoanStatus.Paid.ToString(),
                        //    Emp_Id = item.Id,
                        //    Paid_Amt = item3.Loan_Amt,
                        //    Loan_Amt = item3.Loan_Amt,
                        //    Loan_Id = item3.Loan_Id,
                        //    Id = Convert.ToInt64(item3.Id)
                        //};
                        //using (MED_MISEntities edb = new MED_MISEntities())
                        //{
                        //    edb.Loan_Installments.Attach(l);
                        //    var entry = edb.Entry(l);
                        //    entry.Property(e => e.Status).IsModified = true;
                        //    entry.Property(e => e.Paid_Amt).IsModified = true;
                        //    edb.SaveChanges();
                        //};
                        //Salaries_Details Ldetails = new Salaries_Details
                        //{
                        //    Description = (item3.Type == "Loan") ? item3.Type : "Advance_Salary",
                        //    Amount = (item3.Loan_Amt).Value,
                        //    Type = SalaryAdditions.OtherDeduction.ToString()
                        //};
                        //sd.Add(Ldetails);
                        if (item3.Type == "Advance_Salary")
                        {
                            Advance = Advance + item3.Loan_Amt.Value;
                        }
                        if (item3.Type == "Loan")
                        {
                            loanall = loanall + item3.Loan_Amt.Value;
                        }
                    }
                    //New Implementation of allowances
                    var res2 = db.Employee_Allownce.Where(x => x.Emp_id == item.Id).ToList();
                    var __currentEmpsMultiDep = filteredEmps.Where(x => x.Emp_Id == item.Id).ToList();
                    foreach (var item2 in __currentEmpsMultiDep)
                    {
                        var _perDepAllnce = config.FixedAllowances.Where(x => x.DepId == item2.Department_Id).FirstOrDefault();
                        if (item.Basic_Salary <= _perDepAllnce.MinSalaryAmt)
                        {
                            if (counter == 30)
                            {
                                Salaries_Details Sdetails = new Salaries_Details
                                {
                                    Description = "Fixed Salary Allowance",
                                    Amount = _perDepAllnce.PerMonthAmount,
                                    Type = SalaryAdditions.AllowncesAndBonus.ToString()
                                };
                                sd.Add(Sdetails);
                            }
                            else
                            {
                                Salaries_Details Sdetails = new Salaries_Details
                                {
                                    Description = "Fixed Salary Allowance",
                                    Amount = Math.Ceiling(counter * (_perDepAllnce.PerMonthAmount / (decimal)30.0)),
                                    Type = SalaryAdditions.AllowncesAndBonus.ToString()
                                };
                                sd.Add(Sdetails);
                            }
                        }
                    }
                    foreach (var item2 in res2)
                    {
                        Salaries_Details Sdetails = new Salaries_Details
                        {
                            Description = item2.AllownceName,
                            Amount = item2.Allownce_Amount.Value,
                            Type = SalaryAdditions.AllowncesAndBonus.ToString()
                        };
                        sd.Add(Sdetails);
                    }
                    var details = new XElement("Salaries", sd.Select(x => new XElement("Details",
                    new XAttribute("Description", x.Description),
                     new XAttribute("Amount", x.Amount),
                     new XAttribute("Type", x.Type)
                     ))).ToString();
                    var allounceamt = sd.Select(x => x.Amount).Sum();
                    var salId = db.Sp_Add_Salary(item.Id, item.Name, item.Department_Designation, allounceamt, item.Employee_Code, item.CNIC, item.Basic_Salary, item.Stipend, counter, loanall, item.Date_Of_Joining, item.Email, Advance, details, extra_Days, item.BankId).SingleOrDefault();
                    var Leaves = db.Sp_Get_EmployeeLeave_dates(item.Id).SingleOrDefault();
                    if (Leaves != null && salId != null)
                    {
                        db.Sp_Update_Leave(salId, Leaves);
                    }
                    if (salId == null)
                    {
                        NSG_Names.Add(item.Name);
                    }
                }
                if (NSG_Names.Any())
                {
                    return Json(NSG_Names);
                }
                else
                {
                    return Json(true);
                }
            }
            catch (Exception ex)
            {
                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                throw ex;
            }
        }

        //public JsonResult GenerateiontableSalariesSlip()
        //{
        //    try
        //    {
        //        long Userid = long.Parse(User.Identity.GetUserId());
        //        db.Sp_Add_Salary_Log("Generate Salary", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, null);
        //        db.Sp_Update_SalarySteps_Dates("Salary_Step_1", DateTime.UtcNow);
        //        var ResEmp = db.Sp_Get_Employee_For_Salaries().ToList();
        //        var empDeps = db.Employee_Designations.ToList();
        //        List<string> NSG_Names = new List<string>();
        //        DateTime dateTime = DateTime.UtcNow.Date;


        //        var res = db.MIS_Modules_Configurations.Where(x => x.Module == MIS_Modules.HR.ToString()).FirstOrDefault();

        //        HRConfiguration config = new HRConfiguration();
        //        if (res != null)
        //        {
        //            config = JsonConvert.DeserializeObject<HRConfiguration>(res.Config_For_Update);
        //        }
        //        var __tempDep_Holder = config.FixedAllowances.Select(x => x.DepId).ToList();
        //        var filteredEmps = empDeps.Where(x => __tempDep_Holder.Contains((long)x.Department_Id)).Select(x => new { x.Emp_Id, x.Department_Id }).ToList();


        //        // if date of joining and current month date are equal then count no days for salary
        //        foreach (var item in ResEmp)
        //        {
        //            var counter = 0;
        //            decimal? Advance = 0;
        //            decimal? loanall = 0;
        //            List<Salaries_Details> sd = new List<Salaries_Details>();
        //            if (item.Date_Of_Joining.Value.ToString("MM-yyyy") == DateTime.Now.ToString("MM-yyyy"))
        //            {
        //                DateTime nod = (item.Date_Of_Joining.Value);
        //                double differenceInDays = nod.Day;
        //                counter = Convert.ToInt32((30 - differenceInDays + 1));
        //            }
        //            else
        //            {
        //                counter = 30;
        //            }

        //            var res3 = db.Sp_Get_LoanAdvanceInstallment(item.Id).ToList();  // for Loan
        //            foreach (var item3 in res3)    // Loan Comment
        //            {
        //                Loan_Installments l = new Loan_Installments()
        //                {
        //                    Date = item3.Date,
        //                    Status = LoanStatus.Paid.ToString(),
        //                    Emp_Id = item.Id,
        //                    Paid_Amt = item3.Loan_Amt,
        //                    Loan_Amt = item3.Loan_Amt,
        //                    Loan_Id = item3.Loan_Id,
        //                    Id = Convert.ToInt64(item3.Id)
        //                };
        //                using (MED_MISEntities edb = new MED_MISEntities())
        //                {
        //                    edb.Loan_Installments.Attach(l);
        //                    var entry = edb.Entry(l);
        //                    entry.Property(e => e.Status).IsModified = true;
        //                    entry.Property(e => e.Paid_Amt).IsModified = true;
        //                    edb.SaveChanges();
        //                };

        //                //Salaries_Details Ldetails = new Salaries_Details
        //                //{
        //                //    Description = (item3.Type == "Loan") ? item3.Type : "Advance_Salary",
        //                //    Amount = (item3.Loan_Amt).Value,
        //                //    Type = SalaryAdditions.OtherDeduction.ToString()
        //                //};
        //                //sd.Add(Ldetails);

        //                if (item3.Type == "Advance_Salary")
        //                {
        //                    Advance = Advance + item3.Loan_Amt.Value;
        //                }
        //                if (item3.Type == "Loan")
        //                {
        //                    loanall = loanall + item3.Loan_Amt.Value;
        //                }
        //            }

        //            //New Implementation of allowances

        //            //var res2 = db.Employee_Allownce.Where(x => x.Emp_id == item.Id).ToList();
        //            var __currentEmpsMultiDep = filteredEmps.Where(x => x.Emp_Id == item.Id).ToList();
        //            foreach (var item2 in __currentEmpsMultiDep)
        //            {
        //                var _perDepAllnce = config.FixedAllowances.Where(x => x.DepId == item2.Department_Id).FirstOrDefault();
        //                if(item.Basic_Salary <= _perDepAllnce.MinSalaryAmt)
        //                {
        //                    if (counter == 30)
        //                    {
        //                        Salaries_Details Sdetails = new Salaries_Details
        //                        {
        //                            Description = "Fixed Salary Allowance",
        //                            Amount = _perDepAllnce.PerMonthAmount,
        //                            Type = SalaryAdditions.AllowncesAndBonus.ToString()
        //                        };
        //                        sd.Add(Sdetails);
        //                    }
        //                    else
        //                    {
        //                        Salaries_Details Sdetails = new Salaries_Details
        //                        {
        //                            Description = "Fixed Salary Allowance",
        //                            Amount = Math.Ceiling(counter * (_perDepAllnce.PerMonthAmount / (decimal)30.0)),
        //                            Type = SalaryAdditions.AllowncesAndBonus.ToString()
        //                        };
        //                        sd.Add(Sdetails);
        //                    }
        //                }
        //            }
        //            //foreach (var item2 in res2)
        //            //{
        //            //    Salaries_Details Sdetails = new Salaries_Details
        //            //    {
        //            //        Description = item2.AllownceName,
        //            //        Amount = item2.Allownce_Amount.Value,
        //            //        Type = SalaryAdditions.AllowncesAndBonus.ToString()
        //            //    };
        //            //    sd.Add(Sdetails);
        //            //}

        //            var details = new XElement("Salaries", sd.Select(x => new XElement("Details",
        //            new XAttribute("Description", x.Description),
        //             new XAttribute("Amount", x.Amount),
        //             new XAttribute("Type", x.Type)
        //             ))).ToString();
        //            var allounceamt = sd.Select(x => x.Amount).Sum();

        //            var salId = db.Sp_Add_Salary(item.Id, item.Name, item.Department_Designation, allounceamt, item.Employee_Code, item.CNIC, item.Basic_Salary, counter, loanall, item.Date_Of_Joining, item.Email, Advance, details).SingleOrDefault();
        //            var Leaves = db.Sp_Get_EmployeeLeave_dates(item.Id).SingleOrDefault();
        //            if (Leaves != null && salId != null)
        //            {
        //                db.Sp_Update_Leave(salId, Leaves);
        //            }
        //            if (salId == null)
        //            {
        //                NSG_Names.Add(item.Name);
        //            }
        //        }
        //        if (NSG_Names.Any())
        //        {
        //            return Json(NSG_Names);
        //        }
        //        else
        //        {
        //            return Json(true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EmailService e = new EmailService();
        //        e.SendEmail(ex.Message + "  --  " + ex.InnerException + "  --  " + ex.StackTrace, "taimoor@sasystems.solutions", "Salaries Error");
        //        throw ex;
        //    }

        //}
        // GET All SALARIES SLIP
        [Authorize(Roles = "Salary 1st Stage,Administrator")]
        public ActionResult Step1()
        {
            try
            {
                var conf = db.MIS_Modules_Configurations.Where(x => x.Module == "HR").Select(x => x.Config_For_Update).FirstOrDefault();
                ViewBag.OTRate = (conf is null) ? 0 : JsonConvert.DeserializeObject<HRConfiguration>(conf).OverTimeRate;
                long Userid = long.Parse(User.Identity.GetUserId());
                var res = db.Sp_Get_All_Employess_Salaries("Step_1").ToList();
                db.Sp_Add_Salary_Log("View Salary", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, null);
                return PartialView(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [Authorize(Roles = "Salary 2nd Stage,Administrator")]
        // Hr inproecss salaris show
        public ActionResult Step2()
        {
            try
            {
                long Userid = long.Parse(User.Identity.GetUserId());
                var res = db.Sp_Get_All_Employess_Salaries("Step_2").ToList();
                db.Sp_Add_Salary_Log("View Salary Inprocess", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, null);
                return PartialView(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        // salary table show by month and year search
        [Authorize(Roles = "Salary 3rd Stage,Administrator")]
        public ActionResult Step3()
        {
            try
            {
                long Userid = long.Parse(User.Identity.GetUserId());
                var res = db.Sp_Get_All_Employess_Salaries("Step_3").ToList();
                db.Sp_Add_Salary_Log("View Salary Approved", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, null);
                ViewBag.BankName = new SelectList(db.Sp_Get_Cash_Bank_Accounts("/1/1/4/_", "/1/1/4/_").ToList(), "Id", "Type");
                //var bankList = db.MIS_Modules_Configurations.Where(x => x.Module == MIS_Modules.Salary_Bank.ToString()).FirstOrDefault();
                //List<SalaryBankEmployee> stm = JsonConvert.DeserializeObject<List<SalaryBankEmployee>>(bankList.CurrentConfig);
                //SalaryStep3Model ss3 = new SalaryStep3Model { Emp_Salaries = res};
                return PartialView(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //
        [Authorize(Roles = "Salary 4th Stage,Administrator")]

        public ActionResult Step4()
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Salary_Log("View Salary Final", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, null);
            return PartialView();
        }

        // For Basic Salaries

        // complete Printable view of salary
        public ActionResult DepartmentReport()
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_All_Employess_Salaries("Step_3").ToList();
            db.Sp_Add_Salary_Log("Complete Salary Print View", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, null);
            return View(res);

        }
        public ActionResult DepartmentReportGazetted()
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            //var res = db.Sp_Get_All_Employess_Salaries("Step_3").ToList();
            var res = db.Sp_Get_Salaries_Gazetted("Step_3").ToList();
            db.Sp_Add_Salary_Log("Gazetted Department Wise Salary Print View", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, null);
            return View(res);
        }
        public ActionResult DepartmentReportNonGazetted()
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            //var res = db.Sp_Get_All_Employess_Salaries("Step_3").ToList();
            var res = db.Sp_Get_Salaries_Non_Gazetted("Step_3").ToList();
            db.Sp_Add_Salary_Log("Non - Gazetted Department Wise Salary Print View", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, null);
            return View(res);
        }
        public ActionResult CompaniesReport()
        {
            var res = db.Sp_Get_All_Employess_Salaries("Step_3").ToList();
            return View(res);
        }

        // For Stipend

        // complete Printable view of salary
        public ActionResult Stipend_DepartmentReport()
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_All_Employess_Salaries("Step_3").ToList();
            db.Sp_Add_Salary_Log("Complete Salary Print View", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, null);
            return View(res);
        }
        public ActionResult Stipend_CompaniesReport()
        {
            var res = db.Sp_Get_All_Employess_Salaries("Step_3").ToList();
            return View(res);
        }

        //      
        [HttpPost]
        public JsonResult UpdateSalaryStatus(long Id, string Status)
        {
            if (Status == "")
            {
                Status = null;
            }
            long Userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Update_SalaryStatus_Parameter(Status, Id, Userid);
            db.Sp_Add_Salary_Log("Update Salary Status", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, Status, null);
            return Json(true);
        }
        // Absentee
        [HttpPost]
        public JsonResult Absentee(long? Id, int? Absente)
        {
            try
            {
                long Userid = long.Parse(User.Identity.GetUserId());
                if (Absente == null)
                {
                    Absente = 0;
                }
                var AbsenteeChecker = db.Sp_Get_Salary(Id).SingleOrDefault();
                if (Absente > AbsenteeChecker.No_Of_Days)
                {
                    return Json(false);
                }
                var res = db.Sp_Update_Absentee(Id, Absente);
                var res1 = db.Sp_Get_Salary(Id).SingleOrDefault();
                var AbsenteeDeduction = Math.Round(Convert.ToDouble(res1.Ded_Absentee));
                var Grand_Total = res1.Grand_total.Value;
                var leave = Convert.ToDecimal((res1.Basic_salary / 30 * res1.Salary_Leaves));
                var result = new { GrandTot = Math.Round(Grand_Total), AbsenteeDed = Math.Round(AbsenteeDeduction), Total_Worked_Days = res1.No_Of_Days - res1.Absentee - res1.Salary_Leaves + res1.Extra_Working_Days, Leave_cal = Math.Round(leave) };
                return Json(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        // Extra Days
        [HttpPost]
        public JsonResult ExtraWorkingDays(long? Id, int? ExtraDays)
        {
            try
            {
                if (ExtraDays == null)
                {
                    ExtraDays = 0;
                }
                var res = db.Sp_Update_WorkingDays(Id, ExtraDays);
                var res1 = db.Sp_Get_Salary(Id).SingleOrDefault();
                var WorkedDaysCalculation = ((res1.Basic_salary / 30) * ExtraDays);
                var Grand_Total = res1.Grand_total.Value;
                var result = new { GrandTot = Math.Round(Grand_Total), Total_Worked_Days = res1.No_Of_Days - res1.Absentee + res1.Extra_Working_Days, WorkedDaysCalculation = Math.Round(Convert.ToDecimal(WorkedDaysCalculation)) };
                return Json(result);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        // OverTime
        [HttpPost]
        public JsonResult OverTime(long? Id, decimal? OverTime)
        {

            if (OverTime == null)
            {
                OverTime = 0;
            }
            var res = db.Sp_Update_Overtime(Id, OverTime);
            var res1 = db.Sp_Get_Salary(Id).SingleOrDefault();
            //var WorkedDaysCalculation = ((res1.Basic_salary / 30) * ExtraDays);
            try
            {
                var Grand_Total = res1.Grand_total.Value;
                var result = new { GrandTot = Math.Round(Grand_Total) };
                return Json(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // Leave
        [HttpPost]
        public JsonResult Leave(long? Id, int? Leave)
        {
            try
            {
                if (Leave == null)
                {
                    Leave = 0;
                }
                var LeaveChecker = db.Sp_Get_Salary(Id).SingleOrDefault();
                if (Leave > LeaveChecker.No_Of_Days)
                {
                    return Json(false);
                }
                var res = db.Sp_Update_Leave(Id, Leave);
                var res1 = db.Sp_Get_Salary(Id).SingleOrDefault();
                var Grand_Total = res1.Grand_total.Value;
                var result = new { GrandTot = Math.Round(Grand_Total), Absen = Math.Round(Convert.ToDecimal((res1.Basic_salary / 30) * res1.Absentee)), Leave = Math.Round(Convert.ToDecimal((res1.Basic_salary / 30) * res1.Salary_Leaves)), Total_Worked_Days = res1.No_Of_Days - res1.Absentee - res1.Salary_Leaves + res1.Extra_Working_Days };
                return Json(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        // Leave
        [HttpPost]
        public JsonResult ImportLeave()
        {
            var res = db.Sp_Get_All_Employess_Salaries("Step_1").ToList();
            foreach (var item in res)
            {
                var Leaves = db.Sp_Get_EmployeeLeave_dates(item.Emp_Id).SingleOrDefault();
                if (Leaves != null)
                {
                    db.Sp_Update_Leave(item.Id, Leaves);
                }
            }
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Imported Leaves", "Update", "Activity_Record", ActivityType.Salaries.ToString(), userid);
            return Json(true);
        }
        // TAX
        public JsonResult Tax(long? Id, decimal? Tax)
        {
            try
            {
                long Userid = long.Parse(User.Identity.GetUserId());

                if (Tax == null)
                {
                    Tax = 0;
                }
                var res1 = db.Sp_Get_Salary(Id).FirstOrDefault();
                var newnet = Math.Round(Convert.ToDecimal(res1.Grand_total + res1.Ded_Tax));
                if (Tax <= newnet)
                {
                    var res2 = db.Sp_Update_Tax(Id, Tax);
                    var Grand_Total = db.Sp_Get_Salary(Id).FirstOrDefault().Grand_total;
                    var result = new { GrandTot = Math.Round(Convert.ToDouble(Grand_Total)) };
                    return Json(result);
                }
                else
                {
                    var Grand_Total = db.Sp_Get_Salary(Id).FirstOrDefault().Grand_total;
                    var result = new { GrandTot = Math.Round(Convert.ToDouble(Grand_Total)), resul = false };
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        // Cash Bank
        [HttpPost]
        public JsonResult CashBank(long Id, string Type, decimal Cash, decimal Bank)
        {
            try
            {
                db.Sp_Add_Cash_Bank(Id, Math.Round(Cash), Type);
                db.Sp_Add_Cash_Bank(Id, Math.Round(Bank), "Bank");
                return Json(true);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        // model popup show
        public ActionResult SalariesDetails(long? Id)
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            ViewBag.userid = Userid;
            var res = db.Sp_Get_Salary_Details(Id).ToList();
            db.Sp_Add_Salary_Log("View Salary Details", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, Id);
            return PartialView(res);

        }
        // Bonus Modal popup 
        public ActionResult Bonus()
        {
            ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Department.ToString()).ToList(), "Id", "Name");
            return View();
        }
        // Bonus Modal popup Induvidual
        public ActionResult BonusIndividual(long Id)
        {
            ViewBag.SalId = Id;
            return View();
        }
        // Bonus On All Employees
        [HttpPost]
        public JsonResult AddBonus(int? BonusPercentage, string Description)
        {
            if (BonusPercentage == null)
            {
                BonusPercentage = 0;
            }
            var res = db.Sp_Get_All_Employess_Salaries("Step_2").ToList();
            if (res != null)
            {
                foreach (var item in res)
                {
                    db.Sp_Add_Bonus(item.Id, Description, Math.Round(Convert.ToDecimal(((item.Basic_salary / 100) * BonusPercentage))));
                }
                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Added Bouneses " + Description, "Create", "Activity_Record", ActivityType.Salaries.ToString(), userid);
                return Json(true);
            }
            else
            {
                return Json(false);

            }

        }
        // Bonus On Departmentwise
        [HttpPost]
        public JsonResult DepartmentBonus(int? BonusPercentage, long? DepartmentId, string Description)
        {
            if (BonusPercentage == null)
            {
                BonusPercentage = 0;
            }
            var res = db.Sp_Get_Deparetment_Approved_Salaries(DepartmentId).ToList();
            if (res != null)
            {
                foreach (var item in res)
                {
                    db.Sp_Add_Bonus(item.Id, Description, Math.Round(Convert.ToDecimal(((item.Basic_salary / 100) * BonusPercentage))));
                }
                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Departmental Bounes Added  " + Description, "Create", "Activity_Record", ActivityType.Salaries.ToString(), userid);
                return Json(true);
            }
            else
            {
                return Json(false);
            }

        }
        // Religion Wise bonus
        public JsonResult ReligionWise(int? BonusPercentage, string Religion, string Description)
        {
            if (BonusPercentage == null)
            {
                BonusPercentage = 0;
            }
            var res = db.Sp_Get_EmployeeReligion_Parameter(Religion).ToList();
            if (res != null)
            {
                foreach (var item in res)
                {
                    db.Sp_Add_Bonus(item.Id, Description, Math.Round(Convert.ToDecimal(((item.Basic_salary / 100) * BonusPercentage))));
                }
                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Added Religion Wise Bouneses " + Description, "Create", "Activity_Record", ActivityType.Salaries.ToString(), userid);
                return Json(true);
            }
            else
            {
                return Json(false);
            }

        }
        // COLA
        public JsonResult COLA(int? BonusPercentage, string Description)
        {
            if (BonusPercentage == null || Description == "")
            {
                return Json(false);
            }
            else
            {
                var res = db.Sp_Get_All_Employess_Salaries("Step_2").ToList();
                foreach (var item in res)
                {
                    if (item.Emp_Date_Of_Joining.Value.ToString("MM-yyyy") == DateTime.Now.ToString("MM-yyyy"))
                    {

                    }
                    else
                    {
                        db.Sp_Add_COLA_On_Salaries(BonusPercentage, ((item.Basic_salary / 100) * BonusPercentage), item.Id, Description);
                        db.Sp_Update_COLA_On_Employees(BonusPercentage, item.Emp_Id);
                    }
                }
                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "UPdated COLA " + Description, "Update", "Activity_Record", ActivityType.Salaries.ToString(), userid);
                return Json(true);
            }
        }
        // COLA On Individual
        public JsonResult IndividualCOLA(long Id, int? BonusPercentage, string Description)
        {
            if (BonusPercentage == null || Description == "")
            {
                return Json(false);
            }
            else
            {
                var res = db.Sp_Get_Salary(Id).FirstOrDefault();

                if (res.Emp_Date_Of_Joining.Value.ToString("MM-yyyy") == DateTime.Now.ToString("MM-yyyy"))
                {
                    return Json(false);
                }
                else
                {
                    db.Sp_Add_COLA_On_Salaries(BonusPercentage, ((res.Basic_salary / 100) * BonusPercentage), res.Id, Description);
                    db.Sp_Update_COLA_On_Employees(BonusPercentage, res.Emp_Id);

                    long userid = long.Parse(User.Identity.GetUserId());
                    db.Sp_Add_Activity(userid, "UPdated COLA " + Description, "Update", "Activity_Record", ActivityType.Salaries.ToString(), userid);
                    return Json(true);
                }


            }
        }
        // add deductions and taxes
        public JsonResult AddDeductionAndBonus(Salaries_Details details, string Group)
        {
            try
            {
                long Userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Salary_Details(details.Description, details.Amount, details.Salaries_id, details.Type, Userid);

                if (details.Type == SalaryAdditions.OtherDeduction.ToString())
                {
                    //db.Sp_Update_Deduction_Other(details.Amount, details.Salaries_id);
                    var sum = db.Salaries_Details.Where(x => x.Salaries_id == details.Salaries_id && x.Type == SalaryAdditions.OtherDeduction.ToString()).Select(x => x.Amount).Sum();
                    var Leave = db.Sp_Get_Salary(details.Salaries_id).FirstOrDefault();
                    var grandtot = db.Salaries.Where(x => x.Id == details.Salaries_id).Select(x => x.Grand_total).SingleOrDefault();
                    var res = db.Sp_Get_SalaryReport().ToList();
                    var GrandTot = res.Sum(x => x.Grand_total);
                    var res3 = res.Where(x => x.DepartmentName == Group).ToList();
                    var Grand_Total_Sum = res.Sum(x => x.Grand_total);
                    List<decimal> sd = new List<decimal>();
                    foreach (var item in res3)
                    {
                        var Group_Sum = db.Salaries_Details.Where(x => x.Salaries_id == item.Id && x.Type == SalaryAdditions.OtherDeduction.ToString()).Select(x => x.Amount).Sum();
                        if (Group_Sum != null)
                        {
                            sd.Add(Group_Sum.Value);
                        }
                    }
                    var ListSum = sd.Sum(x => Convert.ToDecimal(x));
                    db.Sp_Add_Salary_Log("Add Deduction And Taxes ", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, details.Salaries_id);
                    long userid = long.Parse(User.Identity.GetUserId());
                    db.Sp_Add_Activity(userid, "Added Deduction And bonus" + Group, "Update", "Activity_Record", ActivityType.Salaries.ToString(), userid);
                    var result = new { GrandTot = Grand_Total_Sum, GroupSum = ListSum, Result = sum, grntot = Math.Round(grandtot.Value) };
                    return Json(result);
                }
                else
                {
                    var sum = db.Salaries_Details.Where(x => x.Salaries_id == details.Salaries_id && x.Type == SalaryAdditions.AllowncesAndBonus.ToString()).Select(x => x.Amount).Sum();
                    var grandtot = db.Salaries.Where(x => x.Id == details.Salaries_id).Select(x => x.Grand_total).SingleOrDefault();
                    var res = db.Sp_Get_SalaryReport().ToList();
                    var res3 = res.Where(x => x.DepartmentName == Group).ToList();
                    var Grand_Total_Sum = res.Sum(x => x.Grand_total);
                    List<decimal> sd = new List<decimal>();

                    foreach (var item in res3)
                    {
                        var Group_Sum = db.Salaries_Details.Where(x => x.Salaries_id == item.Id && x.Type == SalaryAdditions.AllowncesAndBonus.ToString()).Select(x => x.Amount).Sum();
                        if (Group_Sum != null)
                        {
                            sd.Add(Group_Sum.Value);
                        }

                    }
                    var ListSum = sd.Sum(x => Convert.ToDecimal(x));
                    db.Sp_Add_Salary_Log("Add Allownces And Bonus ", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, details.Salaries_id);
                    long userid = long.Parse(User.Identity.GetUserId());
                    db.Sp_Add_Activity(userid, "Added Allounces And bonus" + Group, "Update", "Activity_Record", ActivityType.Salaries.ToString(), userid);
                    var result = new { GrandTot = Grand_Total_Sum, GroupSum = ListSum, Result = sum, grntot = Math.Round(grandtot.Value) };
                    return Json(result);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        // Get OtherDeductions
        public ActionResult GetOtherDeductions(long? Id)
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            ViewBag.userid = Userid;
            var res = db.Sp_Get_Salary_Details(Id).ToList();
            return PartialView(res);
        }
        // get OtherAllownces
        public ActionResult GetAllowncesAndBonus(long Id)
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            ViewBag.userid = Userid;
            var res = db.Sp_Get_Salary_Details(Id).ToList();
            return PartialView(res);
        }
        //remove deductions and taxes
        public JsonResult RemoveDeductionAndTax(long? Id, long? salaryid, string Group)
        {
            try
            {
                long Userid = long.Parse(User.Identity.GetUserId());
                var amount = db.Salaries_Details.Where(x => x.Id == Id).Select(x => x.Amount).SingleOrDefault();
                //db.Sp_Update_Deduction_Other(amount, salaryid);
                db.Sp_Delete_Salary_Detail(Id);
                var sum = db.Salaries_Details.Where(x => x.Salaries_id == salaryid && x.Type == SalaryAdditions.OtherDeduction.ToString()).Select(x => x.Amount).Sum();
                var grandtot = db.Salaries.Where(x => x.Id == salaryid).Select(x => x.Grand_total).SingleOrDefault();
                var res = db.Sp_Get_SalaryReport().ToList();
                var res3 = res.Where(x => x.DepartmentName == Group).ToList();
                var Grand_Total_Sum = res.Sum(x => x.Grand_total);
                List<decimal> sd = new List<decimal>();
                foreach (var item in res3)
                {
                    var Group_Sum = db.Salaries_Details.Where(x => x.Salaries_id == item.Id && x.Type == SalaryAdditions.OtherDeduction.ToString()).Select(x => x.Amount * (-1)).Sum();
                    if (Group_Sum != null)
                    {
                        sd.Add(Group_Sum.Value);
                    }
                }
                var ListSum = sd.Sum(x => Convert.ToDecimal(x));
                db.Sp_Add_Salary_Log("Remove Deduction And Taxes ", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, salaryid);
                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Remove Deduction And Taxes" + Group, "Update", "Activity_Record", ActivityType.Salaries.ToString(), userid);
                var result = new { GrandTot = Grand_Total_Sum, GroupSum = ListSum, Result = sum, grntot = Math.Round(grandtot.Value) };
                return Json(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //remove Allownces and bonuses
        public JsonResult RemoveAllowncesAndBonus(long? Id, long? salaryid, string Group)
        {
            try
            {
                long Userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Delete_Salary_Detail(Id);
                var sum = db.Salaries_Details.Where(x => x.Salaries_id == salaryid && x.Type == SalaryAdditions.AllowncesAndBonus.ToString()).Select(x => x.Amount).Sum();
                var grandtot = db.Salaries.Where(x => x.Id == salaryid).Select(x => x.Grand_total).SingleOrDefault();
                var res = db.Sp_Get_SalaryReport().ToList();
                var res3 = res.Where(x => x.DepartmentName == Group).ToList();
                var Grand_Total_Sum = res.Sum(x => x.Grand_total);
                List<decimal> sd = new List<decimal>();
                foreach (var item in res3)
                {
                    var Group_Sum = db.Salaries_Details.Where(x => x.Salaries_id == item.Id && x.Type == SalaryAdditions.AllowncesAndBonus.ToString()).Select(x => x.Amount).Sum();
                    if (Group_Sum != null)
                    {
                        sd.Add(Group_Sum.Value);
                    }
                }
                var ListSum = sd.Sum(x => Convert.ToDecimal(x));
                db.Sp_Add_Salary_Log("Remove Deduction And Taxes ", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, salaryid);
                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Remove Deduction And Taxes " + Group, "Update", "Activity_Record", ActivityType.Salaries.ToString(), userid);
                var result = new { GrandTot = Grand_Total_Sum, GroupSum = ListSum, Result = sum ?? 0, grntot = Math.Round(grandtot ?? 0) };
                return Json(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //pdf report of salary
        [Authorize(Roles = "Salary 4th Stage,Administrator")]

        public ActionResult PaySlipView(long? SalariesId)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_EmployeeSalaryInfo(SalariesId).FirstOrDefault();
            if (res.Status == "Received")
            {
                var res12 = db.Sp_Get_Salary(SalariesId).SingleOrDefault();
                var res22 = db.Sp_Get_Salary_Details(SalariesId).ToList();
                var result2 = new SalariesDetails { salary = res12, details = res22 };
                return View(result2);
            }
            this.UpdateSalaryStatus(Convert.ToInt64(SalariesId), "Received");
            var jvNo = db.Sp_Add_Journal_Voucher().FirstOrDefault();

            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var debitAcc = ah.HeadFinder(COA_Mapper_Modules.Human_Resource.ToString(), COA_Mapper_ModuleTypes.Salary_NetSalary.ToString(), COA_Mapper_HeadType.Transaction_Head.ToString(), comp.Id, null);
            try
                {
                    
                    var res1 = db.Sp_Get_Salary(SalariesId).SingleOrDefault();
                    var res2 = db.Sp_Get_Salary_Details(SalariesId).ToList();

                    var narration = "Salary Received by " + res1.Employee_Name + " - " + res1.Employee_code;
                    
                    //if (res1.Bank > 0)
                    //{
                    //    //var TransactionId = new Helpers().RandomNumber();
                    //    //var bankHead = GetHead(Convert.ToInt32(res1.BankHeadId));
                    //    //var bankmod = db.COA_Mapper.Where(x => x.AccountId == res1.BankHeadId).FirstOrDefault();
                    //    //var creditAcc = ah.Payment_Head("Bank", Transaction_Type.Credit.ToString(),Convert.ToInt32( bankmod.Module_Id), comp.Id );

                    //    //if (User.IsInRole("Head Cashier"))
                    //    //{
                    //    //    var Debit = db.Sp_Add_Journal_Entry(res1.Bank, 0, debitAcc.Text_ChartCode + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, narration, TransactionId, 1, userid, userid, null, bankHead.Head, res1.InstNo, creditAcc.PaymentStatus, res1.InstDate, null, "", null, jvNo, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                    //    //    var Credit = db.Sp_Add_Journal_Entry(0, res1.Bank, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, narration, TransactionId, 1, userid, userid, null, bankHead.Head, res1.InstNo, creditAcc.PaymentStatus, res1.InstDate, null, "", null, jvNo, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                    //    //}
                    //    //else
                    //    //{
                    //    //    var Debit = db.Sp_Add_General_Entry(res1.Bank, 0, bankHead.Head, res1.InstNo, creditAcc.PaymentStatus, res1.InstDate, debitAcc.Text_ChartCode + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, narration, TransactionId, 1, null, null, null, userid, jvNo, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                    //    //    var Credit = db.Sp_Add_General_Entry(0, res1.Bank, bankHead.Head, res1.InstNo, creditAcc.PaymentStatus, res1.InstDate, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, narration, TransactionId, 1, null, null, null, userid, jvNo, creditAcc.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                    //    //}

                    //}
                    //if (res1.Cash > 0)
                    //{
                    //    var creditAcc = ah.Payment_Head("Cash", Transaction_Type.Credit.ToString(),null,  comp.Id);
                    //    var TransactionId = new Helpers().RandomNumber();

                    //    if (User.IsInRole("Head Cashier"))
                    //    {
                    //        var Debit = db.Sp_Add_Journal_Entry(res1.Cash, 0, debitAcc.Text_ChartCode + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, narration, TransactionId, 1, userid, userid, null, null, res1.InstNo, creditAcc.PaymentStatus, res1.InstDate, null, "", null, jvNo, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                    //        var Credit = db.Sp_Add_Journal_Entry(0, res1.Cash, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, narration, TransactionId, 1, userid, userid, null, null, res1.InstNo, creditAcc.PaymentStatus, res1.InstDate, null, "", null, jvNo, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                    //    }
                    //    else
                    //    {
                    //        var Debit = db.Sp_Add_General_Entry(res1.Cash, 0, null, res1.InstNo, creditAcc.PaymentStatus, res1.InstDate, debitAcc.Text_ChartCode + " - " + debitAcc.Head, debitAcc.Id, debitAcc.Text_ChartCode, debitAcc.Head, narration, TransactionId, 1, null, null, null, userid, jvNo, creditAcc.VoucherType, comp.Id).FirstOrDefault();
                    //        var Credit = db.Sp_Add_General_Entry(0, res1.Cash, null, res1.InstNo, creditAcc.PaymentStatus, res1.InstDate, creditAcc.PaymentData.Text_ChartCode + " - " + creditAcc.PaymentData.Head, creditAcc.PaymentData.Id, creditAcc.PaymentData.Text_ChartCode, creditAcc.PaymentData.Head, narration, TransactionId, 1, null, null, null, userid, jvNo, creditAcc.VoucherType, comp.Id).FirstOrDefault();   /// For Cash and Bank Credit
                    //    }
                    //}
                    db.Sp_Add_Salary_Log("Print Salary ", userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, SalariesId);
                    var result = new SalariesDetails { salary = res1, details = res2 };
                    try
                    {
                        System.Threading.Thread emlDispatcher = new System.Threading.Thread(() =>
                        {
                            DispatchSalarySlips((long)SalariesId, res);
                        });
                        emlDispatcher.Start();
                    }
                    catch (Exception ex)
                    {
                        //Transaction.Rollback();
                        db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                        return Json(false);
                    }
                    return View(result);
                }
                catch (Exception ex)
                {
                    //Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    return View();
                }
        }
        [Authorize(Roles = "Salary 4th Stage,Administrator")]

        public ActionResult PaySlipViewOnly(long? SalariesId)
        {
            //var res = db.Sp_get_salaries_by_Id(SalariesId).ToList();
            long Userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_Salary(SalariesId).SingleOrDefault();
            var res2 = db.Sp_Get_Salary_Details(SalariesId).ToList();
            db.Sp_Add_Salary_Log("Print Salary ", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, SalariesId);
            var result = new SalariesDetails { salary = res1, details = res2 };
            return View(result);
        }
        [Authorize(Roles = "Salary 4th Stage,Administrator")]

        // View slip in distribution
        public ActionResult PaySlipViewDistribution(long? SalariesId)
        {
            //var res = db.Sp_get_salaries_by_Id(SalariesId).ToList();
            long Userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_Salary(SalariesId).SingleOrDefault();
            var res2 = db.Sp_Get_Salary_Details(SalariesId).ToList();
            db.Sp_Add_Salary_Log("View Print In Salary Distribution ", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, SalariesId);
            var result = new SalariesDetails { salary = res1, details = res2 };
            return View(result);
        }
        public ActionResult PaySlipEmployee(long? SalariesId)
        {
            //var res = db.Sp_get_salaries_by_Id(SalariesId).ToList();
            long Userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_Salary(SalariesId).SingleOrDefault();
            var res2 = db.Sp_Get_Salary_Details(SalariesId).ToList();
            db.Sp_Add_Salary_Log("View Print In Salary Distribution ", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, SalariesId);
            var result = new SalariesDetails { salary = res1, details = res2 };
            return View(result);
        }

        // view salary only
        [Authorize(Roles = "Salary 4th Stage,Administrator")]
        public ActionResult ViewSalary(long? SalariesId)
        {
            //var res = db.Sp_get_salaries_by_Id(SalariesId).ToList();
            long Userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_Salary(SalariesId).SingleOrDefault();
            var res2 = db.Sp_Get_Salary_Details(SalariesId).ToList();
            db.Sp_Add_Salary_Log("View Salary ", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, SalariesId);
            var result = new SalariesDetails { salary = res1, details = res2 };
            return View(result);
        }
        // Status updation
        [HttpPost]
        public JsonResult UpdateStatus(string status)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            if (status == "Step_3")
            {
                db.Sp_Update_SalarySteps_Dates("Salary_Step_3", DateTime.UtcNow);
                var a = db.Sp_Add_Salaries_Cash();
            }
            if (status == "Step_2")
            {
                db.Sp_Update_SalarySteps_Dates("Salary_Step_2", DateTime.UtcNow);
            }
            db.Sp_Update_SalaryStatus(status);
            db.Sp_Add_Salary_Log("Update Salary Status", userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, status, null);

            db.Sp_Add_Activity(userid, "Updated Salaries Status " + status, "Update", "Activity_Record", ActivityType.Salaries.ToString(), userid);
            return Json(status);
        }
        public JsonResult FinalizeSalries()
        {
            db.Sp_Update_SalarySteps_Dates("Salary_Step_4", DateTime.UtcNow);
            var res = db.Sp_Get_All_Employess_Salaries("Step_3").ToList();

            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Sp_Update_SalaryStatus("Step_4");
                    long Userid = long.Parse(User.Identity.GetUserId());
                    db.Sp_Add_Activity(Userid, "Updated Salaries ", "Update", "Activity_Record", ActivityType.Salaries.ToString(), Userid);
                    Helpers h = new Helpers();
                    bool headcashier = false;
                    if (User.IsInRole("Head Cashier"))
                    {
                        headcashier = true;
                    }
                    foreach (var v in res.GroupBy(x => x.CompId))
                    {
                        var TransactionId = h.RandomNumber();
                        var vouch = db.Sp_Add_Journal_Voucher().FirstOrDefault();
                        decimal? BasicSalary = Convert.ToDecimal(Math.Round(Convert.ToDouble(v.Sum(x => x.Act_Salary) + v.Sum(x => x.Stipend))));
                        decimal? Allowance = Convert.ToDecimal(Math.Round(Convert.ToDouble(v.Sum(x => x.Allownces))));
                        decimal? Bonus = Convert.ToDecimal(Math.Round(Convert.ToDouble(v.Sum(x => x.Bonus))));
                        decimal? Overtime = Convert.ToDecimal(Math.Round(Convert.ToDouble(v.Sum(x => x.Overtime))));
                        decimal? TaxDeduction = Convert.ToDecimal(Math.Round(Convert.ToDouble(v.Sum(x => x.Ded_Tax))));
                        decimal? LoanDeduction = Convert.ToDecimal(Math.Round(Convert.ToDouble(v.Sum(x => x.Ded_Loan))));
                        decimal? AdvanceDeduction = Convert.ToDecimal(Math.Round(Convert.ToDouble(v.Sum(x => x.Ded_Advance))));
                        decimal? OtherDeduction = Convert.ToDecimal(Math.Round(Convert.ToDouble(v.Sum(x => x.Ded_Other))));
                        decimal? ExtraFuel = Convert.ToDecimal(Math.Round(Convert.ToDouble(v.Sum(x => x.Extra_Fuel_Charges))));
                        decimal? NetSalary = Convert.ToDecimal(Math.Round(Convert.ToDouble(v.Sum(x => x.Grand_total))));

                        AccountHandlerController de = new AccountHandlerController();
                        de.Salary_Posting(BasicSalary, Allowance, Bonus, Overtime, TaxDeduction, LoanDeduction, AdvanceDeduction, OtherDeduction, ExtraFuel, NetSalary, TransactionId, Userid, vouch, headcashier, COA_Mapper_Modules.Human_Resource.ToString(), v.Key.Value, v.Select(x => x.Salary_Month).FirstOrDefault());
                    }
                    Transaction.Commit();
                    return Json(true);
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    return Json(false);
                }
            }
        }
        // This is New Enable
        [Authorize(Roles = "Salary Search,Administrator")]
        public ActionResult SalaryList()
        {
            return PartialView();
        }
        public ActionResult SalariedPersonnels()
        {
            return PartialView();
        }
        public ActionResult StipendPersonnels()
        {
            return PartialView();
        }
        // This is New Enable
        public ActionResult SearchSalaries(DateTime From, DateTime To, string Status, int DepartmentId)
        {
            var res = db.Sp_Get_SalariesList(From, To, Status, DepartmentId).ToList();
            if (res == null || res.Count <= 0)
            {
                res = db.Sp_Get_SalariesList(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(-1), "All", 0).ToList();
            }
            return PartialView(res);
        }
        public ActionResult SearchStipends(DateTime From, DateTime To, string Status, int DepartmentId)
        {
            var res = db.Sp_Get_StipendsList(From, To, Status, DepartmentId).ToList();
            if (res == null || res.Count <= 0)
            {
                res = db.Sp_Get_StipendsList(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(-1), "All", 0).ToList();
            }
            return PartialView(res);
        }
        [HttpPost]
        public JsonResult ExtraFuelDeductions()
        {
            var ResEmp = db.Sp_Get_Employee_For_Salaries().ToList();
            foreach (var item in ResEmp)
            {
                var res4 = db.Sp_Get_Employee_Fuel_Details(item.Id).SingleOrDefault();
                if (res4 != null)
                {
                    db.Sp_Update_Extra_Fuel_Salary(item.Id, Convert.ToInt16(res4.Vehicle_Consumption - res4.Vehicle_Fuel_Allow), Convert.ToDecimal(res4.Total * (-1)));
                }
            }

            return Json(true);
        }
        public ActionResult DailySalaryDistribution(int? Month, int? Year)
        {
            var res = db.Sp_Get_Salaries_Delivery(Month, Year).ToList();
            return PartialView(res);
        }
        public ActionResult CashPayables(int? Month, int? Year)
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_Final_Cash_Salaries(Month, Year).ToList();
            db.Sp_Add_Salary_Log("View Salary Final", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, null);
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Viewed Cash Payable Salaries ", "Read", "Activity_Record", ActivityType.Salaries.ToString(), userid);

            return PartialView(res);
        }
        public ActionResult BankPayables(int? Month, int? Year)
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_Final_Bank_Salaries(Month, Year).ToList();
            db.Sp_Add_Salary_Log("View Salary Final", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, null);
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Viewed Bank Payable Salaries ", "Read", "Activity_Record", ActivityType.Salaries.ToString(), userid);

            return PartialView(res);
        }
        // This is New Enable
        public ActionResult SalaryListWithStatus()
        {
            return View();
        }
        // This is New Enable
        public ActionResult SearchSalariesStatus(DateTime From, DateTime To, string Status, int DepartmentId)
        {

            var res = db.Sp_Get_SalariesList(From, To, Status, DepartmentId).ToList();
            return PartialView(res);
        }
        // on Step 4  all salaries search enable 
        public ActionResult FinanceFinalSlipSearch(int Month, int Year)
        {
            ViewBag.Mo = Month;
            ViewBag.Yr = Year;
            return PartialView();
        }
        public JsonResult Delete(long Id)
        {
            var res = db.Sp_Get_Salary(Id).FirstOrDefault();
            try
            {
                if (res.COLA_Amount != null)
                {
                    db.Sp_Update_Employee_BasicSalary_COLA(res.Emp_Id, res.COLA_Amount);
                }
                long userid = long.Parse(User.Identity.GetUserId());
                db.Sp_Add_Activity(userid, "Deleted Salary ", "Delete", "Activity_Record", ActivityType.Salaries.ToString(), Id);

                db.Sp_Delete_Salary(Id);
                return Json(true);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public JsonResult Update(long Id)
        {
            try
            {
                var res = db.Sp_Get_Salary(Id).FirstOrDefault();
                List<string> NSG_Names = new List<string>();
                if (res.COLA_Amount != null)
                {
                    db.Sp_Update_Employee_BasicSalary_COLA(res.Emp_Id, res.COLA_Amount);
                }

                db.Sp_Delete_Salary(Id);
                var item = db.Sp_Get_Employee_Parameter(res.Emp_Id).FirstOrDefault();
                var date = db.ReceiptNumbers.Where(x => x.Type == "Salary_Step_1").FirstOrDefault().Year;
                var extra_Days = res.Extra_Working_Days;
                var counter = 0;
                decimal? loanall = 0;
                decimal? Advance = 0;
                if (item.Date_Of_Joining.Value.ToString("MM-yyyy") == Convert.ToDateTime(date).ToString("MM-yyyy"))
                {
                    DateTime nod = (item.Date_Of_Joining.Value);
                    double differenceInDays = nod.Day;
                    counter = Convert.ToInt32((30 - differenceInDays + 1));
                }
                else
                {
                    counter = 30;
                }
                List<Salaries_Details> sd = new List<Salaries_Details>();
                var res2 = db.Employee_Allownce.Where(x => x.Emp_id == item.Id).ToList();
                var res3 = db.Sp_Get_LoanAdvanceInstallment_Updation(item.Id).ToList();  // for Loan
                foreach (var item2 in res2)
                {
                    Salaries_Details Sdetails = new Salaries_Details
                    {
                        Description = item2.AllownceName,
                        Amount = item2.Allownce_Amount,
                        Type = SalaryAdditions.AllowncesAndBonus.ToString()
                    };

                    sd.Add(Sdetails);
                }
                foreach (var item3 in res3)    // Loan Comment
                {
                    //db.Sp_Update_LoaninstallmentStatus(LoanStatus.Paid.ToString(), item3.Id);
                    //Salaries_Details Ldetails = new Salaries_Details
                    //{
                    //    Description = (item3.Type == "Loan") ? item3.Type : "Advance_Salary",
                    //    Amount = (item3.Loan_Amt),
                    //    Type = SalaryAdditions.OtherDeduction.ToString()
                    //};
                    //sd.Add(Ldetails);

                    if (item3.Type == "Advance_Salary")
                    {
                        Advance = Advance + item3.Loan_Amt.Value;
                    }
                    if (item3.Type == "Loan")
                    {
                        loanall = loanall + item3.Loan_Amt.Value;
                    }
                }

                var details = new XElement("Salaries", sd.Select(x => new XElement("Details",
                new XAttribute("Description", x.Description),
                 new XAttribute("Amount", x.Amount),
                 new XAttribute("Type", x.Type)
                 ))).ToString();
                var allounceamt = res2.Select(x => x.Allownce_Amount).Sum();
                var salId = db.Sp_Add_Salary(item.Id, item.Name, item.Department_Designation, allounceamt, item.Employee_Code, item.CNIC, item.Basic_Salary, item.Stipend, counter, loanall, item.Date_Of_Joining, item.Email, Advance, details, extra_Days, item.BankId).SingleOrDefault();

                var Leaves = db.Sp_Get_EmployeeLeave_dates(item.Id).SingleOrDefault();
                if (Leaves != null && salId != null)
                {
                    db.Sp_Update_Leave(salId, Leaves);
                }
                if (salId == null)
                {
                    NSG_Names.Add(item.Name);
                }
                if (NSG_Names.Any())
                {
                    return Json(NSG_Names);
                }
                else
                {
                    return Json(true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //// This is New Enable
        //public ActionResult SearchSalayByDate(DateTime date)
        //{
        //    long Userid = long.Parse(User.Identity.GetUserId());
        //    var res = db.Sp_Get_Salary_By_Date(date).ToList();
        //    db.Sp_Add_Salary_Log("Search Salary By Date", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, date, null, null);
        //    return PartialView(res);
        //}
        //************** Reports Section Implementation *************
        public ActionResult SalariesReports()
        {
            var res = db.Sp_Get_Reports_Of_All_Salary_Parameter(null, null).ToList();
            return View(res);

        }
        public ActionResult SalariesReportsSearch(int Month, int Year)
        {
            var res = db.Sp_Get_Reports_Of_All_Salary_Parameter(Month, Year).ToList();
            return PartialView(res);
        }
        public ActionResult VarianceOfNewHiringAndBasic()
        {
            var res = db.Sp_Get_Last_Month_Salary_Report().ToList();
            var res2 = db.Sp_Get_Previous_Month_Salary_Report().ToList();

            var selectedTwo = res2.Select(x => x.Emp_Id).ToList();
            var PreNotGen = res.Where(n => !selectedTwo.Contains(n.Emp_Id)).ToList();


            List<BasicSalaryComparison> result = (
                  from p in res
                  join sc in res2 on p.Emp_Id equals sc.Emp_Id
                  where p.Emp_Id == sc.Emp_Id
                  select new BasicSalaryComparison
                  {
                      Id = p.Id,
                      Employee_Name = p.Employee_Name,
                      Employee_code = p.Employee_code,
                      Designation_name = p.Designation_name,
                      Basic_salary = p.Basic_salary,
                      Previous_Basic_Salary = sc.Basic_salary,
                      DepartmentId = p.DepartmentId,
                      DepartmentName = p.DepartmentName

                  }).ToList();

            var res4 = new SalaryModelListing { PreviusNotGenerated = PreNotGen, PreviousBasicSalaryvariance = result };
            return View(res4);


        }
        public ActionResult GetEmployeeSalaries(long emp)
        {
            var res = db.Salaries.Where(x => x.Emp_Id == emp).ToList();
            return PartialView(res);
        }
        public ActionResult SalaryManagement()
        {
            return View();
        }
        public JsonResult HoldSalary(long Id, int stat)
        {
            var res = db.Salaries.Where(x => x.Id == Id).FirstOrDefault();
            var uid = User.Identity.GetUserId<long>();
            var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
            if (res is null)
            {
                return Json(false);
            }
            if (stat == 0)
            {
                res.IsHold = true;
            }
            else if (stat == 1)
            {
                res.IsHold = null;
            }
            db.Salaries.Attach(res);
            db.Entry(res).Property(x => x.IsHold).IsModified = true;
            db.SaveChanges();
            if (stat == 1)
            {
                MarkActivity(new EmployeeHistory
                {
                    AddedBy_Id = uid,
                    AddedBy_Name = uname,
                    AddedOn = DateTime.Now,
                    Comment = "Salary has been unholded.",
                    CommentType = "Salary",
                    EmpId = res.Emp_Id,
                    EmpName = res.Employee_Name,
                    GroupId = new Helpers().RandomNumber()
                });
            }
            else if (stat == 0)
            {
                MarkActivity(new EmployeeHistory
                {
                    AddedBy_Id = uid,
                    AddedBy_Name = uname,
                    AddedOn = DateTime.Now,
                    Comment = "Salary has been holded.",
                    CommentType = "Salary",
                    EmpId = res.Emp_Id,
                    EmpName = res.Employee_Name,
                    GroupId = new Helpers().RandomNumber()
                });
            }
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Holded Salary ", "Update", "Activity_Record", ActivityType.Salaries.ToString(), Id);

            return Json(true);
        }
        private void MarkActivity(EmployeeHistory eh)
        {
            try
            {
                if (eh is null)
                    return;
                if (eh.GroupId != null)
                {
                    if (!db.EmployeeHistories.Any(x => x.GroupId == eh.GroupId))
                    {
                        db.EmployeeHistories.Add(eh);
                        db.SaveChanges();
                    }
                }
                else
                {
                    db.EmployeeHistories.Add(eh);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }
        }
        public ActionResult SalaryTaxConfig()
        {
            var res = db.MIS_Modules_Configurations.Where(x => x.Module == MIS_Modules.Salary_Tax.ToString()).FirstOrDefault();

            if (res is null)
            {
                //FIrst Time
                return View(new List<SalaryTaxEmployee>());
            }
            else
            {
                //already exists
                List<SalaryTaxEmployee> stm = JsonConvert.DeserializeObject<List<SalaryTaxEmployee>>(res.CurrentConfig);
                return View(stm);
            }
        }
        public JsonResult GetEmployees_Select(string s)
        {
            var result = db.Employees.Where(x => x.Name.Contains(s)).Select(x => new Employee_serch { Id = x.Id, Name = x.Name, EmpCode = x.Employee_Code, Basic_Salary = x.Basic_Salary ?? 0 }).ToList();

            return Json(new { items = result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployees_Detail_Byname(long s)
        {
            //By Id
            var result = db.Employees.Where(x => x.Id == s).Select(x => new Employee_serch { Id = x.Id, Name = x.Name, EmpCode = x.Employee_Code, Basic_Salary = x.Basic_Salary ?? 0 }).ToList();

            return Json(new { items = result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveSalaryTaxConfig(List<SalaryTaxEmployee> stm)
        {
            if (stm is null)
            {
                return Json(false);
            }

            var res = db.MIS_Modules_Configurations.Where(x => x.Module == MIS_Modules.Salary_Tax.ToString()).FirstOrDefault();
            var uid = User.Identity.GetUserId<long>();
            var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Salary Tax Configured for " + string.Join(",", stm.Select(x => x.EmployeeName)), "Update", "Activity_Record", ActivityType.Salaries.ToString(), userid);

            if (res is null)
            {
                //First time
                MIS_Modules_Configurations __m = new MIS_Modules_Configurations
                {
                    CurrentConfig = JsonConvert.SerializeObject(stm),
                    LastUpdated = DateTime.Now,
                    Module = MIS_Modules.Salary_Tax.ToString(),
                    UpdatedBy_Id = uid,
                    UpdatedBy_Name = uname
                };
                db.MIS_Modules_Configurations.Add(__m);
                db.SaveChanges();
                return Json(true);
            }
            else
            {
                //already exists
                res.CurrentConfig = JsonConvert.SerializeObject(stm);
                res.UpdatedBy_Id = uid;
                res.UpdatedBy_Name = uname;
                res.LastUpdated = DateTime.Now;
                db.MIS_Modules_Configurations.Attach(res);
                db.Entry(res).Property(x => x.CurrentConfig).IsModified = true;
                db.Entry(res).Property(x => x.UpdatedBy_Name).IsModified = true;
                db.Entry(res).Property(x => x.UpdatedBy_Id).IsModified = true;
                db.Entry(res).Property(x => x.LastUpdated).IsModified = true;
                db.SaveChanges();
                return Json(true);
            }
        }
        public JsonResult EmbedSalaryTaxes()
        {
            try
            {
                DateTime dt = DateTime.Now;
                List<string> Names = new List<string>(); 

                var config = db.MIS_Modules_Configurations.Where(x => x.Module == MIS_Modules.Salary_Tax.ToString()).FirstOrDefault();
                var parsedData = JsonConvert.DeserializeObject<List<SalaryTaxEmployee>>(config.CurrentConfig);
                foreach (var slab in parsedData)
                {
                    var sal_id = db.Sp_Get_EmpSalId(dt, dt, slab.EmployeeId).FirstOrDefault();
                    if (sal_id == null)
                    {
                        Names.Add(slab.EmployeeName + " " + slab.EmpCode);
                        continue;
                    }
                    var skls = new XElement("SalaryTaxes", new XElement("TaxAmount",
                   new XAttribute("SalId", sal_id),
                   new XAttribute("EmpId", slab.EmployeeId),
                   new XAttribute("Tax", slab.TaxAmount))).ToString();

                    var _out = db.Sp_Update_SalaryTaxes(skls);
                }
                return Json(Names);
            }
            catch (Exception ex)
            {
                db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                return Json(false);
            }
        }
        public JsonResult ForwardSalary(string status)
        {
            var currDt = DateTime.Now;
            if (db.Salaries.Where(x => x.Salary_Month.Value.Month == currDt.Month && x.Salary_Month.Value.Year == currDt.Year).Any(x => x.Grand_total <= -1))
            {
                var negSalEmp = db.Salaries.Where(x => x.Salary_Month.Value.Month == currDt.Month && x.Salary_Month.Value.Year == currDt.Year && x.Grand_total <= -1).Select(x => new { Name = x.Employee_Name + " ( " + x.Employee_code + " )" }).FirstOrDefault();
                return Json(new { Msg = "Salary of Employee : " + negSalEmp + " is negative. Please revise salary before forwarding.", NewStatus = status, Status = false });
            }
            long userid = long.Parse(User.Identity.GetUserId());
            if (status == "Step_3")
            {
                db.Sp_Update_SalarySteps_Dates("Salary_Step_3", DateTime.UtcNow);

                db.Sp_Add_Activity(userid, "Forwarded Salaries ", "Update", "Activity_Record", ActivityType.Salaries.ToString(), userid);

                var a = db.Sp_Add_Salaries_Cash();
            }
            if (status == "Step_2")
            {
                db.Sp_Update_SalarySteps_Dates("Salary_Step_2", DateTime.UtcNow);
                db.Sp_Add_Activity(userid, "Forwarded Salaries ", "Update", "Activity_Record", ActivityType.Salaries.ToString(), userid);

            }
            db.Sp_Update_SalaryStatus(status);
            db.Sp_Add_Salary_Log("Update Salary Status", userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, status, null);
            return Json(new { Msg = "Salaries have been forwarded successfully.", NewStatus = status, Status = true });
        }
        public void DispatchSalarySlips(long salId, Sp_Get_EmployeeSalaryInfo_Result salary)
        {
            try
            {
                var dt = DateTime.Now.AddMonths(-1);
                //var res = db.Sp_Get_SalariesList(dt, dt, "Received", null).ToList();
                var details = db.Sp_Get_Salary_Details(salId).ToList();

                var empsData = db.Employees.Where(x => x.Id == salary.Emp_Id).Select(x => new { x.Id, CompanyEmail = x.Company_Email, x.Mobile_1, x.Mobile_2, SecondaryEmail = x.Email }).FirstOrDefault();
                string eml = (!string.IsNullOrEmpty(empsData.CompanyEmail)) ? empsData.CompanyEmail : empsData.SecondaryEmail;
                string mobno = (!string.IsNullOrEmpty(empsData.Mobile_1)) ? empsData.Mobile_1 : empsData.Mobile_2;
                if (!string.IsNullOrEmpty(eml))
                {
                    string sb = "<html xmlns=\"http://www.w3.org/1999/xhtml\">" +
    "<head>" +
    "    <meta name='viewport' content='width=device-width' />" +
    "<style>" +
    "body {" +
    "    width: 100vw;" +
    "    height: 100vh;" +
    "    display: flex;" +
    "    /*justify-content: center;*/" +
    "    padding: 0px;" +
    "    height: 10%;" +
    "}" +
    "* {" +
    "    font-family: 'Roboto', sans-serif;" +
    "    font-size: 12px;" +
    "    color: #444;" +
    "}" +
    "#payslip {" +
    "    width: calc( 8.5in - 80px );" +
    "    background: #fff;" +
    "}" +
    ".imagesa {" +
    "    float: right;" +
    "}" +
    ".imagesas {" +
    "    width: 150px;" +
    "    float: right;" +
    "}" +
    "#title {" +
    "    margin-bottom: 10px;" +
    "    font-size: 30px;" +
    "    font-weight: 600;" +
    "    float: left;" +
    "    width: 100%;" +
    "}" +
    "#title2 {" +
    "    float: left;" +
    "    width: 100%;" +
    "}" +
    "#scope {" +
    "    border-top: 1px solid #ccc;" +
    "    border-bottom: 1px solid #ccc;" +
    "    padding: 7px 0 4px 0;" +
    "    display: flex;" +
    "    justify-content: space-around;" +
    "}" +
    "    #scope > .scope-entry {" +
    "        text-align: center;" +
    "    }" +
    "        #scope > .scope-entry > .value {" +
    "            font-size: 14px;" +
    "            font-weight: 700;" +
    "        }" +
    ".content {" +
    "    display: flex;" +
    "    width: 100%;" +
    "    height: 100%;" +
    "    margin-top: 5%;" +
    "}" +
    "    .content .left-panel {" +
    "        border-right: 1px solid #ccc;" +
    "        width: 45%;" +
    "        /*padding: 1px 6px 0px 68px;*/" +
    "        padding: 1px 10px 5px 0px;" +
    "        font-weight: 400;" +
    "        height: 64%;" +
    "    }" +
    "    .content .right-panel {" +
    "        width: 50%;" +
    "        padding: 0px 0px 0px 10px;" +
    "        height: 34%;" +
    "    }" +
    ".month {" +
    "    float: right;" +
    "    font-weight: 700;" +
    "}" +
    "#employee {" +
    "    text-align: center;" +
    "    margin-bottom: 20px;" +
    "}" +
    "    #employee #name {" +
    "        font-size: 15px;" +
    "        font-weight: 700;" +
    "    }" +
    "    #employee #email {" +
    "        font-size: 11px;" +
    "        font-weight: 300;" +
    "    }" +
    ".details, .contributions, .ytd, .gross {" +
    "    margin-bottom: 0px;" +
    "}" +
    "    .details .entry, .contributions .entry, .ytd .entry {" +
    "        display: flex;" +
    "        justify-content: space-between;" +
    "        margin-bottom: 3px;" +
    "    }" +
    "        .details .entry .value, .contributions .entry .value, .ytd .entry .value {" +
    "            font-weight: 700;" +
    "            max-width: 200px;" +
    "            text-align: right;" +
    "        }" +
    "    .gross .entry .value {" +
    "        font-weight: 700;" +
    "        text-align: right;" +
    "        font-size: 16px;" +
    "    }" +
    "    .contributions .title, .ytd .title, .gross .title {" +
    "        font-size: 15px;" +
    "        font-weight: 700;" +
    "        border-bottom: 1px solid #ccc;" +
    "        padding-bottom: 4px;" +
    "        margin-bottom: 6px;" +
    "    }" +
    "    .content .right-panel .details .entry {" +
    "        display: flex;" +
    "        /*padding: 0 5px;*/" +
    "        margin: 0px 0;" +
    "    }" +
    ".content .right-panel .details .label {" +
    "    font-weight: bold;" +
    "    width: 50%;" +
    "}" +
    ".content .right-panel .details .detail {" +
    "    font-weight: bold;" +
    "    width: 100%;" +
    "}" +
    "    .content .right-panel .details .rate {" +
    "        font-weight: 400;" +
    "        width: 80px;" +
    "        font-style: italic;" +
    "        letter-spacing: 1px;" +
    "    }" +
    ".content .right-panel .details .amount {" +
    "    text-align: right;" +
    "    font-weight: 700;" +
    "    width: 50%;" +
    "}" +
    "    .content .right-panel .details .net_pay div, .content .right-panel .details .nti div {" +
    "        font-weight: 600;" +
    "        font-size: 12px;" +
    "    }" +
    "    .content .right-panel .details .net_pay, .content .right-panel .details .nti {" +
    "        padding: 3px 0 2px 0;" +
    "        margin-bottom: 10px;" +
    "        background: rgba(0, 0, 0, 0.04);" +
    "    }" +
    "</style>" +
    "</head>" +
    "<body>" +
    "    <div class=' '>" +
    "        <div id='payslip' class=''>" +
    "            <div id='title'>" +
    "                Employee Copy" +
    "                <img src='/assets/static/images/meherestate-simple.png' class='imagesas'>" +
    "            </div>" +
    "            <div id='title2'>" +
    "                <label class='' style='font-weight:700; float:left'>Salary Month: &nbsp;" + string.Format("{0:MMMMMMMMM, yyyy}", salary.Salary_Month) + " &nbsp;  </label>" +
    "                <label class='' style='font-weight:700; float:right'>Received Date: &nbsp;" + string.Format("{0: dd MMMM yyyy h:mm tt}", salary.Received_Date) + " &nbsp;  </label>" +
    "            </div>" +
    "            <div class='content'>" +
    "                <div class='left-panel '>" +
    "                    <div class='details' id=''>" +
    "                        <div class='entry'>" +
    "                            <div class='label'>Department : </div>" +
    "                            <div class='value'>" + salary.DepartmentName + "</div>" +
    "                        </div>" +
    "                        <div class='entry'>" +
    "                            <div class='label'>Employee Code : </div>" +
    "                            <div class='value'>" + salary.Employee_code + "</div>" +
    "                        </div>" +
    "                        <div class='entry'>" +
    "                            <div class='label'>Employee Name : </div>" +
    "                            <div class='value'>" + salary.Employee_Name + "</div>" +
    "                        </div>" +
    "                        <div class='entry'>" +
    "                            <div class='label'>CNIC / NICOP : </div>" +
    "                            <div class='value'>" + salary.CNIC + "</div>" +
    "                        </div>" +
    "                        <div class='entry'>" +
    "                            <div class='label'>Date Hired : </div>" +
    "                            <div class='value'>" + string.Format("{0: MM/dd/yyyy}", salary.Emp_Date_Of_Joining) + "</div>" +
    "                        </div>" +
    "                        <div class='entry'>" +
    "                            <div class='label'>Position : </div>" +
    "                            <div class='value'>" + salary.Designation_name + "</div>" +
    "                        </div>" +
    "                        <div class='entry'>" +
    "                            <div class='label'>Prepared by : </div>" +
    "                            <div class='value'>HR Department</div>" +
    "                        </div>" +
    "                        <div class='entry'>" +
    "                            <div class='label'>No Of Days : </div>" +
    "                            <div class='value'>" + salary.No_Of_Days + "</div>" +
    "                        </div>";
                    if (salary.Absentee != null && salary.Absentee != 0)
                    {
                        sb += "                            <div class='entry'>" +
                        "                                <div class='label'>Absentee : </div>" +
                        "                                <div class='value'>" + salary.Absentee + "</div>" +
                        "                            </div>";
                    }
                    if (salary.Salary_Leaves != null && salary.Salary_Leaves != 0)
                    {
                        sb += "                            <div class='entry'>" +
                        "                                <div class='label'>Paid Leaves : </div>" +
                        "                                <div class='value'>" + salary.Salary_Leaves + "</div>" +
                        "                            </div>";
                    }
                    if (salary.Extra_Working_Days != null && salary.Extra_Working_Days != 0)
                    {
                        sb += "                            <div class='entry'>" +
                        "                                <div class='label'>Extra Working Days : </div>" +
                        "                                <div class='value'>" + salary.Extra_Working_Days + "</div>" +
                        "                            </div>";
                    }
                    sb += "                        <div class='entry'>" +
                    "                            <div class='label'>Worked Days : </div>" +
                    "                            <div class='value'>" + (salary.No_Of_Days - salary.Absentee + salary.Extra_Working_Days) + "</div>" +
                    "                        </div>" +
                    "                    </div>" +
                    "                    <div class='gross'>" +
                    "                        <div class='title'>Gross Total : </div>" +
                    "                        <div class='entry'>" +
                    "                            <div class='label'></div>" +
                    "                            <div class='value'>Rs. " + String.Format("{0:n0}", Math.Round(Convert.ToDecimal(salary.Grand_total))) + "</div>" +
                    "                        </div>" +
                    "                    </div>" +
                    "                </div>" +
                    "                <div class='right-panel'>" +
                    "                    <div class='details'>" +
                    "                        <div class='taxable_allowance'>" +
                    "                            <div class='entry'>" +
                    "                                <div class='label'>Basic Salary : </div>" +
                    "                                <div class='value'>" + string.Format("{0:n0}", salary.Basic_salary) + "</div>" +
                    "                            </div>" +
                    "                            <hr />";
                    if (salary.Extra_Working_Days != 0 && salary.Extra_Working_Days != null)
                    {
                        sb += "                                <div class='entry'>" +
                        "                                    <div class='label'>Extra Days Wage</div>" +
                        "                                </div>" +
                        "                                <div class='entry'>" +
                        "                                    <div class='label'></div>" +
                        "                                    <div class='detail'>Payment : </div>" +
                        "                                    <div class='amount'>" + String.Format("{0:n0}", (Math.Round(Convert.ToDecimal((salary.Basic_salary / 30) * salary.Extra_Working_Days)))) + "</div>" +
                        "                                </div>" +
                        "                                <hr />";
                    }
                    sb += "                            <div class='entry'>" +
                    "                                <div class='label'><u>Allowance and Bonus : </u></div>" +
                    "                            </div>";
                    if (details.Any())
                    {
                        foreach (var item2 in details.Where(x => x.Type == "AllowncesAndBonus"))
                        {
                            sb += "                                    <div class='entry'>" +
                            "                                        <div class='label'></div>" +
                            "                                        <div class='detail'>" + item2.Description + "</div>" +
                            "                                        <div class='amount'>" + String.Format("{0:n0}", Math.Round(Convert.ToDecimal(item2.Amount))) + "</div>" +
                            "                                    </div>";
                        }
                    }
                    if (salary.Bonus != null && salary.Bonus != 0)
                    {
                        sb += "                                <div class='entry'>" +
                        "                                    <div class='label'></div>" +
                        "                                    <div class='detail'>" + salary.Bonus_Description + "</div>" +
                        "                                    <div class='amount'>" + String.Format("{0:n0}", Math.Round(Convert.ToDecimal(salary.Bonus))) + "</div>" +
                        "                                </div>";
                    }
                    if (salary.Overtime != null && salary.Overtime != 0)
                    {
                        sb += "                                <div class='entry'>" +
                        "                                    <div class='label'></div>" +
                        "                                    <div class='detail'>Overtime : </div>" +
                        "                                    <div class='amount'>" + String.Format("{0:n0}", Math.Round(Convert.ToDecimal(salary.Overtime))) + "</div>" +
                        "                                </div>";
                    }
                    sb += "                            <div class='entry' style='border: 1px solid;'>" +
                    "                                <div class='label'>Total : </div>" +
                    "                                <div class='detail'></div>" +
                    "                                <div class='amount'>" +
                    "                                    " + String.Format("{0:n0}", Math.Round(Convert.ToDecimal((salary.Allownces) + (salary.Bonus) + ((salary.Basic_salary / 30) * (salary.Extra_Working_Days)) + (salary.Overtime) + (salary.Extra_Fuel_Charges)))) +
                    "                                </div>" +
                    "                            </div>" +
                    "                        </div>" +
                    "                        <hr />" +
                    "                        <div class='deductions'>" +
                    "                            <div class='entry'>" +
                    "                                <div class='label'><u>Deductions And Taxes : </u></div>" +
                    "                            </div>";
                    if (details.Any())
                    {
                        foreach (var item3 in details.Where(x => x.Type == "OtherDeduction"))
                        {
                            sb += "                                    <div class='entry'>" +
                            "                                        <div class='label'></div>" +
                            "                                        <div class='detail'>" + item3.Description + " : </div>" +
                            "                                        <div class='amount'>" + String.Format("{0:n0}", Math.Round(Convert.ToDecimal(item3.Amount))) + "</div>" +
                            "                                    </div>";
                        }
                    }
                    if (salary.Absentee != 0 && salary.Absentee != null)
                    {
                        sb += "                                <div class='entry'>" +
                        "                                    <div class='label'></div>" +
                        "                                    <div class='detail'>Absentee : </div>" +
                        "                                    <div class='amount'>" + String.Format("{0:n0}", Math.Round(Convert.ToDecimal((salary.Basic_salary / 30) * (salary.Absentee)))) + "</div>" +
                        "                                </div>" +
                        "                                <hr />";
                    }
                    if (salary.Extra_Fuel_Charges != 0 && salary.Extra_Fuel_Charges != null)
                    {
                        sb += "                                <div class='entry'>" +
                        "                                    <div class='label'></div>" +
                        "                                    <div class='detail'>Extra Fuel&nbsp;&nbsp;" + salary.Extra_Fuel + " &nbsp;&nbsp; ;&nbsp; Payment</div>" +
                        "                                    <div class='amount'>" + String.Format("{0:n0}", Math.Round(Convert.ToDecimal(salary.Extra_Fuel_Charges))) + "</div>" +
                        "                                </div>" +
                        "                                <hr />";
                    }
                    if (salary.Ded_Tax != 0 && salary.Ded_Tax != null)
                    {
                        sb += "                                <div class='entry'>" +
                        "                                    <div class='label'></div>" +
                        "                                    <div class='detail'>Tax : </div>" +
                        "                                    <div class='amount'>" + String.Format("{0:n0}", Math.Round(Convert.ToDecimal((salary.Ded_Tax)))) + "</div>" +
                        "                                </div>" +
                        "                                <hr />";
                    }
                    if (salary.Ded_Advance != 0 && salary.Ded_Advance != null)
                    {
                        sb += "                                <div class='entry'>" +
                        "                                    <div class='label'></div>" +
                        "                                    <div class='detail'>Advance : </div>" +
                        "                                    <div class='amount'>" + String.Format("{0:n0}", Math.Round(Convert.ToDecimal((salary.Ded_Advance)))) + "</div>" +
                        "                                </div>" +
                        "                                <hr />";
                    }
                    if (salary.Ded_Loan != 0 && salary.Ded_Loan != null)
                    {
                        sb += "                                <div class='entry'>" +
                        "                                    <div class='label'></div>" +
                        "                                    <div class='detail'>Loan : </div>" +
                        "                                    <div class='amount'>" + String.Format("{0:n0}", Math.Round(Convert.ToDecimal((salary.Ded_Loan)))) + "</div>" +
                        "                                </div>" +
                        "                                <hr />";
                    }
                    sb += "                            <div class='entry'>" +
                    "                                <div class='label'>Deduction Total : </div>" +
                    "                                <div class='detail'></div>" +
                    "                                <div class='amount'>" + String.Format("{0:n0}", Math.Round(Convert.ToDecimal((salary.Ded_Other + salary.Ded_Tax + salary.Ded_Absentee + salary.Ded_Advance + salary.Ded_Loan)))) + "</div>" +
                    "                            </div>" +
                    "                        </div>" +
                    "                        <hr>" +
                    "                        <div class='net_pay'>" +
                    "                            <div class='' style='width:100%'>" +
                    "                                <div class=''>NET PAY &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Cash:&nbsp; " + String.Format("{0:n0}", Math.Round(Convert.ToDecimal(salary.Cash))) + "   &nbsp; Bank:&nbsp;" + String.Format("{0:n0}", Math.Round(Convert.ToDecimal(salary.Bank))) + " <div style='float:right'> " + String.Format("{0:n0}", Math.Round(Convert.ToDecimal(salary.Grand_total))) + "</div>  </div>" +
                    "                            </div>" +
                    "                        </div>" +
                    "                        <br />" +
                    "                        <br />" +
                    "                    </div>" +
                    "                </div>" +
                    "            </div>" +
                    "            <br />" +
                    "            <span>--------------------------------------- MIS AUTO GENERATED SALARY SLIP (ERRORS AND OMISSIONS EXCEPTED)-------------------------------------------------------------</span>" +
                    "        </div>" +
                    "    </div>" +
                    "</body>" +
                    "</html>";

                    EmailService e = new EmailService();
                    e.SendEmail(sb, eml, "Salary Slip For Month : " + salary.Salary_Month.Value.ToString("MMM-yyyy"));
                }

                if (!string.IsNullOrEmpty(mobno))
                {
                    //send sms here
                    var msgtext = "Dear " + salary.Employee_Name + "\n\r," +
                           "Your salary has been received.\n\r" +
                           "Cash Amount : " + salary.Cash + ".\n\r" +
                           "Date : " + salary.Received_Date.Value.ToLongDateString() + ".\n\r" +
                           "Time : " + salary.Received_Date.Value.ToLongTimeString() + ".\n\r" +
                           "Please contact HR department in case of any issue.\n\r" +
                           "Meher Estate Developers.";
                    try
                    {
                        SmsService smsService = new SmsService();
                        smsService.SendMsg(msgtext, mobno);
                    }
                    catch (Exception ee)
                    {
                        //EmailService e = new EmailService();
                        //e.SendEmail(item.Name + " " + item.FileFormNumber, "taimoor@sasystems.solutions", "Msg Not Sent");
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public ActionResult AddSalaryAllowance(long Id)
        {
            long Userid = long.Parse(User.Identity.GetUserId());
            ViewBag.userid = Userid;
            var res = db.Sp_Get_Salary_Details(Id).ToList();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "View Salary Detail ", "Read", "Activity_Record", ActivityType.Salaries.ToString(), userid);
            db.Sp_Add_Salary_Log("View Salary Details", Userid, GeneralMethods.GetIPAddress(), DateTime.UtcNow, null, null, Id);
            return PartialView(res);
        }
        //public void SalaryMakingEntries(long Userid)
        //      {

        //          var res = db.Sp_Get_All_Employess_Salaries("Step_4").ToList();

        //          AccountHandlerController de = new AccountHandlerController();
        //          var salaryHead = de.SalaryExpense();
        //          var overtimeHead = de.OvertimeExpense();
        //          var bonusesHead = de.BonusExpense();
        //          var taxdeductionHead = de.TaxDeductions();
        //          var netSalaryHead = de.NetSalary();

        //          foreach (var person in res)
        //          {
        //              try
        //              {
        //                  var receivableAccount = de.OtherDeductions(person.Employee_code);
        //                  System.Threading.Thread accountsEntries = new System.Threading.Thread(() => {
        //                      FinalizeSalaryEntries(person, salaryHead, overtimeHead, bonusesHead, taxdeductionHead, netSalaryHead, receivableAccount, Userid);
        //                  });
        //                  accountsEntries.Start();
        //              }
        //              catch (Exception ex)
        //              {

        //                  db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
        //              }
        //          }
        //      }

        //public void FinalizeSalaryEntries(Sp_Get_All_Employess_Salaries_Result person, Sp_Get_CA_Head_MultiRef_3_Result salaryHead, Sp_Get_CA_Head_MultiRef_3_Result overtimeHead, Sp_Get_CA_Head_MultiRef_3_Result bonusesHead, Sp_Get_CA_Head_MultiRef_4_Result taxdeductionHead, Sp_Get_CA_Head_MultiRef_4_Result netSalaryHead, Sp_Get_CA_Head_MultiRef_Fixed_Param_4_Result receivableAccount, long Userid)
        //{
        //    List<GeneralEntryDetailsModel> Details = new List<GeneralEntryDetailsModel>();
        //    var tranId = new Helpers().RandomNumber();
        //    int j = 0;

        //    GeneralEntryDetailsModel geItems = new GeneralEntryDetailsModel();
        //    //Gross Salary Dr
        //    geItems.Narration = "Gross Salary of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
        //    geItems.HeadCode = salaryHead.Code;
        //    geItems.HeadName = salaryHead.Head;
        //    geItems.HeadId = salaryHead.Id;
        //    geItems.TransactionId = tranId;
        //    geItems.Line = ++j;
        //    geItems.Debit = person.Basic_salary + person.Allownces - person.Ded_Absentee;
        //    geItems.Credit = 0;
        //    Details.Add(geItems);
        //    //Overtime Dr
        //    if (person.Overtime > 0)
        //    {
        //        GeneralEntryDetailsModel geItems1 = new GeneralEntryDetailsModel();
        //        geItems1.Narration = "Overtime of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
        //        geItems1.HeadCode = overtimeHead.Code;
        //        geItems1.HeadName = overtimeHead.Head;
        //        geItems1.HeadId = overtimeHead.Id;
        //        geItems1.TransactionId = tranId;
        //        geItems1.Line = ++j;
        //        geItems1.Debit = person.Overtime;
        //        geItems1.Credit = 0;
        //        Details.Add(geItems1);
        //    }
        //    //Bonuses Dr
        //    if (person.Bonus > 0)
        //    {
        //        GeneralEntryDetailsModel geItems2 = new GeneralEntryDetailsModel();
        //        geItems2.Narration = "Bonus Salary (" + person.Bonus_Description + ") of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
        //        geItems2.HeadCode = bonusesHead.Code;
        //        geItems2.HeadName = bonusesHead.Head;
        //        geItems2.HeadId = bonusesHead.Id;
        //        geItems2.TransactionId = tranId;
        //        geItems2.Line = ++j;
        //        geItems2.Debit = person.Bonus;
        //        geItems2.Credit = 0;
        //        Details.Add(geItems2);
        //    }
        //    //Tax Deductions Cr
        //    if (person.Ded_Tax > 0)
        //    {
        //        GeneralEntryDetailsModel geItems3 = new GeneralEntryDetailsModel();
        //        geItems3.Narration = "Tax Deductions of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
        //        geItems3.HeadCode = taxdeductionHead.Code;
        //        geItems3.HeadName = taxdeductionHead.Head;
        //        geItems3.HeadId = taxdeductionHead.Id;
        //        geItems3.TransactionId = tranId;
        //        geItems3.Line = ++j;
        //        geItems3.Debit = 0;
        //        geItems3.Credit = person.Ded_Tax;
        //        Details.Add(geItems3);
        //    }
        //    //Loans Cr
        //    if (person.Ded_Loan > 0)
        //    {
        //        GeneralEntryDetailsModel geItems4 = new GeneralEntryDetailsModel();
        //        geItems4.Narration = "Loans Deductions of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
        //        geItems4.HeadCode = receivableAccount.Code;
        //        geItems4.HeadName = receivableAccount.Head;
        //        geItems4.HeadId = receivableAccount.Id;
        //        geItems4.TransactionId = tranId;
        //        geItems4.Line = ++j;
        //        geItems4.Debit = 0;
        //        geItems4.Credit = person.Ded_Loan;
        //        Details.Add(geItems4);
        //    }
        //    //Advances Cr
        //    if (person.Ded_Advance > 0)
        //    {
        //        GeneralEntryDetailsModel geItems5 = new GeneralEntryDetailsModel();
        //        geItems5.Narration = "Advances Deduction of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
        //        geItems5.HeadCode = receivableAccount.Code;
        //        geItems5.HeadName = receivableAccount.Head;
        //        geItems5.HeadId = receivableAccount.Id;
        //        geItems5.TransactionId = tranId;
        //        geItems5.Line = ++j;
        //        geItems5.Debit = 0;
        //        geItems5.Credit = person.Ded_Advance;
        //        Details.Add(geItems5);
        //    }
        //    //Other Deductions Cr
        //    if (person.Ded_Other > 0)
        //    {
        //        GeneralEntryDetailsModel geItems6 = new GeneralEntryDetailsModel();
        //        geItems6.Narration = "Other Deductions of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
        //        geItems6.HeadCode = receivableAccount.Code;
        //        geItems6.HeadName = receivableAccount.Head;
        //        geItems6.HeadId = receivableAccount.Id;
        //        geItems6.TransactionId = tranId;
        //        geItems6.Line = ++j;
        //        geItems6.Debit = 0;
        //        geItems6.Credit = person.Ded_Other;
        //        Details.Add(geItems6);
        //    }
        //    if (person.Extra_Fuel_Charges > 0)
        //    {
        //        GeneralEntryDetailsModel geItems7 = new GeneralEntryDetailsModel();
        //        geItems7.Narration = "Extra Fuel Charges Deductions of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
        //        geItems7.HeadCode = receivableAccount.Code;
        //        geItems7.HeadName = receivableAccount.Head;
        //        geItems7.HeadId = receivableAccount.Id;
        //        geItems7.TransactionId = tranId;
        //        geItems7.Line = ++j;
        //        geItems7.Debit = 0;
        //        geItems7.Credit = person.Extra_Fuel_Charges;
        //        Details.Add(geItems7);
        //    }
        //    //Net Salary Cr
        //    decimal? netsalary = person.Grand_total;
        //    if (person.Grand_total > 0)
        //    {
        //        GeneralEntryDetailsModel geItems8 = new GeneralEntryDetailsModel();
        //        geItems8.Narration = "Net Salary of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
        //        geItems8.HeadCode = netSalaryHead.Code;
        //        geItems8.HeadName = netSalaryHead.Head;
        //        geItems8.HeadId = netSalaryHead.Id;
        //        geItems8.TransactionId = tranId;
        //        geItems8.Line = ++j;
        //        geItems8.Debit = 0;
        //        geItems8.Credit = person.Grand_total;
        //        Details.Add(geItems8);
        //    }
        //    if (User.IsInRole("Head Cashier"))
        //    {
        //        var JournalEnt = new XElement("JournalEntries", Details.Select(x => new XElement("Entries",
        //            new XAttribute("Naration", x.Narration),
        //            new XAttribute("Head", (x.HeadName == null) ? "" : x.HeadCode + " - " + x.HeadName),
        //            new XAttribute("Head_Name", x.HeadName),
        //            new XAttribute("Head_Code", x.HeadCode),
        //            new XAttribute("Head_Id", x.HeadId),
        //            new XAttribute("Line", x.Line),
        //            new XAttribute("Qty", 0),
        //            new XAttribute("UOM", ""),
        //            new XAttribute("Rate", 0),
        //            new XAttribute("Debit", x.Debit),
        //            new XAttribute("Credit", x.Credit),
        //            new XAttribute("GroupId", x.TransactionId)
        //            ))).ToString();

        //        db.Sp_Add_Journal_Entries(JournalEnt, Userid);
        //    }
        //    else
        //    {
        //        var GeneralEnt = new XElement("GeneralEntries", Details.Select(x => new XElement("Entries",
        //            new XAttribute("Naration", x.Narration),
        //            new XAttribute("Head", (x.HeadName == null) ? "" : x.HeadCode + " - " + x.HeadName),
        //            new XAttribute("Head_Name", x.HeadName),
        //            new XAttribute("Head_Code", x.HeadCode),
        //            new XAttribute("Head_Id", x.HeadId),
        //            new XAttribute("Line", x.Line),
        //            new XAttribute("Qty", 0),
        //            new XAttribute("UOM", ""),
        //            new XAttribute("Rate", 0),
        //            new XAttribute("Debit", x.Debit),
        //            new XAttribute("Credit", x.Credit),
        //            new XAttribute("GroupId", x.TransactionId)
        //            ))).ToString();

        //        db.Sp_Add_General_Entries(GeneralEnt, Userid);
        //    }

        //}

        //public void Salarytest(long Userid)
        //{
        //    var res = db.Sp_Get_All_Employess_Salaries("Step_4").ToList();

        //    AccountHandlerController de = new AccountHandlerController();
        //    var salaryHead = de.SalaryExpense();
        //    var overtimeHead = de.OvertimeExpense();
        //    var bonusesHead = de.BonusExpense();
        //    var taxdeductionHead = de.TaxDeductions();
        //    var netSalaryHead = de.NetSalary();
        //    foreach (var person in res)
        //    {
        //        var receivableAccount = de.OtherDeductions(person.Employee_code);
        //        List<GeneralEntryDetailsModel> Details = new List<GeneralEntryDetailsModel>();
        //        var tranId = new Helpers().RandomNumber();
        //        int j = 0;

        //        GeneralEntryDetailsModel geItems = new GeneralEntryDetailsModel();
        //        //Gross Salary Dr
        //        geItems.Narration = "Gross Salary of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
        //        geItems.HeadCode = salaryHead.Code;
        //        geItems.HeadName = salaryHead.Head;
        //        geItems.HeadId = salaryHead.Id;
        //        geItems.TransactionId = tranId;
        //        geItems.Line = ++j;
        //        geItems.Debit = person.Basic_salary + person.Allownces - person.Ded_Absentee;
        //        geItems.Credit = 0;
        //        Details.Add(geItems);
        //        //Overtime Dr
        //        if (person.Overtime > 0)
        //        {
        //            GeneralEntryDetailsModel geItems1 = new GeneralEntryDetailsModel();
        //            geItems1.Narration = "Overtime of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
        //            geItems1.HeadCode = overtimeHead.Code;
        //            geItems1.HeadName = overtimeHead.Head;
        //            geItems1.HeadId = overtimeHead.Id;
        //            geItems1.TransactionId = tranId;
        //            geItems1.Line = ++j;
        //            geItems1.Debit = person.Overtime;
        //            geItems1.Credit = 0;
        //            Details.Add(geItems1);
        //        }
        //        //Bonuses Dr
        //        if (person.Bonus > 0)
        //        {
        //            GeneralEntryDetailsModel geItems2 = new GeneralEntryDetailsModel();
        //            geItems2.Narration = "Bonus Salary (" + person.Bonus_Description + ") of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
        //            geItems2.HeadCode = bonusesHead.Code;
        //            geItems2.HeadName = bonusesHead.Head;
        //            geItems2.HeadId = bonusesHead.Id;
        //            geItems2.TransactionId = tranId;
        //            geItems2.Line = ++j;
        //            geItems2.Debit = person.Bonus;
        //            geItems2.Credit = 0;
        //            Details.Add(geItems2);
        //        }
        //        //Tax Deductions Cr
        //        if (person.Ded_Tax > 0)
        //        {
        //            GeneralEntryDetailsModel geItems3 = new GeneralEntryDetailsModel();
        //            geItems3.Narration = "Tax Deductions of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
        //            geItems3.HeadCode = taxdeductionHead.Code;
        //            geItems3.HeadName = taxdeductionHead.Head;
        //            geItems3.HeadId = taxdeductionHead.Id;
        //            geItems3.TransactionId = tranId;
        //            geItems3.Line = ++j;
        //            geItems3.Debit = 0;
        //            geItems3.Credit = person.Ded_Tax;
        //            Details.Add(geItems3);
        //        }
        //        //Loans Cr
        //        if (person.Ded_Loan > 0)
        //        {
        //            GeneralEntryDetailsModel geItems4 = new GeneralEntryDetailsModel();
        //            geItems4.Narration = "Loans Deductions of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
        //            geItems4.HeadCode = receivableAccount.Code;
        //            geItems4.HeadName = receivableAccount.Head;
        //            geItems4.HeadId = receivableAccount.Id;
        //            geItems4.TransactionId = tranId;
        //            geItems4.Line = ++j;
        //            geItems4.Debit = 0;
        //            geItems4.Credit = person.Ded_Loan;
        //            Details.Add(geItems4);
        //        }
        //        //Advances Cr
        //        if (person.Ded_Advance > 0)
        //        {
        //            GeneralEntryDetailsModel geItems5 = new GeneralEntryDetailsModel();
        //            geItems5.Narration = "Advances Deduction of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
        //            geItems5.HeadCode = receivableAccount.Code;
        //            geItems5.HeadName = receivableAccount.Head;
        //            geItems5.HeadId = receivableAccount.Id;
        //            geItems5.TransactionId = tranId;
        //            geItems5.Line = ++j;
        //            geItems5.Debit = 0;
        //            geItems5.Credit = person.Ded_Advance;
        //            Details.Add(geItems5);
        //        }
        //        //Other Deductions Cr
        //        if (person.Ded_Other > 0)
        //        {
        //            GeneralEntryDetailsModel geItems6 = new GeneralEntryDetailsModel();
        //            geItems6.Narration = "Other Deductions of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
        //            geItems6.HeadCode = receivableAccount.Code;
        //            geItems6.HeadName = receivableAccount.Head;
        //            geItems6.HeadId = receivableAccount.Id;
        //            geItems6.TransactionId = tranId;
        //            geItems6.Line = ++j;
        //            geItems6.Debit = 0;
        //            geItems6.Credit = person.Ded_Other;
        //            Details.Add(geItems6);
        //        }
        //        if (person.Extra_Fuel_Charges > 0)
        //        {
        //            GeneralEntryDetailsModel geItems7 = new GeneralEntryDetailsModel();
        //            geItems7.Narration = "Extra Fuel Charges Deductions of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
        //            geItems7.HeadCode = receivableAccount.Code;
        //            geItems7.HeadName = receivableAccount.Head;
        //            geItems7.HeadId = receivableAccount.Id;
        //            geItems7.TransactionId = tranId;
        //            geItems7.Line = ++j;
        //            geItems7.Debit = 0;
        //            geItems7.Credit = person.Extra_Fuel_Charges;
        //            Details.Add(geItems7);
        //        }
        //        //Net Salary Cr
        //        decimal? netsalary = person.Grand_total;
        //        if (person.Grand_total > 0)
        //        {
        //            GeneralEntryDetailsModel geItems8 = new GeneralEntryDetailsModel();
        //            geItems8.Narration = "Net Salary of " + person.Employee_Name + " - " + person.Employee_code + " for the month " + person.Salary_Month.Value.ToString("MM/yyyy");
        //            geItems8.HeadCode = netSalaryHead.Code;
        //            geItems8.HeadName = netSalaryHead.Head;
        //            geItems8.HeadId = netSalaryHead.Id;
        //            geItems8.TransactionId = tranId;
        //            geItems8.Line = ++j;
        //            geItems8.Debit = 0;
        //            geItems8.Credit = person.Grand_total;
        //            Details.Add(geItems8);
        //        }
        //        if (User.IsInRole("Head Cashier"))
        //        {
        //            var JournalEnt = new XElement("JournalEntries", Details.Select(x => new XElement("Entries",
        //                new XAttribute("Naration", x.Narration),
        //                new XAttribute("Head", (x.HeadName == null) ? "" : x.HeadCode + " - " + x.HeadName),
        //                new XAttribute("Head_Name", x.HeadName),
        //                new XAttribute("Head_Code", x.HeadCode),
        //                new XAttribute("Head_Id", x.HeadId),
        //                new XAttribute("Line", x.Line),
        //                new XAttribute("Qty", 0),
        //                new XAttribute("UOM", ""),
        //                new XAttribute("Rate", 0),
        //                new XAttribute("Debit", x.Debit),
        //                new XAttribute("Credit", x.Credit),
        //                new XAttribute("GroupId", x.TransactionId)
        //                ))).ToString();

        //            db.Sp_Add_Journal_Entries(JournalEnt, Userid);
        //        }
        //        else
        //        {
        //            var GeneralEnt = new XElement("GeneralEntries", Details.Select(x => new XElement("Entries",
        //                new XAttribute("Naration", x.Narration),
        //                new XAttribute("Head", (x.HeadName == null) ? "" : x.HeadCode + " - " + x.HeadName),
        //                new XAttribute("Head_Name", x.HeadName),
        //                new XAttribute("Head_Code", x.HeadCode),
        //                new XAttribute("Head_Id", x.HeadId),
        //                new XAttribute("Line", x.Line),
        //                new XAttribute("Qty", 0),
        //                new XAttribute("UOM", ""),
        //                new XAttribute("Rate", 0),
        //                new XAttribute("Debit", x.Debit),
        //                new XAttribute("Credit", x.Credit),
        //                new XAttribute("GroupId", x.TransactionId)
        //                ))).ToString();

        //            db.Sp_Add_General_Entries(GeneralEnt, Userid);
        //        }

        //    }
        //}
        public JsonResult GetBankDetails(string s)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            AccountHandlerController ah = new AccountHandlerController();
            var comp = ah.Company_Attr(userid);
            var bankdet = ah.HeadFinder(COA_Mapper_Modules.Accounting.ToString(), COA_Mapper_ModuleTypes.Bank_Account.ToString(), COA_Mapper_HeadType.Account_Head.ToString(), comp.Id, null);
            var allsearch = db.Sp_Get_CA_LeaveNodes_HeadSearch_Parameter(s, bankdet.Text_ChartCode, comp.COA).Select(x => new { id = x.Id, text = x.Head /*text = x.Text_ChartCode + " - " + x.Head*/ }).ToList();
            return Json(new { items = allsearch }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployees_Details(string s)
        {
            var result = db.Employees.Where(x => x.Name.Contains(s)).Select(x => new Employee_Search_Bank { Id = x.Id, Name = x.Name, EmpCode = x.Employee_Code, DeptDesig = x.Department_Designation }).ToList();

            return Json(new { items = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SalaryBankConfig()
        {
            var result = db.Employees.Where(x => x.Status == "Registered" && x.BankId != null && x.BankName != null).ToList();
            return View(result);
        }
        public ActionResult AddNewBankConfig()
        {
            return PartialView();
        }
        //Obselete:
        public JsonResult SaveSalaryBankConfig(List<SalaryBankEmployee> stm)
        {
            if (stm is null)
            {
                return Json(false);
            }

            var res = db.MIS_Modules_Configurations.Where(x => x.Module == MIS_Modules.Salary_Bank.ToString()).FirstOrDefault();
            var uid = User.Identity.GetUserId<long>();
            var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
            long userid = long.Parse(User.Identity.GetUserId());
            db.Sp_Add_Activity(userid, "Salary Bank Configured for " + string.Join(",", stm.Select(x => x.EmployeeName)), "Update", "Activity_Record", ActivityType.Salaries.ToString(), userid);

            if (res is null)
            {
                //First time
                MIS_Modules_Configurations __m = new MIS_Modules_Configurations
                {
                    CurrentConfig = JsonConvert.SerializeObject(stm),
                    LastUpdated = DateTime.Now,
                    Module = MIS_Modules.Salary_Bank.ToString(),
                    UpdatedBy_Id = uid,
                    UpdatedBy_Name = uname
                };
                db.MIS_Modules_Configurations.Add(__m);
                db.SaveChanges();
                return Json(true);
            }
            else
            {
                //already exists
                res.CurrentConfig = JsonConvert.SerializeObject(stm);
                res.UpdatedBy_Id = uid;
                res.UpdatedBy_Name = uname;
                res.LastUpdated = DateTime.Now;
                db.MIS_Modules_Configurations.Attach(res);
                db.Entry(res).Property(x => x.CurrentConfig).IsModified = true;
                db.Entry(res).Property(x => x.UpdatedBy_Name).IsModified = true;
                db.Entry(res).Property(x => x.UpdatedBy_Id).IsModified = true;
                db.Entry(res).Property(x => x.LastUpdated).IsModified = true;
                db.SaveChanges();
                return Json(true);
            }
        }
        public Sp_Get_CA_Head_Parameter_Result GetHead(int Id)
        {
            var res = db.Sp_Get_CA_Head_Parameter(Id).FirstOrDefault();
            return res;
        }
        public JsonResult GetEmployeeDetails(long EmpId)
        {
            var res = db.Sp_Get_Employee_Parameter(EmpId).FirstOrDefault();
            return Json(res);
        }
        public JsonResult UpdateEmployeeBankHead(long EmpId, int HeadId, string HeadName)
        {
            var bank = HeadName;
            var bankName = HeadName.Split('-');
            if (bankName[1] != null)
            {
                bank = bankName[1];
                bank.Trim();
            }
            db.Sp_Update_Employee_BankDetails(EmpId, bank, HeadId);
            return Json(true);
        }
        public ActionResult AddBankAccountDetailsStep3()
        {
            var res = db.Employees.Where(x => x.Status == "Registered" && x.BankId != null).ToList();
            return PartialView(res);
        }
        public JsonResult AddStandingOrderDetails(List<StandingOrderDetails> standingOrders)
        {
            foreach (var v in standingOrders)
            {
                db.Sp_Update_StandingOrderDetails(v.HeadId, v.InstNo, v.InstDate);
            }
            return Json(true);
        }
        public JsonResult RemoveEmployeeBankAccount(long EmployeeId)
        {
            db.Sp_Update_Employee_Bank_Account(EmployeeId);
            return Json(true);
        }
        public void transsal()
        {
            var res = db.Sp_Get_All_Employess_Salaries("Step_4").ToList();

            using (var Transaction = db.Database.BeginTransaction())
            {
                try
                {
                    Helpers h = new Helpers();
                   
                    foreach (var v in res.GroupBy(x => x.CompId))
                    {
                        var TransactionId = h.RandomNumber();
                        var vouch = db.Sp_Add_Journal_Voucher().FirstOrDefault();
                        decimal? BasicSalary = v.Sum(x => x.Act_Salary) + v.Sum(x => x.Stipend);
                        decimal? Allowance = v.Sum(x => x.Allownces);
                        decimal? Bonus = v.Sum(x => x.Bonus);
                        decimal? Overtime = v.Sum(x => x.Overtime);
                        decimal? TaxDeduction = v.Sum(x => x.Ded_Tax);
                        decimal? LoanDeduction = v.Sum(x => x.Ded_Loan);
                        decimal? AdvanceDeduction = v.Sum(x => x.Ded_Advance);
                        decimal? OtherDeduction = v.Sum(x => x.Ded_Other);
                        decimal? ExtraFuel = v.Sum(x => x.Extra_Fuel_Charges);
                        decimal? NetSalary = v.Sum(x => x.Grand_total);

                        AccountHandlerController de = new AccountHandlerController();
                        de.Salary_Posting(BasicSalary, Allowance, Bonus, Overtime, TaxDeduction, LoanDeduction, AdvanceDeduction, OtherDeduction, ExtraFuel, NetSalary, TransactionId, 20058, vouch, true, COA_Mapper_Modules.Human_Resource.ToString(), v.Key.Value, v.Select(x => x.Salary_Month).FirstOrDefault());
                    }
                    Transaction.Commit();
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    db.Sp_Add_ErrorLog(ex.Message, (ex.InnerException is null) ? string.Empty : ex.InnerException.ToString(), ex.StackTrace, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                }
            }
        }

    }
}
