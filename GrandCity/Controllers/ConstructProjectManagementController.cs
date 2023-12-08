//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Threading;
//using System.Web;
//using System.Web.Mvc;
//using System.Xml.Linq;
//using MeherEstateDevelopers.Filters;
//using MeherEstateDevelopers.Models;
//using Microsoft.AspNet.Identity;
//using Newtonsoft.Json;

//namespace MeherEstateDevelopers.Controllers
//{
//    [Authorize]
//    [LogAction]
//    public class ConstructProjectManagementController : Controller
//    {
//        private const int ThumbSize = 80;

//        // GET: ConstructProjectManagement
//        private Grand_CityEntities db = new Grand_CityEntities();
//        //Creation of Project
//        public ActionResult ProjectsList()
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            ViewBag.Userid = userid;
//            if (User.IsInRole("Administrator"))
//            {
//                var res = db.Projects.ToList();
//                ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Department.ToString()), "Id", "Name");
//                return PartialView(res);
//            }
//            else if (User.IsInRole("All Projects"))
//            {
//                var res = db.Projects.ToList();
//                ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Department.ToString()), "Id", "Name");
//                return PartialView(res);
//            }
//            else
//            {
//                var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//                var prjid = db.Prj_Attendees.Where(x => x.Userid == EmpId).Select(x => x.PrjId).ToList();

//                var res = db.Projects.Where(x => prjid.Contains(x.Id)).ToList();
//                ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Department.ToString()), "Id", "Name");
//                return PartialView(res);
//            }
//        }
//        public ActionResult ProjectInventory()
//        {
//            var userid = User.Identity.GetUserId<long>();
//            if (User.IsInRole("Administrator"))
//            {
//                var Dep = db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Department.ToString()).ToList();
//                string Ids = new XElement("Dep", Dep.Select(x => new XElement("DepId",
//                                     new XAttribute("Id", x.Id)
//                                     ))).ToString();
//                var res = db.Sp_Get_InventoryList_Dep(Ids).ToList();
//                return PartialView(res);
//            }
//            else
//            {

//                var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//                var Dep = db.Employee_Designations.Where(x => x.Emp_Id == EmpId).ToList();
//                string Ids = new XElement("Dep", Dep.Select(x => new XElement("DepId",
//                                     new XAttribute("Id", x.Department_Id)
//                                     ))).ToString();
//                var res = db.Sp_Get_InventoryList_Dep(Ids).ToList();
//                return PartialView(res);
//            }
//        }
//        public ActionResult ProjectDetails(long Prj_Id)
//        {
//            var res = db.Projects.Where(x => x.Id == Prj_Id).FirstOrDefault();
//            if (res.Status == "Pending")
//            {
//                return RedirectToAction("ProjectConfiguration", new { Id = Prj_Id });
//            }
//            else if (res.Status == "Pending_Approval")
//            {
//                return RedirectToAction("ProjectApproval", new { Id = Prj_Id });
//            }
//            else if (res.Status == "Approved")
//            {
//                return RedirectToAction("ProjectDetail", new { Id = Prj_Id });
//            }
//            else
//            {
//                return View();
//            }
//        }
//        public JsonResult DeletePrjAttendee(long Id)
//        {
//            var res = db.Sp_Delete_PrjAttendee(Id);
//            return Json(true);
//        }
//        public ActionResult CreateProject()
//        {
//            return PartialView();
//        }
//        [HttpPost]
//        public JsonResult CreateProject(string ProjectName, long[] Emp,string Department, string PrjType, bool Offsite, int AccountId)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//            var EmpDep = db.Sp_Get_EmployeeFirstDesignation(Emp[0]).FirstOrDefault();
//            string Prjid = ProjectName + "-" + string.Format("{0:ddMMyy}", DateTime.Now);
//            Prjid = Prjid.Replace(" ", "");
//            TextInfo text = new CultureInfo("en-us", false).TextInfo;
//            ProjectName = text.ToTitleCase(ProjectName);
//            var Acc = db.Sp_Get_CA_Head_Parameter(AccountId).FirstOrDefault();
//            long? res = db.Sp_Add_Project(ProjectName, userid, Prjid, PrjType, EmpDep.DepartmentName, Offsite, Acc.Text_ChartCode).FirstOrDefault();
//            var userdata1 = db.Sp_Get_EmployeeFirstDesignation(EmpId).FirstOrDefault();
//            var res8 = db.Sp_Add_Prj_Users(res, userdata1.DepartmentId, userdata1.DepartmentName, EmpId, userdata1.Name);
//            if (res != 0)
//            {
//                foreach (var item in Emp)
//                {
//                    var userdata = db.Sp_Get_EmployeeFirstDesignation(item).FirstOrDefault();
//                    var res1 = db.Sp_Add_Prj_Users(res, userdata.DepartmentId, userdata.DepartmentName, item, userdata.Name);
//                }
//                PostModel post = new PostModel()
//                {
//                    Prj_Id = Convert.ToInt64(res),
//                    Description = "<p><b>General Discussuion</b></p>"
//                };
//                this.AddPost(post, 1);
//            }
//            return Json(res);
//        }
//        public ActionResult AddPrjAttendee()
//        {
//            ViewBag.deps = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Department.ToString()), "Id", "Name");
//            return PartialView();
//        }
//        //public ActionResult DpCopywbdbilling(long prj_Id)
//        //{

//        //    var res1 = db.Vendors.Where(x => x.Name != null).Select(x => new { x.Id, x.Name }).Distinct().ToList();
//        //    ViewBag.Vendors = new SelectList(res1, "Id", "Name");
//        //    var res = db.Sp_Prj_Dcopy_Activity_Billing(prj_Id).ToList() ;
//        //    ViewBag.Total = db.Prj_WBD_Activity_Billing.Where(x => x.Type == "Activity").Select(x => x.Amount).Sum();
//        //    ViewBag.prj_Id = prj_Id;
//        //    return PartialView(res);
//        //}
//        public JsonResult AddAttendee(long[] Emp, long Prj_Id)
//        {
//            foreach (var item in Emp)
//            {
//                var userdata = db.Sp_Get_EmployeeFirstDesignation(item).FirstOrDefault();
//                var res1 = db.Sp_Add_Prj_Users(Prj_Id, userdata.DepartmentId, userdata.DepartmentName, item, userdata.Name);
//            }
//            var ret = new { Status = true, Msg = "Added Succussfully" };
//            return Json(ret);
//        }

//        public JsonResult DeleteProject(long Id)
//        {
//            var res = db.Sp_Delete_Project(Id).FirstOrDefault();
//            return Json(res);
//        }
//        public JsonResult DeleteMTS(long Id)
//        {
//            var res = db.Sp_Delete_MTS(Id).FirstOrDefault();
//            return Json(res);
//        }
//        public ActionResult ProjectConfiguration(long Id)
//        {
//            var res = db.Projects.Where(x => x.Id == Id).FirstOrDefault();

//            if (res == null)
//            {
//                return View();
//            }
//            if (res.Status != "Pending")
//            {
//                return RedirectToAction("ProjectDetails", new { Prj_Id = Id });
//            }
//            return View(res);
//        }
//        public ActionResult ProjectApproval(long Id)
//        {
//            var res = db.Projects.Where(x => x.Id == Id).FirstOrDefault();

//            if (res == null)
//            {
//                return View();
//            }
//            if (res.Status != "Pending_Approval")
//            {
//                return RedirectToAction("ProjectDetails", new { Prj_Id = Id });
//            }
//            return View(res);
//        }
//        public ActionResult ProjectDetail(long Id)
//        {
//            var res = db.Projects.Where(x => x.Id == Id).FirstOrDefault();

//            if (res == null)
//            {
//                return View();
//            }
//            if (res.Status != "Approved")
//            {
//                return RedirectToAction("ProjectDetails", new { Prj_Id = Id });
//            }
//            return View(res);
//        }
//        public ActionResult PrjDepartments(long Prj_Id)
//        {
//            var res = db.Prj_Attendees.Where(x => x.PrjId == Prj_Id).ToList();
//            return PartialView(res);
//        }
//        public ActionResult ProjectFiles(long Prj_Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            ViewBag.ThumbSize = ThumbSize;
//            ViewBag.Prj_Id = Prj_Id;
//            ViewBag.Userid = userid;

//            return PartialView();
//        }
//        public JsonResult UpdateProjectDetails(string PrjAttr, string Value, long Prj_Id)
//        {
//            bool? res = db.Sp_Update_Project(Prj_Id, PrjAttr, Value).FirstOrDefault();
//            return Json(res);
//        }
//        public ActionResult ProjectOverview(long Prj_Id)
//        {
//            return PartialView();
//        }
//        // Gantt Chart Data
//        public ActionResult GanttChart(long Prj_Id)
//        {
//            ViewBag.id = Prj_Id;
//            return PartialView();
//        }
//        public JsonResult getGanttChart(long Prj_Id)
//        {
//            var res = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Prj_Id == Prj_Id).ToList();
//            var json = JsonConvert.SerializeObject(res);
//            return Json(json, JsonRequestBehavior.AllowGet);
//        }
//        public JsonResult getPrjData(long Id)
//        {
//            var h = new { };
//            var res = db.Projects.Where(x => x.Id == Id).SingleOrDefault();
//            var json = JsonConvert.SerializeObject(res);
//            return Json(json, JsonRequestBehavior.AllowGet);
//        }
//        public ActionResult MilestoneSummary(long Prj_Id)
//        {
//            var res = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Prj_Id == Prj_Id && x.Type == "Milestone").ToList();
//            return PartialView(res);
//        }
//        public ActionResult FinanceSummary(long Prj_Id)
//        {
//            var res1 = db.Projects.Where(x => x.Id == Prj_Id).FirstOrDefault().Budget;
//            var res2 = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Type == "Milestone" && x.Prj_Id == Prj_Id).Sum(x => x.Amount);
//            var res3 = db.Prj_Milestone_Task_Subtask_Versions.Where(x => x.Type == "Milestone" && x.Prj_Id == Prj_Id && x.Current_Status == "Approved").Sum(x => x.Amount);
//            var res4 = db.Prj_Voucher_Req.Where(x => x.Status == "Approved" && x.Prj_Id == Prj_Id).Sum(x => x.Amount);
//            var res = new Prj_Financial_Summary { Prj_Budget = res1, Actual_Budget = res2, Budget_Diff = res3, Payment_Requests = res4 };
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Finance Summary  Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Prj_Id);
//            return PartialView(res);
//        }
//        public ActionResult Create_Milestones(long Prj_Id)
//        {
//            var res = db.Prj_Attendees.Where(x => x.PrjId == Prj_Id).Select(x => new { x.Dep_Id, x.Dep_Name }).Distinct().ToList();
//            ViewBag.Department = new SelectList(res, "Dep_Id", "Dep_Name");
//            return PartialView();
//        }
//        public JsonResult Add_MTS(long Prj_Id, string Title, string Description, string Unit, decimal? No ,decimal? L, decimal? H, decimal? B, decimal? Qty, decimal? Rate, decimal Amount, DateTime? Deadline, long? MTS_Id, string MTS, DateTime Start_Date, long? DepId)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            if (MTS == "Milestone")
//            {
//                var Prj = db.Projects.Where(x => x.Id == Prj_Id).FirstOrDefault();
//                // For Project Budget validation

