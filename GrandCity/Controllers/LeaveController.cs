//using MeherEstateDevelopers.Filters;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.UI.WebControls;
//using System.Xml.Linq;
//using MeherEstateDevelopers.Models;
//using Microsoft.AspNet.Identity;
//using Newtonsoft.Json;
//using Hangfire.Annotations;
//namespace MeherEstateDevelopers.Controllers
//{
//    [LogAction]
//    [Authorize]
//    public class LeaveController : Controller
//    {
//        private Grand_CityEntities db = new Grand_CityEntities();
//        // Main View Of Leaves
//        private FinancialYear GetHRFiscalYear()
//        {
//            return new FinancialYear { Start = new DateTime(2022, 1, 1), End = new DateTime(2022, 12, 31) };
//        }
//        public ActionResult UserLeaves()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//            ViewBag.EmployeeId = EmpId;
//            return View();
//        }
//        // manager
//        public ActionResult AllLeaves()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var Empid = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//            var result = db.Sp_Get_LeaveManagement_Manager(Empid, LeaveStatus.Pending.ToString()).ToList();
//            db.Sp_Add_Activity(userid, "Accessed All Leaves Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
//            return PartialView(result);
//        }
//        public ActionResult ApprovedLeaves()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var Empid = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//            var result = db.Sp_Get_LeaveManagement_Manager(Empid, LeaveStatus.Approved.ToString()).ToList();
//            db.Sp_Add_Activity(userid, "Accessed Approved Leaves  Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
//            return PartialView(result);
//        }
//        public ActionResult RejectedLeaves()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var Empid = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//            var result = db.Sp_Get_LeaveManagement_Manager(Empid, LeaveStatus.Rejected.ToString()).ToList();
//            db.Sp_Add_Activity(userid, "Accessed Rejected Leaves Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
//            return PartialView(result);
//        }
//        // Employee
//        public ActionResult UserLeavesPending(long EmpId)
//        {
//            var res = db.Sp_Get_EmployeeLeave_Management(LeaveStatus.Pending.ToString(), EmpId).ToList();
//            return PartialView(res);
//        }
//        public ActionResult UserLeavesApproved(long EmpId)
//        {
//            var res = db.Sp_Get_EmployeeLeave_Management(LeaveStatus.Approved.ToString(), EmpId).ToList();
//            return PartialView(res);
//        }
//        public ActionResult UserLeavesRejected(long EmpId)
//        {
//            var res = db.Sp_Get_EmployeeLeave_Management(LeaveStatus.Rejected.ToString(), EmpId).ToList();
//            return PartialView(res);
//        }
//        // Leave For others
//        public ActionResult LeaveForOther()
//        {
//            if (User.IsInRole("Leave For Others"))
//            {
//                long userid = long.Parse(User.Identity.GetUserId());
//                var emps = db.Sp_Get_Employee().Where(x => x.Status == "Registered").ToList();
//                emps.ForEach(x => x.Name += " ( " + x.Employee_Code + " ) ");
//                ViewBag.Employee = new SelectList(emps, "Id", "Name", "Department", 1);
//                return PartialView();
//            }
//            else if (User.IsInRole("Leave For Department"))
//            {
//                long uid = User.Identity.GetUserId<long>();
//                var empId = db.Sp_Get_EmployeeId(uid).FirstOrDefault();
//                var deps = db.Employee_Designations.Where(x => x.Emp_Id == empId).ToList();
//                List<Sp_Get_DepartmentEmployees_Result> depEmpsList = new List<Sp_Get_DepartmentEmployees_Result>();
//                foreach (var v in deps)
//                {
//                    depEmpsList.AddRange(db.Sp_Get_DepartmentEmployees(v.Department_Id).ToList());
//                }
//                depEmpsList.ForEach(x => x.Name += " ( " + x.Employee_Code + " ) ");
//                ViewBag.Employee = new SelectList(depEmpsList, "Id", "Name", 1);
//                return PartialView();
//            }
//            return PartialView();
//        }
//        public ActionResult OtherPending()
//        {
//            var res = db.Sp_Get_LeaveManagement_Other(LoanStatus.Pending.ToString()).ToList();
//            return PartialView(res);
//        }
//        public ActionResult OtherApproved()
//        {
//            var res = db.Sp_Get_LeaveManagement_Other(LoanStatus.Approved.ToString()).ToList();
//            ViewBag.Type = "Approved";
//            return PartialView(res);
//        }
//        public ActionResult OtherRejected()
//        {
//            var res = db.Sp_Get_LeaveManagement_Other(LoanStatus.Rejected.ToString()).ToList();
//            ViewBag.Type = "Rejected";
//            return PartialView("OtherApproved", res);
//        }
//        public ActionResult DeptPending()
//        {
//            var uid = User.Identity.GetUserId<long>();
//            var res = db.Sp_Get_DepLeaves(uid, LoanStatus.Pending.ToString()).ToList();
//            return PartialView(res);
//        }
//        public ActionResult DepApproved()
//        {
//            var uid = User.Identity.GetUserId<long>();
//            var res = db.Sp_Get_DepLeaves(uid, LoanStatus.Approved.ToString()).ToList();
//            return PartialView("DeptPending", res);
//        }
//        public ActionResult DeptRejected()
//        {
//            var uid = User.Identity.GetUserId<long>();
//            var res = db.Sp_Get_DepLeaves(uid, LoanStatus.Rejected.ToString()).ToList();
//            return PartialView("DeptPending", res);
//        }
//        // create and requisition
//        public ActionResult Create(long Id, string attDate)
//        {
//            ViewBag.defDate = attDate;
//            var Leaves = db.Sp_Get_EmployeeLeaves(Id).ToList();
//            ViewBag.EmployeeId = Id;
//            if (Leaves == null || Leaves.Count() <= 0)
//            {
//                Leaves = db.Sp_Get_EmployeeLeaves(Id).ToList();
//                ViewBag.EmployeeId = Id;
//            }
//            ViewBag.TotalOfficialsLeaves = db.Sp_Get_OfficialLeaves().Count();