//                if (Prj.Budget < 0 || Prj.Budget is null)
//                {
//                    var ret = new Return { Status = false, Msg = "Project Budget cannot be empty" };
//                    return Json(ret);
//                }
//                var milestone = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Type == "Milestone" && x.Prj_Id == Prj_Id).Sum(x => x.Amount);
//                milestone = (milestone == null) ? 0 : milestone;

//                var Prj_Rem_Bud = Prj.Budget - milestone;
//                if (Prj_Rem_Bud < Amount)
//                {
//                    var ret = new Return { Status = false, Msg = "Milestone:"+ Title +" Budget cannot be greator than Project Budget, Please Revise the Budget" };
//                    return Json(ret);
//                }

//                // For Project Start & End Date Validation

//                //if (Prj.StartDate is null)
//                //{
//                //    var ret = new { Status = false, Msg = "Project Start Date cannot be empty" };
//                //    return Json(ret);
//                //}
//                //if (Prj.Deadline is null)
//                //{
//                //    var ret = new { Status = false, Msg = "Project Deadline cannot be empty" };
//                //    return Json(ret);
//                //}
//                //if (Prj.StartDate > Start_Date)
//                //{
//                //    var ret = new { Status = false, Msg = "Milestone Start Date Cannot be less than Project Start Date" };
//                //    return Json(ret);
//                //}
//                //if (Prj.Deadline < Deadline)
//                //{
//                //    var ret = new { Status = false, Msg = "Milestone Deadline Cannot be greater than Project Deadline Date" };
//                //    return Json(ret);
//                //}
//                //if (Start_Date > Deadline)
//                //{
//                //    var ret = new { Status = false, Msg = "Milestone Deadline Cannot be less than milestone Start Date" };
//                //    return Json(ret);
//                //}

//                var res = db.Sp_Add_MilestoneTaskSubTask(Prj_Id, Title, Description, Unit,No, L, H, B, Qty, Rate, Amount, userid, MTS, Deadline, MTS_Id, Start_Date, DepId).FirstOrDefault();
//                if (res == 0)
//                {
//                    var rete = new Return { Status = false, Msg = "Milestone Already Exists" };
//                    return Json(rete);
//                }
//                else
//                {
//                    var rete = new Return { Status = true, Msg = "Milestone has been Created" };
//                    return Json(rete);
//                }
//            }
//            else if (MTS == "Task")
//            {
//                var Milestone = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Id == MTS_Id).FirstOrDefault();
//                // For Project Budget validation

//                if (Milestone.Amount < 0 || Milestone.Amount is null)
//                {
//                    var ret = new Return { Status = false, Msg = "Milestone Amount cannot be empty or 0" };
//                    return Json(ret);
//                }
//                var Tasks = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Type == "Task" && x.MTS_Id == MTS_Id).Sum(x => x.Amount);
//                Tasks = (Tasks == null) ? 0 : Tasks;

//                var Mile_Rem_Bud = Milestone.Amount - Tasks;
//                if (Mile_Rem_Bud < Amount)
//                {
//                    var ret = new Return { Status = false, Msg = "Task:" + Description  +" under Milestone: "+ Milestone.Title +" Budget cannot be greator than Milestone Budget, Please Revise the Budget" };
//                    return Json(ret);
//                }

//                // For Project Start & End Date Validation

//                if (Milestone.Start_Date is null)
//                {
//                    var ret = new Return { Status = false, Msg = "Milestone Start Date cannot be empty" };
//                    return Json(ret);
//                }
//                if (Milestone.Deadline is null)
//                {
//                    var ret = new Return { Status = false, Msg = "Milestone Deadline cannot be empty" };
//                    return Json(ret);
//                }
//                if (Milestone.Start_Date > Start_Date)
//                {
//                    var ret = new Return { Status = false, Msg = "Task:" + Description + " under Milestone: " + Milestone.Title + " Start Date Cannot be less than Milestone Start Date" };
//                    return Json(ret);
//                }
//                if (Milestone.Deadline < Deadline)
//                {
//                    var ret = new Return { Status = false, Msg = "Task:" + Description + " under Milestone: " + Milestone.Title + " Deadline Cannot be greater than Milestone Deadline Date" };
//                    return Json(ret);
//                }
//                if (Start_Date > Deadline)
//                {
//                    var ret = new Return { Status = false, Msg = "Task:" + Description + " under Milestone: " + Milestone.Title + " Deadline Cannot be less than Task Start Date" };
//                    return Json(ret);
//                }

//                var res = db.Sp_Add_MilestoneTaskSubTask(Prj_Id, Title, Description, Unit,No, L, H, B, Qty, Rate, Amount, userid, MTS, Deadline, MTS_Id, Start_Date, DepId).FirstOrDefault();
//                if (res == 0)
//                {
//                    var rete = new Return { Status = false, Msg = "Task:" + Description + " under Milestone: " + Milestone.Title + " Already Exists" };
//                    return Json(rete);
//                }
//                else
//                {
//                    var rete = new Return { Status = true, Msg = "Task:" + Description + " under Milestone: " + Milestone.Title + " has been Created" };
//                    return Json(rete);
//                }
//            }
//            else if (MTS == "SubTask")
//            {
//                var Task = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Id == MTS_Id).FirstOrDefault();
//                // For Project Budget validation

//                if (Task.Amount < 0 || Task.Amount is null)
//                {
//                    var ret = new Return { Status = false, Msg = "Task Amount cannot be empty or 0" };
//                    return Json(ret);
//                }
//                var SubTasks = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Type == "SubTask" && x.MTS_Id == MTS_Id).Sum(x => x.Amount);
//                SubTasks = (SubTasks == null) ? 0 : SubTasks;

//                var Task_Rem_Bud = Task.Amount - SubTasks;
//                if (Task_Rem_Bud < Amount)
//                {
//                    var ret = new Return { Status = false, Msg = "SubTask Budget cannot be greator than Task Budget, Please Revise the Budget" };
//                    return Json(ret);
//                }

//                // For Task Start & End Date Validation

//                if (Task.Start_Date is null)
//                {
//                    var ret = new Return { Status = false, Msg = "Task Start Date cannot be empty" };
//                    return Json(ret);
//                }
//                if (Task.Deadline is null)
//                {
//                    var ret = new Return { Status = false, Msg = "Task Deadline cannot be empty" };
//                    return Json(ret);
//                }
//                if (Task.Start_Date > Start_Date)
//                {
//                    var ret = new Return { Status = false, Msg = "SubTask Start Date Cannot be less than Task Start Date" };
//                    return Json(ret);
//                }
//                if (Task.Deadline < Deadline)
//                {
//                    var ret = new Return { Status = false, Msg = "SubTask Deadline Cannot be greater than Task Deadline Date" };
//                    return Json(ret);
//                }
//                if (Start_Date > Deadline)
//                {
//                    var ret = new Return { Status = false, Msg = "SubTask Deadline Cannot be less than SubTask Start Date" };
//                    return Json(ret);
//                }

//                var res = db.Sp_Add_MilestoneTaskSubTask(Prj_Id, Title, Description, Unit, No, L, H, B, Qty, Rate, Amount, userid, MTS, Deadline, MTS_Id, Start_Date, DepId).FirstOrDefault();
//                if (res == 0)
//                {
//                    var rete = new Return { Status = false, Msg = "SubTask Already Exists" };
//                    return Json(rete);
//                }
//                else
//                {
//                    var rete = new Return { Status = true, Msg = "SubTask has been Created" };
//                    return Json(rete);
//                }
//            }
//            else
//            {
//                var rete = new Return { Status = false, Msg = "Cannot Submit right now" };
//                return Json(rete);
//            }
//        }

//        public ActionResult Create_WorkBreakDown(long Prj_Id)
//        {
//            var activity = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Type == "Activity").Select(x => new { x.Id, x.Title, x.Activity_Id }).Distinct().ToList();
//            var wbd = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Type == "WBD").Select(x => new { x.Id, x.Title, x.Activity_Id }).Distinct().ToList();
//            ViewBag.Dependency = new SelectList(activity, "Id", "Title");
//            ViewBag.WBDDDown = new SelectList(wbd, "Id", "Title");
//            return PartialView();
//        }
//        public JsonResult Add_WBDResourceMaterial(long Prj_Id, long? Activity_Id, string Type, long? Inventory, decimal? Qty, decimal? Rate, decimal Amount, string UOM)
//        {

//            long userid = long.Parse(User.Identity.GetUserId());
//            var a = db.Sp_Add_WBD_rsc_Material(Prj_Id, Activity_Id, Type, Inventory, Qty, Rate, Amount, UOM, userid);
//            var rete = new Return { Id = Prj_Id, Status = true, Msg = "WBD Resource has been Created" };

//            return Json(rete);
//        }
//        //public JsonResult Add_WBDResourceLabour(long Prj_Id, long? Activity_Id, string Type, decimal? Qty, decimal? Rate, decimal Amount,string Phpd)
//        //{

//        //    long userid = long.Parse(User.Identity.GetUserId());
//        //    var a = db.Sp_Add_WBD_rsc_Labour(Prj_Id, Activity_Id, Type, Qty, Rate, Amount, userid, Phpd);
//        //    var rete = new Return { Id = Prj_Id, Status = true, Msg = "WBD Resource has been Created" };

//        //    return Json(rete);
//        //}
//        public JsonResult Add_WBDResourceEquipment(long Prj_Id, long? Activity_Id, string Type, decimal? Qty, decimal? Rate, decimal Amount, string Phpd, string Mitem)
//        {

//            long userid = long.Parse(User.Identity.GetUserId());
//            var a = db.Sp_Add_WBD_rsc_Equipment(Prj_Id, Activity_Id, Type,  Qty, Rate, Amount,userid, Phpd, Mitem);
//            var rete = new Return { Id = Prj_Id, Status = true, Msg = "WBD Resource has been Created" };

//            return Json(rete);
//        }
//        //public JsonResult UpdateWbdBillingCheck(long[] Wbd_Id, long Vendor_id, long prj_Id)
//        //{
//        //    //List<string> ItemIds1 = new List<string>();
//        //    //List<string> ItemIds2 = new List<string>();
//        //    //   //ItemIds1.Add(Convert.ToString(Id));

//        //    foreach (var item in Wbd_Id)
//        //    {
//        //        var Id = db.Prj_WBD_Activity_Billing.Where(x => x.WBD_Id == item && x.Finalize=="False").Select(x => x.Id).FirstOrDefault();
//        //        var WBD_Id = db.Prj_WBD_Activity_Billing.Where(x => x.Dependencyid == item).Select(x => x.WBD_Id).ToList();

//        //        db.Sp_Update_Billing_Flz_Status(Convert.ToInt64(Id), Vendor_id,prj_Id);

//        //        foreach (var item2 in WBD_Id)
//        //        {
//        //            var Id2 = db.Prj_WBD_Activity_Billing.Where(x => x.WBD_Id == item2 && x.Finalize == "False").Select(x => x.Id).FirstOrDefault();

//        //            var res =db.Sp_Update_Billing_Flz_Status(Convert.ToInt64(Id2), Vendor_id,prj_Id);
                   
                
//        //        }
//        //    }
//        //    var rete = new Return { Status = true};
//        //    return Json(rete);
//        //}
//        //public JsonResult Add_WBDWBD(long Prj_Id, string Title, string Description)
//        //{

//        //    long userid = long.Parse(User.Identity.GetUserId());
//        //    var a = db.Sp_Add_WBD_Activity(Prj_Id, Title, Description, null, null, null, null, userid, null, null, null, null, "WBD", null);
//        //    var rete = new Return { Status = true, Msg = "Work BreakDown Schedule has been Created" };

//        //    return Json(rete);
//        //}
//        //public JsonResult Add_WBD(long Prj_Id, string Title,string Type)
//        //{

//        //    long userid = long.Parse(User.Identity.GetUserId());
//        //    var Prj = db.Projects.Where(x => x.Id == Prj_Id).FirstOrDefault();
//        //    var StartDate = Prj.StartDate;
//        //    var EndDate = Prj.Deadline;
//        //    var a = db.Sp_Add_WBD_Activity(Prj_Id, Title, null, null, null, null, null, userid, null, null, null, null, "WBS", null);
//        //    var rete = new Return { Status = true, Msg = "Work Break Down has been Created" };
//        //    return Json(rete);
//        //}


//        //public JsonResult Add_row_WBD(long Prj_Id, string Title, string Type, long? Wbd_Id)
//        //{

//        //    long userid = long.Parse(User.Identity.GetUserId());
//        //    var Prj = db.Projects.Where(x => x.Id == Prj_Id).FirstOrDefault();
//        //    var StartDate = Prj.StartDate;
//        //    var EndDate = Prj.Deadline;
//        //    var WbdDate = db.Prj_WBD_Activity.Where(x => x.Id == Wbd_Id).FirstOrDefault();
//        //    var wbdstart = WbdDate.Deadline;
//        //    //var wbdEnd = wbdstart.Value.AddDays(NoDays);

//        //    db.Sp_Add_WBD_Activity(Prj_Id, Title, null, null, null, null, null, userid, null, null, null, null, "WBS", Wbd_Id);
//        //    //var SD = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Dependencyid == Wbd_Id && x.Type == "WBS").ToList().Min(x => x.Start_Date);
//        //    //var ED = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Dependencyid == Wbd_Id && x.Type == "WBS").ToList().Max(x => x.Deadline);
//        //    //db.Sp_Update_Prj_WBD_startEnd_Date(SD, ED, Wbd_Id, Prj_Id);


//        //    var rete = new Return { Status = true, Msg = "Work BreakDown Schedule has been Created" };
//        //    return Json(rete);
//        //}

//        public JsonResult SelectActivityList(long Prj_Id, long Wbd_Id)
//        {

//            long userid = long.Parse(User.Identity.GetUserId());
//            var Prj = db.Projects.Where(x => x.Id == Prj_Id).FirstOrDefault();
//            var res = db.Prj_WBD_Activity.Where(x => x.Dependencyid == Wbd_Id && x.Prj_Id==Prj_Id && x.Type=="Activity" ).Select(x => new { x.Id, x.Title }).ToList();
//            return Json(res);
//        }

//        //public JsonResult Add_row_Activity(long Prj_Id, string Title, int NoDays, string Type, long? Wbd_Id, string Relation, long? ActivityId)
//        //{

//        //    long userid = long.Parse(User.Identity.GetUserId());
//        //    var Prj = db.Projects.Where(x => x.Id == Prj_Id).FirstOrDefault();
//        //    var StartDate = Prj.StartDate;
//        //    var EndDate = Prj.Deadline;
//        //    if (Relation == "FS" && ActivityId != null)
//        //    {
//        //        var FSDate = db.Prj_WBD_Activity.Where(x => x.Id == ActivityId).FirstOrDefault();
//        //        var FSStart = FSDate.Deadline;
//        //        var FSEnd = FSStart.Value.AddDays(NoDays);
//        //        db.Sp_Add_WBD_Activity(Prj_Id, Title, Relation, null, null, null, null, userid, FSStart, FSEnd, ActivityId, NoDays, "Activity", Wbd_Id);
//        //        var temp1 = Wbd_Id;
//        //        db.Sp_Update_Prj_WBD_startEnd_Date(StartDate, FSEnd, temp1, Prj_Id);
//        //        while (temp1 != null)
//        //        {
//        //            var Id = db.Prj_WBD_Activity.Where(x => x.Id == temp1).Select(x => x.Dependencyid).FirstOrDefault();
//        //            if (Id == null)
//        //            {
//        //                db.Sp_Update_Prj_WBD_startEnd_Date(StartDate, FSEnd, temp1, Prj_Id);
//        //            }
//        //            temp1 = Id;
//        //        }
//        //    }
//        //    else if (Relation == "SS" && ActivityId != null)
//        //    {
//        //        var SSDate = db.Prj_WBD_Activity.Where(x => x.Id == ActivityId).FirstOrDefault();
//        //        var SSStart = SSDate.Start_Date;
//        //        var SSEnd = SSStart.Value.AddDays(NoDays);
//        //        db.Sp_Add_WBD_Activity(Prj_Id, Title, Relation, null, null, null, null, userid, SSStart, SSEnd, ActivityId, NoDays, "Activity", Wbd_Id);

//        //        var temp2 = Wbd_Id;
//        //        db.Sp_Update_Prj_WBD_startEnd_Date(StartDate, SSEnd, temp2, Prj_Id);
//        //        while (temp2 != null)
//        //        {
//        //            var Id = db.Prj_WBD_Activity.Where(x => x.Id == temp2).Select(x => x.Dependencyid).FirstOrDefault();
//        //            if (Id == null)
//        //            {
//        //                db.Sp_Update_Prj_WBD_startEnd_Date(StartDate, SSEnd, temp2, Prj_Id);
//        //            }
//        //            temp2 = Id;
//        //        }
//        //    }
//        //    else
//        //    {
//        //        var table = db.Prj_WBD_Activity.ToList();
//        //        var End = StartDate.Value.AddDays(NoDays);
//        //        db.Sp_Add_WBD_Activity(Prj_Id, Title, null, null, null, null, null, userid, StartDate, End, null, NoDays, "Activity", Wbd_Id);

//        //        var temp = Wbd_Id;
//        //        db.Sp_Update_Prj_WBD_startEnd_Date(StartDate, End, temp, Prj_Id);
//        //        while (temp != null)
//        //        {
//        //            var Id = db.Prj_WBD_Activity.Where(x => x.Id == temp).Select(x => x.Dependencyid).FirstOrDefault();
//        //            if (Id == null)
//        //            {
//        //                db.Sp_Update_Prj_WBD_startEnd_Date(StartDate, End, temp, Prj_Id);
//        //            }
//        //            temp = Id;
//        //        }
//        //        //var res = new Return { Status = true, Msg = "Work BreakDown Activity has been Created" };
//        //        //return Json(res);
//        //    }

//        //    var Id2 = db.Prj_WBD_Activity.Where(x => x.Id == Wbd_Id).Select(x => x.Dependencyid).FirstOrDefault();
//        //    var ED2 = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Dependencyid == Wbd_Id).ToList().Max(x => x.Deadline);
//        //    db.Sp_Update_Prj_WBD_startEnd_Date(StartDate, ED2, Wbd_Id, Prj_Id);

//        //    var temp6 = Wbd_Id;
//        //    while (temp6 != null)
//        //    {
//        //        var Id3 = db.Prj_WBD_Activity.Where(x => x.Id == temp6).Select(x => x.Dependencyid).FirstOrDefault();
//        //        var Id5 = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Id == temp6).Select(x => x.Deadline).FirstOrDefault();
//        //        if (Id3 == null)
//        //        {
//        //            db.Sp_Update_Prj_WBD_startEnd_Date(StartDate, ED2, temp6, Prj_Id);
//        //            var Id4 = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Dependencyid == temp6 && x.Type == "WBS").ToList();

//        //            List<DateTime> WBSDAte = new List<DateTime>();
//        //            foreach (var item in Id4)
//        //            {
//        //                var SubId = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Dependencyid == item.Id).ToList();
//        //                if (SubId.Any())
//        //                {
//        //                    foreach (var item1 in SubId)
//        //                    {
//        //                        if (item1.Deadline != null)
//        //                        {
//        //                            WBSDAte.Add(Convert.ToDateTime(item.Deadline));
//        //                        }
//        //                    }
//        //                }
//        //                else
//        //                     if (item.Deadline != null)
//        //                {
//        //                    WBSDAte.Add(Convert.ToDateTime(item.Deadline));
//        //                }
//        //            }
//        //            //var MAXWBSDATE = WBSDAte.Take(1).OrderByDescending(x=> x).FirstOrDefault();
//        //            if (WBSDAte.Any())
//        //            {
//        //                var MAXWBSDATE = WBSDAte.Max();
//        //                db.Sp_Update_Prj_WBD_startEnd_Date(StartDate, Convert.ToDateTime(MAXWBSDATE), temp6, Prj_Id);
//        //            }

//        //        //    var MAXWBSDATE = WBSDAte.Max();

//        //        //    db.Sp_Update_Prj_WBD_startEnd_Date(StartDate, Convert.ToDateTime(MAXWBSDATE), temp6, Prj_Id);
//        //        //
//        //        }
//        //        temp6 = Id3;
//        //    }
//        //    var rete = new Return { Status = true, Msg = "Work BreakDown Activity has been Created" };
//        //    return Json(rete);
//        //}