//            var onGoingMon = DateTime.Now;

//            string config_Dets = db.MIS_Modules_Configurations.Where(x => x.Module == MIS_Modules.HR.ToString()).Select(x => x.Config_For_Update).FirstOrDefault();
//            HRConfiguration parsedConfig = JsonConvert.DeserializeObject<HRConfiguration>(config_Dets);
//            ViewBag.fiscStart = new DateTime(onGoingMon.Year, onGoingMon.AddMonths(-1).Month, parsedConfig.PayCycleStart.Day);
//            ViewBag.fiscEnd = new DateTime(onGoingMon.Year, onGoingMon.Month, parsedConfig.PayCycleEnd.Day);



//            return PartialView(Leaves);
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public JsonResult CreateLeave(LeaveRequisition Lr)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            if (Lr.LeaveType == "SPL")
//            {
//                Lr.CreatedAt = DateTimeOffset.UtcNow.ToLocalTime();
//                Lr.OfficalLeave = 0;
//                Lr.ManagerApproval = LeaveStatus.Approved.ToString();
//                Lr.HrApproval = LeaveStatus.Approved.ToString();
//                Lr.Cancel = false;
//                Lr.AppliedBy = User.Identity.GetUserId<long>();
//                var res = db.Sp_Add_Leave(Lr.StartDate, Lr.EndDate, Lr.Reason, Lr.EmpId, Lr.NoOfDays,
//                Lr.OfficalLeave, Lr.ManagerApproval, Lr.AppliedBy, Lr.LeaveType).FirstOrDefault();
//                return Json(new Return { Msg = "Leave application has been placed successfully", Status = true });
//            }
//            else
//            {
//                var fisc = GetHRFiscalYear();
//                if (Lr.LeaveType == "HD")
//                {
//                    if (Lr.NoOfDays != 1)
//                    {
//                        return Json(new Return { Msg = "Start Date and End Date Should be Same for Half Day Leave Application." });
//                    }
//                    Lr.LeaveType = "Casual";
//                    var reamainingCasual = db.Sp_Get_EmployeeLeaves(Lr.EmpId).Where(x => x.LeaveType == Lr.LeaveType && x.EmpId == Lr.EmpId && x.From >= fisc.Start && x.To <= fisc.End).FirstOrDefault();
//                    //   Checking of Remaining leave from Sick if Casual leaves is 0
//                    if (reamainingCasual.Remaining.ToString() == "0" )
//                    {
//                        //For Sick Leave
//                        Lr.LeaveType = "Sick";
//                        var reamainingSick = db.Sp_Get_EmployeeLeaves(Lr.EmpId).Where(x => x.LeaveType == Lr.LeaveType && x.EmpId == Lr.EmpId && x.From >= fisc.Start && x.To <= fisc.End).FirstOrDefault();

//                        if (reamainingSick.Remaining.ToString() == "0")
//                        {
//                            return Json(new Return { Msg = "You have no remaining Leaves." });

//                        }
//                        else if (reamainingSick.Remaining < 0.5)
//                        {
//                            return Json(new Return { Msg = "You can't apply more than remaining Leaves" });
//                        }
//                        db.Sp_Add_Activity(userid, "Created New Leave " + Lr.Reason, "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
//                        Lr.CreatedAt = DateTimeOffset.UtcNow.ToLocalTime();
//                        Lr.OfficalLeave = 0;
//                        Lr.ManagerApproval = LeaveStatus.Pending.ToString();
//                        Lr.HrApproval = LeaveStatus.Pending.ToString();
//                        Lr.Cancel = false;
//                        Lr.AppliedBy = User.Identity.GetUserId<long>();
//                        var res = db.Sp_Add_HD_Leaves(Lr.StartDate, Lr.EndDate, Lr.Reason, Lr.EmpId, 0,
//                             Lr.OfficalLeave, null, Lr.ManagerApproval, Lr.AppliedBy, Lr.LeaveType, "HalfDay").FirstOrDefault();

//                        if (Convert.ToInt32(res) == 1)
//                        {
//                            return Json(new Return { Msg = "First HalfDay Leave application has been placed successfully", Status = true });
//                        }
//                        else if (Convert.ToInt32(res) == 2)
//                        {
//                            var spent = db.LeaveRequisitions.Where(x => x.EmpId == Lr.EmpId && x.Status == 2 && x.AvailLeave=="HalfDay").FirstOrDefault();
//                            db.Sp_Update_Leave_HD_Status(Lr.EmpId);
//                            var deduct = db.LeaveRequisitions.Where(x => x.EmpId == Lr.EmpId && x.Status == null && x.AvailLeave == "HalfDay").FirstOrDefault();
//                            var Sick = db.Employee_Leaves.Where(x => x.EmpId == Lr.EmpId && x.LeaveType == "Sick" && x.From >= fisc.Start && x.To <= fisc.End).FirstOrDefault();
//                            var add = Sick.Spent + 1;
//                            db.Sp_Update_HD_Spent_Leaves(Sick.Id, add);
//                            return Json(new Return { Msg = "Second HalfDay Leave application has been placed successfully", Status = true });
//                        }
//                        else
//                        {
//                            return Json(new Return { Msg = "Cannot place this leave application. A leave application with these dates already exists.", Status = false });
//                        }

//                    }
//                    else
//                    {
//                        //For Casual Leave
//                        if (reamainingCasual.Remaining.ToString() == "0")
//                        {
//                            return Json(new Return { Msg = "You have no remaining Leaves." });
//                        }
//                        else if (reamainingCasual.Remaining < 0.5)
//                        {
//                            return Json(new Return { Msg = "You can't apply more than remaining Leaves" });
//                        }
//                        db.Sp_Add_Activity(userid, "Created New Leave " + Lr.Reason, "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
//                        Lr.CreatedAt = DateTimeOffset.UtcNow.ToLocalTime();
//                        Lr.OfficalLeave = 0;
//                        Lr.ManagerApproval = LeaveStatus.Pending.ToString();

//                        Lr.HrApproval = LeaveStatus.Pending.ToString();
//                        Lr.Cancel = false;
//                        Lr.AppliedBy = User.Identity.GetUserId<long>();
//                        var res = db.Sp_Add_HD_Leaves(Lr.StartDate, Lr.EndDate, Lr.Reason, Lr.EmpId, 0,
//                             Lr.OfficalLeave, null, Lr.ManagerApproval, Lr.AppliedBy, Lr.LeaveType, "HalfDay").FirstOrDefault();

//                        if (Convert.ToInt32(res) == 1)
//                        {
//                            return Json(new Return { Msg = "First HalfDay Leave application has been placed successfully", Status = true });
//                        }
//                        else if (Convert.ToInt32(res) == 2)
//                        {
//                            var spent = db.LeaveRequisitions.Where(x => x.EmpId == Lr.EmpId && x.Status == 2 && x.AvailLeave == "HalfDay").FirstOrDefault();
//                            db.Sp_Update_Leave_HD_Status(Lr.EmpId);
//                            var deduct = db.LeaveRequisitions.Where(x => x.EmpId == Lr.EmpId && x.Status == null && x.AvailLeave == "HalfDay").FirstOrDefault();
//                            var Casual = db.Employee_Leaves.Where(x => x.EmpId == Lr.EmpId && x.LeaveType == "Casual" && x.From >= fisc.Start && x.To <= fisc.End).FirstOrDefault();
//                            var add = Casual.Spent + 1;
//                            db.Sp_Update_HD_Spent_Leaves(Casual.Id, add);
//                            return Json(new Return { Msg = "Second HalfDay  Leave application has been placed successfully", Status = true });
//                        }
//                        else
//                        {
//                            return Json(new Return { Msg = "Cannot place this leave application. A leave application with these dates already exists.", Status = false });
//                        }
//                    }
//                }
//                else
//                {
//                    var reamaining = db.Sp_Get_EmployeeLeaves(Lr.EmpId).Where(x => x.LeaveType == Lr.LeaveType && x.EmpId == Lr.EmpId && x.From >= fisc.Start && x.To <= fisc.End).FirstOrDefault();
//                    //var reamaining = db.Employee_Leaves.Where( x => x.LeaveType == Lr.LeaveType && x.EmpId == Lr.EmpId && x.From >= fisc.Start && x.To <= fisc.End).OrderByDescending(x=> x.Id).FirstOrDefault();
//                    if (Lr.LeaveType != "CPL")
//                    {
//                        if (reamaining.Remaining < Lr.NoOfDays)
//                        {
//                            return Json(new Return { Msg = "You can't apply more than remaining Leaves" });
//                        }
//                        if (Lr.LeaveType == "Annual" && Lr.NoOfDays < 3)
//                        {
//                            return Json(new Return { Msg = "You can't apply less than 3 Annual leaves ", Status = false });
//                        }
//                    }