//        //var WbdDate = db.Prj_WBD_Activity.Where(x => x.Id == Wbd_Id).FirstOrDefault();

//        //var wbdstart = WbdDate.Deadline;
//        //var wbdEnd = wbdstart.Value.AddDays(NoDays);

//        //var a = db.Sp_Add_WBD_Activity(Prj_Id, Title, null, null, null, null, null, userid, wbdstart, wbdEnd, null, NoDays, "Activity", Wbd_Id);
//        ////var SD = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Version == Wbd_Id && x.Type == "WBD").ToList().Min(x => x.Start_Date);
//        ////var ED = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Version == Wbd_Id && x.Type == "WBD").ToList().Max(x => x.Deadline);
//        //if (WbdDate.Version == null)
//        //{
//        //    db.Sp_Update_Prj_WBD_startEnd_Date(StartDate, wbdEnd, Wbd_Id);
//        //}
//        //else
//        //{
//        //    db.Sp_Update_Prj_WBD_startEnd_Date(wbdstart, wbdEnd, Wbd_Id);
//        //}
//        //rete = new Return { Status = true, Msg = "Work Break Down has been Created" };


//        //var ActivityWBD = db.Prj_WBD_Activity.Where(x => x.Id == Wbd_Id).FirstOrDefault();
//        //var EndTime = ActivityWBD.Deadline.Value.AddDays(NoDays);
//        //var a = db.Sp_Add_WBD_Activity(Prj_Id, Title, null, null, null, null, null, userid,ActivityWBD.Deadline, EndTime, Activity_Id, NoDays, "Activity", Wbd_Id);
//        //var SD = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Version == Wbd_Id && x.Type == "Activity").ToList().Min(x => x.Start_Date);
//        //var ED = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Version == Wbd_Id && x.Type == "Activity").ToList().Max(x => x.Deadline);
//        //db.Sp_Update_Prj_WBD_startEnd_Date(SD,ED, Wbd_Id);
//        //    return Json(rete);
//        //}


//        public ActionResult WBDBilling(long? proj)
//        {


//            var res = db.Vendors.Where(x =>x.Name != null).Select(x => new { x.Id, x.Name }).Distinct().ToList();
//            ViewBag.Vendors = new SelectList(res, "Id", "Name");

          
//            ViewBag.Proj_Id = proj;

//            return PartialView();

//        }
//        public ActionResult WbdResources(long? proj)
//        {


//            var res = db.Prj_WBD_Activity.Where(x => x.Prj_Id == proj && x.Type == "Activity").Select(x => new { x.Id, x.Title }).Distinct().ToList();
//            ViewBag.ResourceActivity = new SelectList(res, "Id", "Title");

//            var res1 = db.Inventories.Where(x => x.Item_Name != null).Take(50).Select(x => new { x.Id, x.Item_Name }).ToList();
//            ViewBag.wbdresInventory = new SelectList(res1, "Id", "Item_Name");
//            var res2 = db.Assets.Where(x => x.Asset_Name != null).Select(x => new { x.Id, x.Asset_Name }).ToList();
//            ViewBag.wbdresAssets = new SelectList(res2, "Id", "Asset_Name");
//            ViewBag.Proj_Id = proj;

//            return PartialView();
            
//        }
//        public ActionResult WbdResourcesMaterial(long Prj_Id)
//         {

//            var res = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Type == "Activity").Select(x => new { x.Id, x.Title }).Distinct().ToList();
//            ViewBag.ResourceActivity = new SelectList(res, "Id", "Title");

//            var res1 = db.Inventories.Where(x => x.Item_Name != null).Take(50).Select(x => new { x.Id, x.Item_Name }).ToList();
//            ViewBag.wbdresInventory = new SelectList(res1, "Id", "Item_Name");

//            var res2 = db.Assets.Where(x => x.Asset_Name != null).Select(x => new { x.Id, x.Asset_Name }).ToList();
//            ViewBag.wbdresAssets = new SelectList(res2, "Id", "Asset_Name");

//            long userid = long.Parse(User.Identity.GetUserId());
//            var Empid = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//            var result = db.Sp_Get_WbdResources_Material(Prj_Id, WbdResourceType.Material.ToString()).ToList();
//            db.Sp_Add_Activity(userid, "Accessed Pending QarzeHasna Manager Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
//            return PartialView(result);
//        }
//        public ActionResult WbdResourcesLabour(long Prj_Id)
//        {
//            var res = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Type == "Activity").Select(x => new { x.Id, x.Title }).Distinct().ToList();
//            ViewBag.ResourceActivity = new SelectList(res, "Id", "Title");

        
//            var res2 = db.Assets.Where(x => x.Asset_Name != null).Select(x => new { x.Id, x.Asset_Name }).ToList();
//            ViewBag.wbdresAssets = new SelectList(res2, "Id", "Asset_Name");

//            long userid = long.Parse(User.Identity.GetUserId());
//            var Empid = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//            var result = db.Sp_Get_WbdResources_Labour(Prj_Id,WbdResourceType.Labour.ToString()).ToList();
//            db.Sp_Add_Activity(userid, "Accessed Pending QarzeHasna Manager Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
//            return PartialView(result);
//        }
//        public ActionResult WbdResourcesEquipment(long Prj_Id)
//        {
//            var res = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Type == "Activity").Select(x => new { x.Id, x.Title }).Distinct().ToList();
//            ViewBag.ResourceActivity = new SelectList(res, "Id", "Title");

//            var res2 = db.Assets.Where(x => x.Asset_Name != null).Select(x => new { x.Id, x.Asset_Name }).ToList();
//            ViewBag.wbdresAssets = new SelectList(res2, "Id", "Asset_Name");
//            long userid = long.Parse(User.Identity.GetUserId());
//            var Empid = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//            var result = db.Sp_Get_WbdResources_Equipment(Prj_Id, WbdResourceType.Equipment.ToString()).ToList();
//            db.Sp_Add_Activity(userid, "Accessed Pending QarzeHasna Manager Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), userid);
//            return PartialView(result);
//        }
//        public ActionResult Get_MTS(long Prj_Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            ViewBag.Userid = userid;
//            var res = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Prj_Id == Prj_Id).ToList();
//            return PartialView(res);
//        }
//       // public ActionResult Get_WBDMeasurementBillingList(long Prj_Id)
//       //{
//       //     long userid = long.Parse(User.Identity.GetUserId());
//       //     ViewBag.Userid = userid;
//       //     ViewBag.Total = db.Prj_WBD_Activity_Billing.Where(x => x.Type == "Activity").Select(x => x.Amount).Sum();


//       //     var res1 = db.Vendors.Where(x => x.Name != null).Select(x => new { x.Id, x.Name }).Distinct().ToList();
//       //     ViewBag.Vendors = new SelectList(res1, "Id", "Name");
//       //     var res = db.Sp_Get_WbdBillingList(Prj_Id).ToList();
//       //     return PartialView(res);
//       // }
//       // public ActionResult Get_WBDPercentageBillingList(long Prj_Id)
//       //{
//       //     long userid = long.Parse(User.Identity.GetUserId());
//       //     ViewBag.Userid = userid;
//       //     var res = db.Sp_Get_WbdBillingList(Prj_Id).ToList();
//       //     return PartialView(res);
//       // }
        
//        public JsonResult edit_u_wbd_blg(string Value, string Attr, long Id)
//        {
//            var res = db.Sp_Update_Wbd_Biling_Details(Value, Attr, Id).FirstOrDefault();
//            return Json(res);
//        }
//        public JsonResult edit_l_wbd_blg(string Value, string Attr, long Id)
//        {
//            var res = db.Sp_Update_Wbd_Biling_Details(Value, Attr, Id).FirstOrDefault();
//            return Json(res);
//        }
//        public JsonResult edit_w_wbd_blg(string Value, string Attr, long Id)
//        {
//            var res = db.Sp_Update_Wbd_Biling_Details(Value, Attr, Id).FirstOrDefault();
//            return Json(res);
//        }
//        public JsonResult edit_h_wbd_blg(string Value, string Attr, long Id)
//        {
//            var res = db.Sp_Update_Wbd_Biling_Details(Value, Attr, Id).FirstOrDefault();
//            return Json(res);
//        }
//        public JsonResult edit_b_wbd_blg(string Value, string Attr, long Id)
//        {
//            var res = db.Sp_Update_Wbd_Biling_Details(Value, Attr, Id).FirstOrDefault();
//            return Json(res);
//        }
//        public JsonResult edit_a_wbd_blg(string Value, string Attr, long Id)
//        {
//            var res = db.Sp_Update_Wbd_Biling_Details(Value, Attr, Id).FirstOrDefault();
//            return Json(res);
//        }
//        public ActionResult Get_WBD(long Prj_Id)
//       {
//            long userid = long.Parse(User.Identity.GetUserId());
//            ViewBag.Userid = userid;
//            var res = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id).ToList();
//            return PartialView(res);
//        }
//        public ActionResult Get_WBDResource(long Prj_Id)
//       {
//            long userid = long.Parse(User.Identity.GetUserId());
//            ViewBag.Userid = userid;
//            var res = db.Sp_Get_wbd_Resource(Prj_Id).ToList();
//            return PartialView(res);
//        }
//        public ActionResult Prj_MTS(long Prj_Id)
//        {
//            var res = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Prj_Id == Prj_Id).ToList();
//            return PartialView(res);
//        }
//        // Runtime Edit of MTS
//        public JsonResult EditMTS_Title_Desc(string Value, string Attr, long Id)
//        {
//            var res = db.Sp_Update_MTS_Details(Value, Attr, Id).FirstOrDefault();
//            return Json(res);
//        }
//        // Runtime Edit of MTS
//        public JsonResult EditWBD_Title_Desc(string Value, string Attr, long Id)
//        {
//            var res = db.Sp_Update_Wbd_Details(Value, Attr, Id).FirstOrDefault();
//            return Json(res);
//        }

//        // Runtime Edit of MTS
//        //public JsonResult EditWBD_Title_nodays(long Value, string Attr, long Id, long Prj_Id, string Type, long? Wbd_Id)
//        //{
//        //    var res = db.Sp_Update_Wbd_Details(Value.ToString(), Attr, Id).FirstOrDefault();

//        //    long userid = long.Parse(User.Identity.GetUserId());
//        //    var Prj = db.Projects.Where(x => x.Id == Prj_Id).FirstOrDefault();
//        //    var StartDate = Prj.StartDate;
//        //    var EndDate = Prj.Deadline;
//        //    var WbdDependency = db.Prj_WBD_Activity.Where(x => x.Dependencyid == Wbd_Id).Select(x => new { x.Start_Date,x.Deadline,x.Id }).ToList();