//                    db.Sp_Add_Activity(userid, "Created New Leave " + Lr.Reason, "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
//                    Lr.CreatedAt = DateTimeOffset.UtcNow.ToLocalTime();
//                    Lr.OfficalLeave = 0;
//                    Lr.ManagerApproval = LeaveStatus.Pending.ToString();
//                    Lr.HrApproval = LeaveStatus.Pending.ToString();
//                    Lr.Cancel = false;
//                    Lr.AppliedBy = User.Identity.GetUserId<long>();
//                    var res = db.Sp_Add_Leave(Lr.StartDate, Lr.EndDate, Lr.Reason, Lr.EmpId, Lr.NoOfDays,
//                        Lr.OfficalLeave, Lr.ManagerApproval, Lr.AppliedBy, Lr.LeaveType).FirstOrDefault();
//                    if (res == true)
//                    {
//                        return Json(new Return { Msg = "Leave application has been placed successfully", Status = true });
//                    }
//                    else
//                    {
//                        return Json(new Return { Msg = "Cannot place this leave application. A leave application with these dates already exists.", Status = false });
//                    }
//                }
//            }
//        }
//        [HttpGet]
//        public JsonResult LeaveRequest(long? userid)
//        {
//            var totalleave = db.Sp_Get_LeaveRequisition(userid).ToList(); // get total leaves
//            var officailLeaves = db.Sp_Get_OfficialLeaves().ToList();     // get Official Leaves
//            long officialleavecount = 0, leavesCountStAl = 0, leavesCountStSCL = 0, leavesCountCL = 0;
//            foreach (var items in totalleave)
//            {
//                //leavesCount = leavesCount + Convert.ToInt64(items.NoOfDays);
//                if (items.LeaveType == "Annual")
//                {
//                    leavesCountStAl += Convert.ToInt64(items.NoOfDays);
//                }
//                else if (items.LeaveType == "Sick")
//                {
//                    leavesCountStSCL += Convert.ToInt64(items.NoOfDays);
//                }
//                else if (items.LeaveType == "Causual")
//                {
//                    leavesCountCL += Convert.ToInt64(items.NoOfDays);
//                }
//                else
//                {
//                }
//            }
//            foreach (var item in officailLeaves)
//            {
//                officialleavecount += Convert.ToInt64(item.NoOfDays);
//            }
//            //DateTime[] startDate = new DateTime[leavesCount];
//            DateTime[] officialDates = new DateTime[officialleavecount];
//            //My code to map Leaves according to LeaveTypes on Calender goes here
//            DateTime[] startDateAL = new DateTime[leavesCountStAl];
//            DateTime[] startDateSCL = new DateTime[leavesCountStSCL];
//            DateTime[] startDateCL = new DateTime[leavesCountCL];
//            int index = 0, index1 = 0, index2 = 0, index3 = 0;
//            foreach (var item2 in totalleave)
//            {
//                var start = item2.StartDate;
//                var end = item2.EndDate;
//                for (DateTime date = start; date <= end; date = date.AddDays(1))
//                {
//                    if (item2.LeaveType == "Causual")
//                    {
//                        startDateCL[index3] = date.Date;
//                        index3++;
//                    }
//                    else if (item2.LeaveType == "Sick")
//                    {
//                        startDateSCL[index] = date.Date;
//                        index++;
//                    }
//                    if (item2.LeaveType == "Annual")
//                    {
//                        startDateAL[index2] = date.Date;
//                        index2++;
//                    }
//                    else
//                    {
//                    }
//                }
//            }
//            foreach (var item2 in officailLeaves)
//            {
//                var start = item2.StartDate;
//                var end = item2.EndDate;
//                for (DateTime date = start; date <= end; date = date.AddDays(1))
//                {
//                    officialDates[index1] = date.Date;
//                    index1 = index1 + 1;
//                }
//            }
//            var result = new { startDateAL, startDateSCL, startDateCL, officialDates };
//            return Json(result, JsonRequestBehavior.AllowGet);
//        }
//        public bool CheckSatSun(DateTime date)
//        {
//            string day = Convert.ToString(date.DayOfWeek);
//            if (day == "Saturday" || day == "Sunday")
//            {
//                return false;
//            }
//            else
//            {
//                return true;
//            }
//        }
//        //popup for pendings
//        [HttpPost]
//        public ActionResult AllPendingLeaves(long Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = db.Sp_Get_LeaveDetails(Id).SingleOrDefault();
//            var EmpId = db.Sp_Get_EmployeeId(userid).SingleOrDefault();
//            ViewBag.Manager = (db.Sp_Get_ManagerStatus(EmpId).Count() > 1) ? true : false;
//            return PartialView(res);
//        }
//        public JsonResult UpdateStatus(long Id, string Manger_Remarks, string Manger_Status, string HR_Remarks, string HR_Status, bool? Manager, string lvtype)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            if (User.IsInRole("HR Manager") || User.IsInRole("Leave For Others") || User.IsInRole("Administrator"))
//            {
//                db.Sp_Update_Status_HR(Id, HR_Remarks, HR_Status);
//                var res = db.LeaveRequisitions.Where(x => x.Id == Id).SingleOrDefault();
//                db.Sp_Update_EmployeeLeave(res.EmpId, res.LeaveType, res.NoOfDays);
//            }
//            var availleave = db.LeaveRequisitions.Where(x => x.Id == Id).Select(x => new { x.LeaveType, x.NoOfDays, x.Status, x.EmpId, x.AvailLeave }).SingleOrDefault();
//            if (Manger_Status == "Rejected" && availleave.AvailLeave == "HalfDay")
//            {
//                if (availleave.Status == 1)
//                {
//                    db.Sp_Update_LeaveRequisition_HDStatus(Id);
//                    db.Sp_Update_LeaveStatus_Manager(Id, Manger_Remarks, Manger_Status);
//                    return Json(false);
//                }
//                var LT = db.Employee_Leaves.Where(x => x.EmpId == availleave.EmpId && x.LeaveType == availleave.LeaveType).FirstOrDefault();
//                var deduct = LT.Spent - 1;
//                db.Sp_Update_HD_Spent_Leaves(LT.Id, deduct);
//                db.Sp_Update_LeaveStatus_Manager(Id, Manger_Remarks, Manger_Status);
//                return Json(false);
//            }

//            else if (Manager == true)
//            {
//                db.Sp_Update_LeaveStatus_Manager(Id, Manger_Remarks, Manger_Status);
//            }
//            return Json(true);
//        }
//        // officials leaves
//        public ActionResult OfficailLeaves()
//        {
//            ViewBag.TotalOfficialsLeaves = db.Sp_Get_OfficialLeaves().Count();
//            var userid = int.Parse(User.Identity.GetUserId());
//            var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//            ViewBag.Userid = EmpId;
//            var res = db.LeaveRequisitions.Where(x => x.OfficalLeave == 1).ToList();
//            return PartialView(res);
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public JsonResult OfficailLeaves(LeaveRequisition Lr)
//        {
//            Lr.CreatedAt = DateTimeOffset.UtcNow.ToLocalTime();
//            Lr.OfficalLeave = 1;
//            Lr.Cancel = false;
//            var res = db.Sp_Add_Leave(Lr.StartDate, Lr.EndDate, Lr.Reason, Lr.EmpId, Lr.NoOfDays,
//              Lr.OfficalLeave,  Lr.ManagerApproval, Lr.AppliedBy, Lr.LeaveType);
//            object[] data = new object[1];
//            data[0] = Lr;
//            return Json(res);
//        }
//        [HttpGet]
//        public JsonResult LeaveRequestOfficials(long? userid)
//        {
//            var officailLeaves = db.Sp_Get_OfficialLeaves().ToList(); // get Official leaves
//            long officialleavecount = 0;
//            foreach (var item in officailLeaves)
//            {
//                officialleavecount += Convert.ToInt64(item.NoOfDays);
//            }
//            //DateTime[] startDate = new DateTime[leavesCount];
//            DateTime[] officialDates = new DateTime[officialleavecount];
//            int index1 = 0;
//            foreach (var item2 in officailLeaves)
//            {
//                var start = item2.StartDate;
//                var end = item2.EndDate;
//                for (DateTime date = start; date <= end; date = date.AddDays(1))
//                {
//                    officialDates[index1] = date.Date;
//                    index1 = index1 + 1;
//                }
//            }
//            var result = new { officialDates };
//            return Json(result, JsonRequestBehavior.AllowGet);
//        }
//        //This Method Call by Employee Registration (HumanResourceController) for Get Employee Sick and Casual Leave
//        public JsonResult EmployeeLeave(long EmpId)
//        {
//            var item = db.Employees.Where(x => x.Status == "Registered" && x.Id == EmpId).FirstOrDefault();
//            DateTime currDt = DateTime.Now;

//            List<Employee_Leaves> e = new List<Employee_Leaves>();

//            var con = db.MIS_Modules_Configurations.Where(x => x.Module == MIS_Modules.HR.ToString()).FirstOrDefault();
//            HRConfiguration config = JsonConvert.DeserializeObject<HRConfiguration>(con.Config_For_Update);
//            var joinmonth = Convert.ToDouble(12 - item.Date_Of_Joining.Value.Month + 1);
//            var sick = Math.Round(Convert.ToDouble((joinmonth / 12) * config.Sick_Leaves_Quota));
//            var casual = Math.Round(Convert.ToDouble((joinmonth / 12) * config.Casual_Leaves_Quota));
//            var st = new DateTime(DateTime.Now.Year, config.FiscalYearStart.Month, config.FiscalYearStart.Day);
//            var ed = new DateTime(DateTime.Now.Year, config.FiscalYearEnd.Month, config.FiscalYearEnd.Day);