//        //    if (Type == "WBD")
//        //    {

//        //        var a = 1;
//        //        foreach (var item in WbdDependency)
//        //        {
//        //            if (a == 1)
//        //            {
//        //                var DeadLine1 = item.Deadline.Value.AddDays(Value);
//        //                db.Sp_Update_Prj_WBD_startEnd_Date(item.Start_Date, DeadLine1, item.Id);
                     
//        //                var SD = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Dependencyid == Wbd_Id && x.Type == "WBD").ToList().Min(x => x.Start_Date);
//        //                var ED = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Dependencyid == Wbd_Id && x.Type == "WBD").ToList().Max(x => x.Deadline);
//        //                db.Sp_Update_Wbd_Nodays_Date(StartDate, ED, Wbd_Id);
         
//        //            }
//        //            else
//        //            {
//        //                var Startdate2 = item.Start_Date.Value.AddDays(Value);
//        //                var DeadLine2 = item.Deadline.Value.AddDays(Value);
//        //                db.Sp_Update_Prj_WBD_startEnd_Date(Startdate2, DeadLine2, item.Id);
              

//        //                var SD = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Dependencyid == Wbd_Id && x.Type == "WBD").ToList().Min(x => x.Start_Date);
//        //                var ED = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Dependencyid == Wbd_Id && x.Type == "WBD").ToList().Max(x => x.Deadline);
//        //                db.Sp_Update_Wbd_Nodays_Date(StartDate, ED, Wbd_Id);
                 
//        //            }
//        //            a++;
//        //        }
//        //    }
//        //    else
//        //    {
//        //        var a = 1;
//        //        foreach (var item in WbdDependency)
//        //        {
//        //            if (a == 1)
//        //            {
//        //                var DeadLine1 = item.Deadline.Value.AddDays(Value);
//        //                db.Sp_Update_Prj_WBD_startEnd_Date(item.Start_Date, DeadLine1, item.Id);
             
//        //                var SD = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Dependencyid == Wbd_Id && x.Type == "Activity").ToList().Min(x => x.Start_Date);
//        //                var ED = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Dependencyid == Wbd_Id && x.Type == "Activity").ToList().Max(x => x.Deadline);
//        //                db.Sp_Update_Wbd_Nodays_Date(item.Start_Date, ED, Wbd_Id);
                   
//        //            }
//        //            else
//        //            {
//        //                var Startdate2 = item.Start_Date.Value.AddDays(Value);
//        //                var DeadLine2 = item.Deadline.Value.AddDays(Value);
//        //                db.Sp_Update_Prj_WBD_startEnd_Date(Startdate2, DeadLine2, item.Id);
                    

//        //                var SD = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Dependencyid == Wbd_Id && x.Type == "Activity").ToList().Min(x => x.Start_Date);
//        //                var ED = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Dependencyid == Wbd_Id && x.Type == "Activity").ToList().Max(x => x.Deadline);
//        //                db.Sp_Update_Wbd_Nodays_Date(SD, ED, Wbd_Id);
                  
//        //            }
//        //            a++;
//        //        }
//        //    }
//        //    return Json(res);





//            //if (Wbd_Id != null)
//            //{
//            //    var WbdDate = db.Prj_WBD_Activity.Where(x => x.Id == Wbd_Id).FirstOrDefault();

//            //    var wbdstart = WbdDate.Deadline;
//            //    var wbdEnd = wbdstart.Value.AddDays(Value);

//            //    db.Sp_Update_Wbd_Nodays_Date(StartDate, wbdEnd, Prj_Id);
//            //    //var SD = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Version == Wbd_Id && x.Type == "WBD").ToList().Min(x => x.Start_Date);
//            //    //var ED = db.Prj_WBD_Activity.Where(x => x.Prj_Id == Prj_Id && x.Version == Wbd_Id && x.Type == "WBD").ToList().Max(x => x.Deadline);
//            //    if (WbdDate.Version == null)
//            //    {
//            //        db.Sp_Update_Prj_WBD_startEnd_Date(StartDate, wbdEnd, Wbd_Id);
//            //    }
//            //    else
//            //    {
//            //        db.Sp_Update_Prj_WBD_startEnd_Date(wbdstart, wbdEnd, Wbd_Id);
//            //    }
//            //}





//        //}
//        public ActionResult Milestones()
//        {
//            return PartialView();
//        }
//        public JsonResult Update_MTS(string Value, string Type, long Id)
//        {
//            var res = db.Sp_Update_MTS(Id, Value, Type);
//            return Json(true);
//        }
//        public JsonResult Update_ProjectStatus(long Id, string Status)
//        {
//            var res = db.Sp_Update_ProjectStatus(Id, Status);
//            return Json(true);
//        }
//        public ActionResult MTS_Details(long Prj_Id)
//        {
//            var res1 = long.Parse(User.Identity.GetUserId());
//            var res = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Prj_Id == Prj_Id).ToList();
//            return PartialView(res);
//        }
//        public ActionResult Upload()
//        {
//            return PartialView();
//        }
//        // Project Files

//        // Discussion Thread
//        public ActionResult DisscussionThread()
//        {
//            return PartialView();
//        }
//        // Costing
//        public ActionResult Costing(long Prj_Id)
//        {
//            var res = db.Sp_Get_Prj_Costing(Prj_Id).ToList();
//            return PartialView(res);
//        }
//        public ActionResult Creating()
//        {
//            return PartialView();
//        }
//        // Finance
//        public ActionResult Finances(long Prj_Id)
//        {
//            ViewBag.Prj_Id = Prj_Id;
//            return PartialView();
//        }
//        public ActionResult RequestNewVoucher(long Prj_Id)
//        {
//            var userid = long.Parse(User.Identity.GetUserId());
//            var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//            var Depid = db.Employee_Designations.Where(x => x.Emp_Id == EmpId).Select(x => x.Department_Id).ToList();

//            List<long?> Dep = Depid.ConvertAll(i => (long?)i);
//            ViewBag.Milestones = new SelectList(db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Prj_Id == Prj_Id && x.Type == "Milestone" && Dep.Contains(x.Dep_Id)).ToList(), "Id", "Title");
//            Helpers h = new Helpers();
//            ViewBag.Groupid = h.RandomNumber();
//            return PartialView();
//        }
//        public ActionResult VoucherList(long Prj_Id, string Status)
//        {
//            var res = db.Sp_Get_Prj_VouchersList(Status, Prj_Id).ToList();
//            ViewBag.Status = Status;
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Vouchers List  Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), Prj_Id);
//            return PartialView(res);
//        }
//        public JsonResult RequestPaymentVoucher(List<Payment_Voucher_Req> VoucherDetails, long Groupid)
//        {
//            string chids = null;
//            long uid = User.Identity.GetUserId<long>();

//            if (VoucherDetails.Any())
//            {
//                chids = new XElement("Voucher", VoucherDetails.Select(x => new XElement("Items",
//                                (x.Description == null) ? null : new XAttribute("Description", x.Description),
//                                (x.Milestone == null) ? null : new XAttribute("Milestone", x.Milestone),
//                                 new XAttribute("Mile_Id", x.Mile_Id),
//                                 new XAttribute("Prj_Id", x.Prj_Id),
//                                (x.Quantity == null) ? null : new XAttribute("Quantity", x.Quantity),
//                                (x.Rate == null) ? null : new XAttribute("Rate", x.Rate),
//                                (x.Total == null) ? null : new XAttribute("Total", x.Total),
//                                (x.Unit == null) ? null : new XAttribute("Unit", x.Unit),
//                                 new XAttribute("Groupid", Groupid)
//                                ))).ToString();
//                var res = db.Sp_Add_Prj_PaymentVoucherReq(chids, Groupid).FirstOrDefault();
//                long userid = long.Parse(User.Identity.GetUserId());
//                db.Sp_Add_Activity(userid, "Requested Payment Voucher", "Create", "Activity_Record", ActivityType.Voucher_Request.ToString(), Groupid);
//                var pmtschd = db.SP_Get_PaymentSchedule_ByMile(chids).ToList();

//                foreach (var v in VoucherDetails)
//                {
//                    db.PaymentScheduleDetails.Add(new PaymentScheduleDetail
//                    {
//                        CreatedAt = DateTime.Now,
//                        CreatedBy = uid,
//                        PaymentScheduleId = pmtschd.Where(x => x.MileId == v.Mile_Id).Select(x => x.Id).FirstOrDefault(),
//                        Remarks = v.Description,
//                        WorkDoneAmt = (double)v.Total,
//                        WorkDonePerc = ((double)v.Total / (double)pmtschd.Where(x => x.MileId == v.Mile_Id).Sum(x => x.CompletionAmount)) * (double)(100.0)
//                    });
//                    db.SaveChanges();
//                }

//                return Json(true);
//            }
//            else
//            {
//                return Json(false);
//            }

//        }
//        public ActionResult PaymentVoucherDetails(long GroupId)
//        {
//            var res = db.Prj_Voucher_Req.Where(x => x.Group_Id == GroupId).ToList();
//            long userid = long.Parse(User.Identity.GetUserId());
//            db.Sp_Add_Activity(userid, "Accessed Payment Voucher Details  Page ", "Read", "Activity_Record", ActivityType.Details_Access.ToString(), GroupId);
//            return PartialView(res);
//        }
//        public JsonResult Update_Payment_Status(long GroupId, string Status)
//        {
//            var res = db.Sp_Update_Prj_Payment_Voucher_Req(GroupId, Status);
//            return Json(true);
//        }
//        // Update MTS statu
//        public JsonResult UpdateMTS_Status(long Mts_Id, bool Status)
//        {
//            string Stat = (Status) ? "Completed" : "Approved";
//            var res = db.Sp_Update_MTS_Status(Mts_Id, Stat);
//            return Json(true);
//        }
//        // Versioning of MTS
//        public ActionResult MTS_Update_Details(long Id)
//        {
//            var res = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Id == Id).FirstOrDefault();
//            ViewBag.Department = new SelectList(db.Prj_Departments.Where(x => x.Prj_Id == res.Prj_Id).ToList(), "Dep_Id", "Department_Name", res.Dep_Id);
//            return PartialView(res);
//        }
//        public JsonResult UpdateMTS(decimal? Rate, decimal? Qty, decimal Amount, long Id)
//        {
//            var res1 = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Id == Id).FirstOrDefault();
//            if (res1.Type == "Milestone")
//            {
//                var Prj = db.Projects.Where(x => x.Id == res1.Prj_Id).FirstOrDefault();
//                var milestone = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Type == "Milestone" && x.Prj_Id == res1.Prj_Id && x.Id != res1.Id).Sum(x => x.Amount);
//                var Tasks = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Type == "Task" && x.MTS_Id == Id).Sum(x => x.Amount);
//                milestone = (milestone == null) ? 0 : milestone;