//            Employee_Leaves e1 = new Employee_Leaves()
//            {
//                EmpId = item.Id,
//                From = new DateTime(currDt.Year, 1, 1),
//                Granted = (int)sick,
//                LeaveType = "Sick",
//                Remaining = 0,
//                Spent = 0,
//                To = new DateTime(currDt.Year, 12, 31)
//            };
//            e.Add(e1);
//            if ((currDt - item.Date_Of_Joining.Value).TotalDays >= 90)
//            {
//                Employee_Leaves e2 = new Employee_Leaves()
//                {
//                    EmpId = item.Id,
//                    From = new DateTime(currDt.Year, 1, 1),
//                    Granted = (int)casual,
//                    LeaveType = "Casual",
//                    Remaining = 0,
//                    Spent = 0,
//                    To = new DateTime(currDt.Year, 12, 31)
//                };
//                e.Add(e2);
//            }
//            db.Employee_Leaves.AddRange(e);
//            db.SaveChanges();
//            return Json(true);
//        }
//        //This Method Call by Cron Job for Get Employee Sick and Casual and Annual Leaves
//        public void EmployeeLeaves()
//        {
//            var re = db.Employees.Where(x => x.Status == "Registered").OrderByDescending(x=> x.Date_Of_Joining).ToList();
//            DateTime currDt = DateTime.Now;
//            foreach (var item in re)
//            {
//                //var checkCurrentYear = db.Employee_Leaves.Where(x => x.EmpId == item.Id && (x.From == Convert.ToDateTime(currDt.Year - 01 - 01) && x.To == Convert.ToDateTime(currDt.Year - 12 - 13))).FirstOrDefault();
//                //if (checkCurrentYear == null)
//                //{
//                var lastYrAnnual = db.Employee_Leaves.Where(x => x.EmpId == item.Id && x.LeaveType == "Annual" && (x.From.Value.Year == (currDt.Year - 1))).FirstOrDefault();
//                List<Employee_Leaves> e = new List<Employee_Leaves>();
//                if ((currDt - (DateTime)item.Date_Of_Joining).TotalDays >= 365)
//                {
//                    //Add Sick leave
//                    Employee_Leaves e1 = new Employee_Leaves()
//                    {
//                        EmpId = item.Id,
//                        From = new DateTime(currDt.Year, 1, 1),
//                        Granted = 8,
//                        LeaveType = "Sick",
//                        Remaining = 0,
//                        Spent = 0,
//                        To = new DateTime(currDt.Year, 12, 31)
//                    };
//                    e.Add(e1);

//                    //Add Sick leave
//                    Employee_Leaves e2 = new Employee_Leaves()
//                    {
//                        EmpId = item.Id,
//                        From = new DateTime(currDt.Year, 1, 1),
//                        Granted = 10,
//                        LeaveType = "Casual",
//                        Remaining = 0,
//                        Spent = 0,
//                        To = new DateTime(currDt.Year, 12, 31)
//                    };
//                    e.Add(e2);

//                    //Add Annual Leave
                  
//                    db.Employee_Leaves.AddRange(e);
//                    db.SaveChanges();
//                }
//                else
//                {
//                    //Add Sick leave
//                    Employee_Leaves e1 = new Employee_Leaves()
//                    {
//                        EmpId = item.Id,
//                        From = new DateTime(currDt.Year, 1, 1),
//                        Granted = Convert.ToInt32(Math.Round(Math.Ceiling((8.0 / 12.0) * ((12) - (currDt.Month - 1))), MidpointRounding.AwayFromZero)),
//                        LeaveType = "Sick",
//                        Remaining = 0,
//                        Spent = 0,
//                        To = new DateTime(currDt.Year, 12, 31)
//                    };
//                    e.Add(e1);

//                    //Add Casual leave
//                    Employee_Leaves e2 = new Employee_Leaves()
//                    {
//                        EmpId = item.Id,
//                        From = new DateTime(currDt.Year, 1, 1),
//                        Granted = Convert.ToInt32(Math.Round(Math.Ceiling((10.0 / 12.0) * ((12) - (currDt.Month - 1))), MidpointRounding.AwayFromZero)),
//                        LeaveType = "Casual",
//                        Remaining = 0,
//                        Spent = 0,
//                        To = new DateTime(currDt.Year, 12, 31)
//                    };
//                    e.Add(e2);
//                    db.Employee_Leaves.AddRange(e);
//                    db.SaveChanges();
//                }
//                //}
//            }
//        }
//        public void AllEmployeeLeaves()
//        {
//            var re = db.Employees.Where(x => x.Status == "Registered").OrderByDescending(x => x.Date_Of_Joining).ToList();
//            DateTime currDt = DateTime.Now;
//            foreach (var item in re)
//            {
//                List<Employee_Leaves> e = new List<Employee_Leaves>();
//                if (item.Date_Of_Joining.Value.Year < 2022)
//                {
//                    //Add Sick leave
//                    Employee_Leaves e1 = new Employee_Leaves()
//                    {
//                        EmpId = item.Id,
//                        From = new DateTime(currDt.Year, 1, 1),
//                        Granted = 8,
//                        LeaveType = "Sick",
//                        Remaining = 0,
//                        Spent = 0,
//                        To = new DateTime(currDt.Year, 12, 31)
//                    };
//                    e.Add(e1);

//                    //Add Sick leave
//                    Employee_Leaves e2 = new Employee_Leaves()
//                    {
//                        EmpId = item.Id,
//                        From = new DateTime(currDt.Year, 1, 1),
//                        Granted = 10,
//                        LeaveType = "Casual",
//                        Remaining = 0,
//                        Spent = 0,
//                        To = new DateTime(currDt.Year, 12, 31)
//                    };
//                    e.Add(e2);

//                    //Add Annual Leave

//                    db.Employee_Leaves.AddRange(e);
//                    db.SaveChanges();
//                }
//                else
//                {
//                    //Add Sick leave
//                    Employee_Leaves e1 = new Employee_Leaves()
//                    {
//                        EmpId = item.Id,
//                        From = new DateTime(currDt.Year, 1, 1),
//                        Granted = Convert.ToInt32(Math.Round(Math.Ceiling((8.0 / 12.0) * ((12) - (item.Date_Of_Joining.Value.Month - 1))), MidpointRounding.AwayFromZero)),
//                        LeaveType = "Sick",
//                        Remaining = 0,
//                        Spent = 0,
//                        To = new DateTime(currDt.Year, 12, 31)
//                    };
//                    e.Add(e1);

//                    //Add Casual leave
//                    Employee_Leaves e2 = new Employee_Leaves()
//                    {
//                        EmpId = item.Id,
//                        From = new DateTime(currDt.Year, 1, 1),
//                        Granted = Convert.ToInt32(Math.Round(Math.Ceiling((10.0 / 12.0) * ((12) - (item.Date_Of_Joining.Value.Month - 1))), MidpointRounding.AwayFromZero)),
//                        LeaveType = "Casual",
//                        Remaining = 0,
//                        Spent = 0,
//                        To = new DateTime(currDt.Year, 12, 31)
//                    };
//                    e.Add(e2);
//                    db.Employee_Leaves.AddRange(e);
//                    db.SaveChanges();
//                }
//                //}
//            }
//        }
//        public void CheckAnnualLeaves()
//        {
//            var re = db.Employees.Where(x => x.Status == "Registered" ).ToList();
//            DateTime currDt = DateTime.Now;
//            foreach (var item in re)
//            {
//                var lastYrAnnual = db.Employee_Leaves.Where(x => x.EmpId == item.Id && x.LeaveType == "Annual" && (x.From.Value.Year == (currDt.Year - 1))).FirstOrDefault();
//                var annualspent = db.LeaveRequisitions.Where(x => x.EmpId == item.Id && x.LeaveType == "Annual" && x.ManagerApproval == "Approved" && x.StartDate.Year == (currDt.Year - 1)).Sum(x=> x.NoOfDays);
//                List<Employee_Leaves> e = new List<Employee_Leaves>();
//                if ((currDt - (DateTime)item.Date_Of_Joining).TotalDays >= 365)
//                {
//                    //Add Annual Leave
//                    Employee_Leaves e3 = new Employee_Leaves()
//                    {
//                        EmpId = item.Id,
//                        From = new DateTime(currDt.Year, 1, 1),
//                        Granted = 14,
//                        //Granted = Convert.ToInt32(Math.Round(Math.Ceiling((14.0 / 12.0) * ((12) - (currDt.Month - 1))), MidpointRounding.AwayFromZero)),
//                        LeaveType = "Annual",
//                        Remaining = 0,
//                        Spent = 0,
//                        To = new DateTime(currDt.Year, 12, 31)
//                    };
//                    //e3.Granted += (lastYrAnnual == null) ? 0 : lastYrAnnual.Granted-annualspent;
//                    //e3.Granted = (e3.Granted > 28) ? 28 : e3.Granted;
//                    e.Add(e3);
//                    db.Employee_Leaves.AddRange(e);
//                    db.SaveChanges();
//                }
//            }
//        }