//                var Prj_Rem_Bud = Prj.Budget - milestone;
//                if (Prj_Rem_Bud < Amount)
//                {
//                    var ret = new Return { Status = false, Msg = "Milestone Budget cannot be greator than Project Budget, Please Revise the Budget" };
//                    return Json(ret);
//                }
//                if (Amount < Tasks)
//                {
//                    var ret = new Return { Status = false, Msg = "Task Budget cannot be les than Subtask Budget, Please Revise the Subtask Budget first" };
//                    return Json(ret);
//                }
//                if (Rate != null && Rate != 0)
//                {
//                    var res2 = db.Sp_Update_MTS_Details(Rate.ToString(), "rate", Id).FirstOrDefault();
//                }
//                if (Qty != null && Qty != 0)
//                {
//                    var res3 = db.Sp_Update_MTS_Details(Qty.ToString(), "qty", Id).FirstOrDefault();
//                }
//                var res4 = db.Sp_Update_MTS_Details(Amount.ToString(), "amount", Id).FirstOrDefault();

//            }
//            else if (res1.Type == "Task")
//            {
//                var Milestone = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Id == res1.MTS_Id).FirstOrDefault();

//                var Tasks = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Type == "Task" && x.MTS_Id == res1.MTS_Id && x.Id != res1.Id).Sum(x => x.Amount);
//                var SubTasks = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Type == "SubTask" && x.MTS_Id == Id).Sum(x => x.Amount);
//                Tasks = (Tasks == null) ? 0 : Tasks;

//                var Mile_Rem_Bud = Milestone.Amount - Tasks;
//                if (Mile_Rem_Bud < Amount)
//                {
//                    var ret = new Return { Status = false, Msg = "Task Budget cannot be greator than Milestone Budget, Please Revise the Budget" };
//                    return Json(ret);
//                }
//                if (Amount < SubTasks)
//                {
//                    var ret = new Return { Status = false, Msg = "Task Budget cannot be les than Subtask Budget, Please Revise the Subtask Budget first" };
//                    return Json(ret);
//                }
//                if (Rate != null && Rate != 0)
//                {
//                    var res2 = db.Sp_Update_MTS_Details(Rate.ToString(), "rate", Id).FirstOrDefault();
//                }
//                if (Qty != null && Qty != 0)
//                {
//                    var res3 = db.Sp_Update_MTS_Details(Qty.ToString(), "qty", Id).FirstOrDefault();
//                }
//                var res4 = db.Sp_Update_MTS_Details(Amount.ToString(), "amount", Id).FirstOrDefault();

//            }
//            else if (res1.Type == "SubTask")
//            {
//                var Task = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Id == res1.MTS_Id).FirstOrDefault();

//                var SubTasks = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Type == "SubTask" && x.MTS_Id == res1.MTS_Id && x.Id != res1.Id).Sum(x => x.Amount);
//                SubTasks = (SubTasks == null) ? 0 : SubTasks;

//                var Task_Rem_Bud = Task.Amount - SubTasks;
//                if (Task_Rem_Bud < Amount)
//                {
//                    var ret = new Return { Status = false, Msg = "SubTask Budget cannot be greator than Task Budget, Please Revise the Budget" };
//                    return Json(ret);
//                }
//                if (Rate != null && Rate != 0)
//                {
//                    var res2 = db.Sp_Update_MTS_Details(Rate.ToString(), "rate", Id).FirstOrDefault();
//                }
//                if (Qty != null && Qty != 0)
//                {
//                    var res3 = db.Sp_Update_MTS_Details(Qty.ToString(), "qty", Id).FirstOrDefault();
//                }
//                var res4 = db.Sp_Update_MTS_Details(Amount.ToString(), "amount", Id).FirstOrDefault();

//            }
//            var rett = new Return { Status = true, Msg = "Updated" };
//            return Json(rett);
//        }
//        public JsonResult Update_Start_End(DateTime Date, string S_D, long Id)
//        {
//            var res1 = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Id == Id).FirstOrDefault();
//            if (res1.Type == "Milestone")
//            {
//                var Prj = db.Projects.Where(x => x.Id == res1.Prj_Id).FirstOrDefault();
//                var Tasks = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Type == "Task" && x.MTS_Id == Id).ToList();
//                if (S_D == "Start")
//                {
//                    //if (Prj.StartDate > Date)
//                    //{
//                    //    var ret = new { Status = false, Msg = "Milestone Date cannot be less than Project Date" };
//                    //    return Json(ret);
//                    //}
//                    //else
//                    //{
//                    var res4 = db.Sp_Update_MTS_Details(Date.ToString(), "start", Id).FirstOrDefault();
//                    //}
//                }
//                else if (S_D == "Deadline")
//                {
//                    //if (Prj.Deadline < Date)
//                    //{
//                    //    var ret = new { Status = false, Msg = "Milestone Date cannot be less than Project Date" };
//                    //    return Json(ret);
//                    //}
//                    //else
//                    //{
//                    var res4 = db.Sp_Update_MTS_Details(Date.ToString(), "dead", Id).FirstOrDefault();
//                    //}
//                }
//            }
//            else if (res1.Type == "Task")
//            {
//                var Milestone = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Id == res1.MTS_Id).FirstOrDefault();

//                var SubTasks = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Type == "SubTask" && x.MTS_Id == Id).ToList();

//                if (S_D == "Start")
//                {
//                    if (Milestone.Start_Date > Date)
//                    {
//                        var ret = new { Status = false, Msg = "Task Date cannot be less than Milestone Date" };
//                        return Json(ret);
//                    }
//                    else
//                    {
//                        var res4 = db.Sp_Update_MTS_Details(Date.ToString(), "start", Id).FirstOrDefault();
//                    }
//                }
//                else if (S_D == "Deadline")
//                {
//                    if (Milestone.Deadline < Date)
//                    {
//                        var ret = new { Status = false, Msg = "Task Date cannot be less than Milestone Date" };
//                        return Json(ret);
//                    }
//                    else
//                    {
//                        var res4 = db.Sp_Update_MTS_Details(Date.ToString(), "dead", Id).FirstOrDefault();
//                    }
//                }
//            }
//            else if (res1.Type == "SubTask")
//            {
//                var Task = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Id == res1.MTS_Id).FirstOrDefault();
//                if (S_D == "Start")
//                {
//                    if (Task.Start_Date > Date)
//                    {
//                        var ret = new { Status = false, Msg = "SubTask Date cannot be less than Task Date" };
//                        return Json(ret);
//                    }
//                    else
//                    {
//                        var res4 = db.Sp_Update_MTS_Details(Date.ToString(), "start", Id).FirstOrDefault();

//                    }
//                }
//                else if (S_D == "Deadline")
//                {
//                    if (Task.Deadline < Date)
//                    {
//                        var ret = new { Status = false, Msg = "SubTask Date cannot be less than Task Date" };
//                        return Json(ret);
//                    }
//                    else
//                    {
//                        var res4 = db.Sp_Update_MTS_Details(Date.ToString(), "dead", Id).FirstOrDefault();
//                    }
//                }

//            }
//            var rett = new { Status = true, Msg = "Updated" };
//            return Json(rett);
//        }
//        public JsonResult Update_MTS_Request(long Prj_Id, string Title, string Description, string Unit, decimal? No, decimal? L, decimal? H, decimal? B, decimal? Qty, decimal? Rate, decimal Amount, DateTime? Deadline, long? MTS_Id, string MTS, DateTime Start_Date, long? DepId)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var res = db.Sp_Add_MilestoneTaskSubTask_Req(Prj_Id, Title, Description, Unit, No, L, H, B, Qty, Rate, Amount, userid, MTS, Deadline, MTS_Id, Start_Date, DepId).FirstOrDefault();
//            return Json(res);
//        }
//        public ActionResult MTS_UpdateReqs(long Prj_Id)
//        {
//            var res = db.Sp_Get_MTS_Req_Count(Prj_Id).ToList();
//            return PartialView(res);
//        }
//        public ActionResult MTS_Reqs_Status(long Prj_Id, string Type, string Status)
//        {
//            var res = (from x in db.Prj_Milestone_Task_Subtask_Versions
//                       join y in db.Prj_Milestone_Tasks_SubTasks on x.MTS_Id equals y.Id
//                       where x.Prj_Id == Prj_Id && x.Type == Type && x.Current_Status == Status
//                       select new MTS_MTSV
//                       {
//                           Id = x.Id,
//                           Amount_R = x.Amount,
//                           Amount = y.Amount,
//                           Deadline_R = x.Deadline,
//                           Deadline = y.Deadline,
//                           Dep_Id_R = x.Dep_Id,
//                           Dep_Id = y.Dep_Id,
//                           Description_R = x.Description,
//                           Description = y.Description,
//                           Qty_R = x.Qty,
//                           Qty = y.Qty,
//                           Rate_R = x.Rate,
//                           Rate = y.Rate,
//                           Start_Date_R = x.Start_Date,
//                           Start_Date = y.Start_Date,
//                           Task_CreatedBy = x.Task_CreatedBy,
//                           Task_CreatedBy_Name = x.Task_CreatedBy_Name,
//                           Current_Status = x.Current_Status,
//                           Title_R = x.Title,
//                           Title = y.Title,
//                           Type_R = x.Type,
//                           Type = y.Type,
//                           Unit_R = x.Unit,
//                           Unit = y.Unit
//                       }).ToList();
//            return PartialView(res);
//        }
//        public JsonResult Update_MTS_Status_Req(long Req_Id, string Status)
//        {

//            var res1 = db.Prj_Milestone_Task_Subtask_Versions.Where(x => x.Id == Req_Id).FirstOrDefault();
//            if (Status == "Approved")
//            {
//                var res2 = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Id == res1.MTS_Id).FirstOrDefault();

//                db.Sp_Update_MTS_Req(res2.Id, res2.Title, res2.Description, res2.Unit, res2.Qty, res2.Rate, res2.Amount, res2.Deadline, res2.Start_Date, res2.Version, res2.Dep_Id, res2.L,res2.H,res2.B,res2.No,
//                                     res1.Id, res1.Title, res1.Description, res1.Unit, res1.Qty, res1.Rate, res1.Amount, res1.Deadline, res1.Start_Date, res1.Version, res1.Dep_Id, res1.L, res1.H, res1.B,res1.No);

//                db.Sp_Update_MTS_Version_Status(res1.Id, "Approved");
//            }
//            else if (Status == "Reject")
//            {
//                db.Sp_Update_MTS_Version_Status(res1.Id, "Rejected");
//            }
//            return Json(true);
//        }
//        public ActionResult DiscussionIndex(long Prj_Id)
//        {
//            var res1 = db.Prj_Departments.Where(x => x.Prj_Id == Prj_Id).ToList();
//            List<Sp_Get_DepMISUsers_Result> users = new List<Sp_Get_DepMISUsers_Result>();
//            foreach (var item in res1)
//            {
//                var res2 = db.Sp_Get_DepMISUsers(Convert.ToInt16(item.Dep_Id)).ToList();
//                users.AddRange(res2);
//            }
//            var res3 = users.Select(x => new { tag = x.Name, value = x.Id }).ToList();
//            ViewBag.AllUsers = res3;
//            ViewBag.ThumbSize = ThumbSize;
//            ViewBag.Prj_Id = Prj_Id;
//            Helpers R = new Helpers();
//            ViewBag.Token = R.RandomNumber();
//            return PartialView();
//        }
//        [HttpPost]
//        public JsonResult AddPost(PostModel Post, long Token)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            if (Post.Description == "" || Post.Description == null)
//            {
//                return Json(false);
//            }

//            var res1 = db.Sp_Add_Post(Post.Description, userid, Post.Prj_Id).FirstOrDefault();
//            if (res1 == 0)
//            {
//                return Json(0);
//            }
//            if (Post.Id != null)
//            {
//                if (Post.Id.Count() > 0)
//                {
//                    foreach (var item in Post.Id)
//                    {
//                        var res2 = db.Sp_Add_Post_Attendees(item, res1);
//                    }
//                }
//            }
//            else
//            {
//                var res3 = db.Prj_Departments.Where(x => x.Prj_Id == Post.Prj_Id).ToList();
//                List<Sp_Get_DepMISUsers_Result> users = new List<Sp_Get_DepMISUsers_Result>();
//                foreach (var item in res3)
//                {
//                    var res2 = db.Sp_Get_DepMISUsers(Convert.ToInt16(item.Dep_Id)).ToList();
//                    users.AddRange(res2);
//                }
//                foreach (var item in users)
//                {
//                    var res2 = db.Sp_Add_Post_Attendees(item.Id, res1);
//                }
//            }

//            //// Files To Move
//            //string OldPath = Modules.ProjectManagement.ToString() + "/" + Post.Prj_Id + "/Discussions/" + Token;
//            //string NewPath = Modules.ProjectManagement.ToString() + "/" + Post.Prj_Id + "/Discussions/" + res1;
//            //FileHandlingController FH = new FileHandlingController();

//            //var folder = FH.GetFolder(OldPath);

//            //foreach (var item in folder.GetFiles())
//            //{
//            //    FH.MoveFile(OldPath + "/" + item.Name, NewPath + "/" + item.Name);
//            //}
//            return Json(true);

//        }
//        public ActionResult GetPosts(long Prj_Id)
//        {
//            ViewBag.Prj_Id = Prj_Id;
//            ViewBag.UserId = long.Parse(User.Identity.GetUserId());
//            var res = db.Discussion_Post.Where(x => x.Project_Id == Prj_Id).ToList();
//            return PartialView(res);
//        }
//        public ActionResult GetPostsComments(long Post_Id)
//        {
//            ViewBag.Post_Id = Post_Id;
//            var res = db.Discussion_Post_Comments.Where(x => x.Post_Id == Post_Id).ToList();
//            return PartialView(res);
//        }
//        // Discussions Comments File Upload
//        [HttpGet]
//        public ActionResult DiscussionUpload(long Prj_Id, long Token, IEnumerable<string> names = null)
//        {
//            string FolderPath = Modules.ProjectManagement.ToString() + "/" + Prj_Id + "/Discussions/" + Token;

//            FileHandlingController FH = new FileHandlingController();

//            var files = FH.Files(FolderPath);
//            return Json(new
//            {
//                files
//            }, JsonRequestBehavior.AllowGet);
//        }
//        [HttpPost]
//        public ActionResult DiscussionUpload(List<string> Names, long Prj_Id, long Token)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var files = Request.Files
//                .Cast<string>()
//                .Select(k => Request.Files[k])
//                .ToArray();
//            var names = new List<string>();
//            string FolderPath = Modules.ProjectManagement.ToString() + "/" + Prj_Id + "/Discussions/" + Token;
//            FileHandlingController FH = new FileHandlingController();
//            foreach (var file in files)
//            {
//                var name = FH.SaveFile(file, FolderPath);
//                names.Add(name);
//            }
//            IEnumerable<string> jsonObject = FH.FileNames(FolderPath, names);
//            return DiscussionUpload(Prj_Id, Token, jsonObject);
//        }
//        [HttpGet]
//        public ActionResult PostFiles(long Prj_Id, long Post_Id, IEnumerable<string> names = null)
//        {
//            string FolderPath = Modules.ProjectManagement.ToString() + "/" + Prj_Id + "/Discussions/" + Post_Id;
//            FileHandlingController FH = new FileHandlingController();
//            var files = FH.Files(FolderPath);
//            return Json(new
//            {
//                files
//            }, JsonRequestBehavior.AllowGet);
//        }
//        public ActionResult NewMaterialStatement(long? proj)
//        {
//            var miles = db.SP_Get_Prj_Milestone_Tasks_Subtasks_IDName_Modulewise("Milestone", proj).ToList();
//            ViewBag.Milestones = new SelectList(miles, "Id", "Title");
//            var res = db.SP_Get_MaterialStatement(proj).ToList();
//            return PartialView(res);
//        }
        
//        public JsonResult DeleteMaterialStatument(long Id)
//        {
//            db.Sp_Delete_MaterialStatment(Id);
//            return Json(true);
//        }
//        [HttpPost]
//        public JsonResult AddComment(string Comment, long Post_Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            if (Comment == "" || Comment == null)
//            {
//                return Json(false);
//            }
//            var res = db.Sp_Add_Post_Comment(Comment, userid, Post_Id).FirstOrDefault();
//            return Json(true);
//        }
//        [HttpPost]
//        public JsonResult DeletePost(long Post_Id)
//        {
//            var res = db.Sp_Delete_Post(Post_Id);
//            return Json(true);
//        }
//        public ActionResult EditPost(long Post_Id)
//        {
//            var res = db.Discussion_Post.Where(x => x.Id == Post_Id).FirstOrDefault();
//            return PartialView(res);
//        }
//        public JsonResult UpdatePost(long Post_Id, string Post)
//        {
//            return Json(true);
//        }
//        //[HttpPost]
//        //public JsonResult PrivatePost(long Id)
//        //{
//        //    return Json(true);
//        //}

//        // Project Files
//        [HttpGet]
//        public ActionResult Upload(long Prj_Id, IEnumerable<string> names = null)
//        {
//            string FolderPath = Modules.ProjectManagement.ToString() + "/" + Prj_Id + "/ProjectFiles";
//            FileHandlingController FH = new FileHandlingController();

//            var TotalFolders = FH.GetFolder(FolderPath).GetDirectories().Count();
//            List<FileActions> files = new List<FileActions>();
//            for (int i = 1; i <= TotalFolders; i++)
//            {
//                files.AddRange(FH.Files(FolderPath + "/" + i));
//            }
//            return Json(new
//            {
//                files
//            }, JsonRequestBehavior.AllowGet);
//        }
//        [HttpPost]
//        public ActionResult Upload(List<string> Names, long Prj_Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var files = Request.Files
//                .Cast<string>()
//                .Select(k => Request.Files[k])
//                .ToArray();
//            var names = new List<string>();

//            string FolderPath = Modules.ProjectManagement.ToString() + "/" + Prj_Id + "/ProjectFiles";

//            FileHandlingController FH = new FileHandlingController();
//            var NewFolder = FH.GetFolder(FolderPath).GetDirectories().Count() + 1;

//            foreach (var file in files)
//            {
//                var name = FH.SaveFile(file, FolderPath + "/" + NewFolder);
//                names.Add(name);
//            }
//            IEnumerable<string> jsonObject = FH.FileNames(FolderPath + "/" + NewFolder, names);
//            return Upload(Prj_Id, jsonObject);
//        }
//        public JsonResult CreateMaterialStatement(MaterialStatementData[] materials, long? MileId, long? ProjId)
//        {
//            var uid = long.Parse(User.Identity.GetUserId());
//            var skls = new XElement("MaterialStatement", materials.Select(x => new XElement("MatData",
//                new XAttribute("MileId", MileId),
//                new XAttribute("ProjId", ProjId),
//                new XAttribute("ItemName", x.item),
//                new XAttribute("ItemQty", x.qty),
//                new XAttribute("QtyType", x.qtytp),
//                new XAttribute("Creator", uid),
//                new XAttribute("Rates", x.Rate),
//                (x.len is null) ? null : new XAttribute("leng", x.len),
//                (x.wid is null) ? null : new XAttribute("widt", x.wid),
//                (x.hei is null) ? null : new XAttribute("heig", x.hei),
//                (x.dia is null) ? null : new XAttribute("diam", x.dia),
//                (x.remarks is null) ? null : new XAttribute("rema", x.remarks),
//                (x.description is null) ? null : new XAttribute("descr", x.description)))).ToString();
//            db.SP_Add_MaterialStatement(skls);
//            return Json(true);
//        }
//        public ActionResult PaymentSchedule(long? proj)
//        {
//            var miles = db.SP_Get_Prj_Milestone_Tasks_Subtasks_IDName_Modulewise("Milestone", proj).ToList();
//            ViewBag.Milestones = new SelectList(miles, "Id", "Title");
//            var res = db.Project_Payment_Schedule.Where(x => x.PrjId == proj).ToList();
//            return PartialView(res);
//        }
//        public JsonResult SavePaymentSchedule(PaymentScheduleData[] schd, long? ProjId)
//        {
//            try
//            {
//                var uid = User.Identity.GetUserId<long>();
//                var pmtsData = new XElement("PaymentSchedule", schd.Select(x => new XElement("PmtData",
//                    new XAttribute("ProjId", ProjId),
//                    new XAttribute("Descr", x.desc),
//                    new XAttribute("mile", x.descid),
//                    new XAttribute("CompPerc", x.c_per),
//                    new XAttribute("CompAmt", x.c_amt),
//                    new XAttribute("Sr", Array.IndexOf(schd, x))))).ToString();
//                var ids = db.SP_Add_PaymentSchedule(pmtsData).ToList();