//        private void MarkActivity(EmployeeHistory eh)
//        {
//            if (eh is null)
//                return;
//            db.EmployeeHistories.Add(eh);
//            db.SaveChanges();
//        }
//        public void RefreshLeavesCounts(long? empId)
//        {
//            int yr = DateTime.Now.Year;
//            if (empId is null)
//            {
//                var ttlReqs = db.LeaveRequisitions.Where(x => x.StartDate.Year == yr).ToList();
//                foreach (var emp in ttlReqs.GroupBy(x => x.EmpId))
//                {
//                    foreach (var lv in emp.GroupBy(x => x.LeaveType))
//                    {
//                        int hjkha = lv.Sum(x => x.NoOfDays) ?? 0;
//                        var foundRec = db.Employee_Leaves.Where(x => x.EmpId == emp.Key.Value && x.LeaveType == lv.Key && x.From.Value.Year == yr).FirstOrDefault();
//                        if (foundRec != null)
//                        {
//                            foundRec.Spent += hjkha;
//                            db.Employee_Leaves.Attach(foundRec);
//                            db.Entry(foundRec).Property(x => x.Spent).IsModified = true;
//                            db.SaveChanges();
//                        }
//                    }
//                }
//            }
//            else
//            {
//                var leaveReqsFound = db.LeaveRequisitions.Where(x => x.EmpId == empId).ToList();
//                foreach (var lv in leaveReqsFound.GroupBy(x => x.LeaveType))
//                {
//                    int hjkha = lv.Sum(x => x.NoOfDays) ?? 0;
//                    var foundRec = db.Employee_Leaves.Where(x => x.EmpId == empId && x.LeaveType == lv.Key && x.From.Value.Year == yr).FirstOrDefault();
//                    foundRec.Spent += hjkha;
//                    if (foundRec != null)
//                    {
//                        db.Employee_Leaves.Attach(foundRec);
//                        db.Entry(foundRec).Property(x => x.Spent).IsModified = true;
//                        db.SaveChanges();
//                    }
//                }
//            }
//        }
//        public void RefreshEmployeesLeaves(long? empId)
//        {
//            var empData = db.Employees.Where(x => x.Id == empId).FirstOrDefault();
//            if (empData.Status != "Registered")
//            {
//                return;
//            }
//            DateTime currDt = DateTime.Now;
//            var lastYrAnnual = db.Employee_Leaves.Where(x => x.EmpId == empData.Id && x.LeaveType == "Annual" && (x.From.Value.Year == (currDt.Year - 1))).FirstOrDefault();
//            var currYrLvs = db.Employee_Leaves.Where(x => x.EmpId == empData.Id && (x.From.Value.Year == (currDt.Year))).ToList();
//            List<Employee_Leaves> e = new List<Employee_Leaves>();
//            if (!currYrLvs.Any(x => x.LeaveType == "Sick"))
//            {
//                Employee_Leaves e1 = new Employee_Leaves()
//                {
//                    EmpId = empData.Id,
//                    From = new DateTime(2022, 1, 1),
//                    Granted = Convert.ToInt32(Math.Round(Math.Ceiling((8.0 / 12.0) * ((12) - (currDt.Month - 1))), MidpointRounding.AwayFromZero)),
//                    LeaveType = "Sick",
//                    Remaining = 0,
//                    Spent = 0,
//                    To = new DateTime(2022, 12, 31)
//                };
//                e.Add(e1);
//                var sickLvs = db.Employee_Leaves.Where(x => x.LeaveType == "Sick" && x.From.Value.Year == currDt.Year).FirstOrDefault();
//                if (sickLvs == null)
//                {
//                    db.Employee_Leaves.Add(e1);
//                }
//                else
//                {
//                    db.Employee_Leaves.Attach(sickLvs);
//                    db.Employee_Leaves.Remove(sickLvs);
//                    db.Entry(sickLvs).State = System.Data.Entity.EntityState.Deleted;
//                }
//                db.SaveChanges();
//            }
//            if (!currYrLvs.Any(x => x.LeaveType == "Casual") && ((currDt - empData.Date_Of_Joining.Value).TotalDays >= 90))
//            {
//                Employee_Leaves e2 = new Employee_Leaves()
//                {
//                    EmpId = empData.Id,
//                    From = new DateTime(2022, 1, 1),
//                    Granted = Convert.ToInt32(Math.Round(Math.Ceiling((10.0 / 12.0) * ((12) - (currDt.Month - 1))), MidpointRounding.AwayFromZero)),
//                    LeaveType = "Casual",
//                    Remaining = 0,
//                    Spent = 0,
//                    To = new DateTime(2022, 12, 31)
//                };
//                e.Add(e2);
//                var casLvs = db.Employee_Leaves.Where(x => x.LeaveType == "Casual" && x.From.Value.Year == currDt.Year).FirstOrDefault();
//                db.Employee_Leaves.Attach(casLvs);
//                db.Employee_Leaves.Remove(casLvs);
//                db.Entry(casLvs).State = System.Data.Entity.EntityState.Deleted;
//                db.SaveChanges();
//            }
//            var date = new DateTime(2022, 3, 1);
//            if (!currYrLvs.Any(x => x.LeaveType == "Annual") && ((currDt - (DateTime)empData.Date_Of_Joining).TotalDays >= 365))
//            {
//                Employee_Leaves e3 = new Employee_Leaves()
//                {
//                    EmpId = empData.Id,
//                    From = new DateTime(2022, 1, 1),
//                    Granted = Convert.ToInt32(Math.Round(Math.Ceiling((14.0 / 12.0) * ((12) - (currDt.Month - 1))), MidpointRounding.AwayFromZero)),
//                    LeaveType = "Annual",
//                    Remaining = 0,
//                    Spent = 0,
//                    To = new DateTime(2022, 12, 31)
//                };
//                e3.Granted += (lastYrAnnual == null) ? 0 : lastYrAnnual.Remaining;
//                e3.Granted = (e3.Granted > 28) ? 28 : e3.Granted;
//                e.Add(e3);
//                var annLvs = db.Employee_Leaves.Where(x => x.LeaveType == "Annual" && x.From.Value.Year == currDt.Year).FirstOrDefault();
//                db.Employee_Leaves.Attach(annLvs);
//                db.Employee_Leaves.Remove(annLvs);
//                db.Entry(annLvs).State = System.Data.Entity.EntityState.Deleted;
//                db.SaveChanges();
//            }
//            db.Employee_Leaves.AddRange(e);
//            db.SaveChanges();
//            RefreshLeavesCounts(empId);
//        }

//        public void AnnualLeaves()
//        {
//            var emp = db.Employees.Where(x => x.Status == "Registered" && x.Date_Of_Joining.Value.Year <= 2020).OrderBy(x=> x.Date_Of_Joining).ToList();
//            foreach (var item in emp)
//            {
//                var empleav = db.Employee_Leaves.Where(x => x.From.Value.Year == 2022 && x.EmpId == item.Id && x.LeaveType == "Annual").FirstOrDefault();
//                if (empleav == null)
//                {
//                    continue;
//                }
//                empleav.Granted = empleav.Granted + 14;

//                if(empleav.Granted > 28)
//                {
//                    empleav.Granted = 28;
//                }   

//                db.Employee_Leaves.Attach(empleav);
//                db.Entry(empleav).Property(x => x.Granted).IsModified = true;
//                db.SaveChanges();
//            }
//        }


//    }
//}