//                //saving details here
//                var pmtsDets = new XElement("PaymentSchedule", ids.Select(x => new XElement("PmtData",
//                    new XAttribute("PmtId", x.Id),
//                    new XAttribute("WP", schd[(int)x.sid].w_per),
//                    new XAttribute("WA", schd[(int)x.sid].w_amt),
//                    (schd[(int)x.sid].rem is null) ? null : new XAttribute("Rems", schd[(int)x.sid].rem),
//                    new XAttribute("Crtr", uid)))).ToString();
//                db.SP_Add_PaymentDetail(pmtsDets);
//                return Json(true);
//            }
//            catch
//            {
//                return Json(false);
//            }
//        }
//        public ActionResult Prj_InventoryStockIn(long? Prj_Id)
//        {
//            ViewBag.Prj_Id = Prj_Id;
//            return PartialView();
//        }
//        //public ActionResult Prj_Inventory(long? Prj_Id)
//        //{
//        //    ViewBag.Prj_Id = Prj_Id;
//        //    return PartialView();
//        //}



//        public ActionResult Prj_Demand(long? Prj_Id)
//        {
//            ViewBag.Prj_Id = Prj_Id;
//            return PartialView();
//        }
//        public ActionResult Prj_DemandOrders(long? Prj_Id)
//        {
//            var res = db.Sp_Get_Stockout_Req_Prj(Prj_Id).ToList();
//            return PartialView(res);
//        }
//        public ActionResult Prj_PurchaseReq(long? Prj_Id)
//        {
//            var res = db.Inventory_Purchase_Req.Where(x => x.Prj_Id == Prj_Id).ToList();
//            return PartialView(res);
//        }
//        public ActionResult Prj_Services(long? Prj_Id, string Prj_Type)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var re = db.Sp_Get_Employee_UserId(userid).FirstOrDefault();
//            ViewBag.Depart = re.Department;
//            var res1 = db.Services_Purchase_Req.Where(x => x.Prj_Id == Prj_Id && x.Status != PurchaseRequisitionStatus.Purchase_Order_Generated.ToString()).ToList();
//            var res2 = db.Service_PurchaseOrder.Where(x => x.Prj_Id == Prj_Id).ToList();
//            ViewBag.Prj_Type = Prj_Type;
//            var res = new Project_Services { Req = res1, PO = res2 };
//            return PartialView(res);
//        }

//        public ActionResult Prj_OtherExp(long? Prj_Id)
//        {
//            var res = db.Other_Expense.Where(x => x.Prj_Id == Prj_Id).ToList();
//            return PartialView(res);
//        }

//        public ActionResult UploadBOQ()
//        {
//            return PartialView();
//        }
//        public JsonResult Upload_BOQ(List<ExcelMilestoneData> AllData, long Prj_Id )
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//            var Dep_Id = db.Employee_Designations.Where(x => x.Emp_Id == EmpId).Select(x => (long)x.Department_Id).FirstOrDefault();
//            foreach (var cd in AllData)
//            {
//                var allchart = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Prj_Id == Prj_Id).ToList();
//                var c = Array.ConvertAll(cd.Sr.Trim().Split('.'), int.Parse);

//                if (c.Count() == 1)
//                {
//                    if (!allchart.Any(x => x.Prj_Id == Prj_Id && x.Description == cd.Description && x.Title == cd.Title && x.Amount == cd.Amount && x.Start_Date == cd.Startdate && x.Deadline == cd.Deadline))
//                    {
//                        Return res1 = (Return)(this.Add_MTS(Prj_Id, cd.Title, cd.Description, cd.Unit,cd.No,cd.L, cd.H, cd.B, cd.Qty, cd.Rate, cd.Amount, cd.Deadline, null, "Milestone", cd.Startdate, Dep_Id).Data);
//                        if (!res1.Status)
//                        {
//                            return Json(res1);
//                        }
//                    }
//                }
//                else if (c.Count() == 2)
//                {
//                    if (!allchart.Any(x => x.Prj_Id == Prj_Id && x.Description == cd.Description && x.Title == cd.Title && x.Amount == cd.Amount && x.Start_Date == cd.Startdate && x.Deadline == cd.Deadline))
//                    {
//                        string nc = c[0].ToString();
//                        var p_ref = AllData.Where(x => x.Sr == nc).SingleOrDefault();
//                        var parent = allchart.Where(x => x.Prj_Id == Prj_Id && x.Description == p_ref.Description && x.Title == p_ref.Title && x.Amount == p_ref.Amount && x.Start_Date == p_ref.Startdate && x.Deadline == p_ref.Deadline).SingleOrDefault();
//                        Return res1 = (Return)this.Add_MTS(Prj_Id, cd.Title, cd.Description, cd.Unit, cd.No, cd.L, cd.H, cd.B, cd.Qty, cd.Rate, cd.Amount, cd.Deadline, parent.Id, "Task", cd.Startdate, Dep_Id).Data;
//                        if (!res1.Status)
//                        {
//                            return Json(res1);
//                        }
//                    }
//                }
//                else if (c.Count() == 3)
//                {
//                    if (!allchart.Any(x => x.Prj_Id == Prj_Id && x.Description == cd.Description && x.Title == cd.Title && x.Amount == cd.Amount && x.Start_Date == cd.Startdate && x.Deadline == cd.Deadline))
//                    {
//                        string nc = c[0] + "." + c[1];
//                        var p_ref = AllData.Where(x => x.Sr == nc).SingleOrDefault();
//                        var parent = allchart.Where(x => x.Prj_Id == Prj_Id && x.Description == p_ref.Description && x.Title == p_ref.Title && x.Amount == p_ref.Amount && x.Start_Date == p_ref.Startdate && x.Deadline == p_ref.Deadline).SingleOrDefault();
//                        Return res1 = (Return)this.Add_MTS(Prj_Id, cd.Title, cd.Description, cd.Unit, cd.No, cd.L, cd.H, cd.B, cd.Qty, cd.Rate, cd.Amount, cd.Deadline, parent.Id, "SubTask", cd.Startdate, Dep_Id).Data;
//                        if (!res1.Status)
//                        {
//                            return Json(res1);
//                        }
//                    }
//                }
//            }
//            return Json( new Return { Msg = "Uploaded", Status = true });
//        }

//        [HttpGet]
//        public JsonResult GetProject(string q)
//        {
//            var res = db.Projects.Where(x => x.Name.Contains(q)).Select(x => new { id = x.Id, text = x.Name}).ToList();
//            return Json(new { items = res }, JsonRequestBehavior.AllowGet);
//        }
//        public ActionResult ProjectMapping()
//        {
//            var res = db.Projects.ToList();
//            return PartialView(res);
//        }
//        public void Proj()
//        {
//            var res = db.Projects.Where(x => x.CreatedBy_Dep == "Accounts").ToList();
//            foreach (var item in res)
//            {
//                var deps = db.Prj_Attendees.Where(x => x.PrjId == item.Id).FirstOrDefault();
//                item.CreatedBy_Dep = deps.Dep_Name;
//                db.Projects.Attach(item);
//                db.Entry(item).Property(x => x.CreatedBy_Dep).IsModified = true;
//                db.SaveChanges();
//            }
//        }
//        public ActionResult Noti(long? id, NotifierMsg? tp, long? noti)
//        {
//            Thread notiReader = new Thread(() => Notifier.ReadNotification((long)noti));
//            notiReader.Start();

//            return RedirectToAction("ProjectConfiguration", "ConstructProjectManagement", new { Id = id });
//        }
//        public ActionResult UploadMaterialStatement()
//        {
//            return PartialView();
//        }
//        public JsonResult Upload_MaterialStatement(List<ExcelMaterialStatementData> AllData, long Prj_Id)
//        {
//            long userid = long.Parse(User.Identity.GetUserId());
//            var EmpId = db.Sp_Get_EmployeeId(userid).FirstOrDefault();
//            var Dep_Id = db.Employee_Designations.Where(x => x.Emp_Id == EmpId).Select(x => (long)x.Department_Id).FirstOrDefault();
//            List<Material_Statements> msList = new List<Material_Statements>();
//            foreach (var cd in AllData)
//            {
//                var res = db.Prj_Milestone_Tasks_SubTasks.Where(x => x.Title == cd.Title && x.Prj_Id == Prj_Id).Select(x => x.Id).FirstOrDefault();
//                if (res != 0)
//                {
//                    Material_Statements upmst = new Material_Statements()
//                    {
//                        MilestoneID = res,
//                        ProjectID = Prj_Id,
//                        RequestedItemName = cd.Material,
//                        RequestedItemQty = Convert.ToDouble(cd.Qty),
//                        RequestedItemQtyType = cd.Unit,
//                        CreatedBy = userid,
//                        CreatedAt = DateTime.Now,
//                        Remarks = cd.Remarks,
//                        Rate = cd.Rate,
//                        Amount = cd.Amount
//                    };
//                    msList.Add(upmst);
//                }
//                else
//                {
//                    return Json(new Return { Msg = "Error Occured at " + cd.Title, Status = false });
//                }
				
                
//            }
//            foreach (var v in msList)
//            {
//                db.Material_Statements.Add(v);
//                db.SaveChanges();
//            }
//            return Json(new Return { Msg = "Uploaded", Status = true });
//        }
//        public ActionResult prj_MaterialComparrison(long? Prj_Id)
//        {
//            var res = db.Sp_Get_MSItemName(Prj_Id).ToList();
//            return PartialView(res);
//        }
//        public ActionResult prj_Monthly_Consumption()
//        {
//            return PartialView();
//        }
//        public ActionResult ProjectMonthlyConsumption(DateTime? FromDate, DateTime? ToDate, long PrjId)
//        {
//            if (FromDate == null || ToDate == null)
//            {
//                FromDate = DateTime.Now;
//                ToDate = DateTime.Now;
//                var res = db.Sp_Get_IssuedItems_Monthly(PrjId, FromDate, ToDate).ToList();
//                return PartialView(res);

//            }
//            else
//            {
//                var res = db.Sp_Get_IssuedItems_Monthly(PrjId, FromDate, ToDate).ToList();
//                return PartialView(res);
//            }
//        }
//        public ActionResult ItemWise(long? prj_Id)
//        {
//            var res = db.Sp_Get_Stockout_Prj(prj_Id).ToList();
//            return PartialView(res);
//        }
//        public ActionResult IssueNoteWise(long? prj_Id)
//        {
//            var res = db.Sp_Get_Stockout_Prj(prj_Id).ToList();
//            return PartialView(res);
//        }
//    }
//}