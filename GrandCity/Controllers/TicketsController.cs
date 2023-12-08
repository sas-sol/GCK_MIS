using System;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Microsoft.AspNet.Identity;
using System.IO;
using MeherEstateDevelopers.Filters;
using MeherEstateDevelopers.Models;
using MeherEstateDevelopers;

namespace Management_Information_System.Controllers
{
    [Authorize]
    [LogAction]
    public class TicketsController : Controller

    {
        // GET: Tickets
        Grand_CityEntities db = new Grand_CityEntities();
        //SAG_HRMS_DBEntities db2 = new SAG_HRMS_DBEntities();
        public ActionResult CreateTicket()
        {
            //var All = db.Users.Where(x => x.Roles.Any(y => y.Name == "Ticket Resolver")).ToList();
            ViewBag.Title = new SelectList(db.Sp_Get_Ticket_Type().ToList(), "Id", "Name");
            ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Department.ToString()), "Id", "Name");
            return PartialView();
        }
        public ActionResult ReceptionTicketing()
        {

            return View();
        }
        //public ActionResult CreateReceptionTicket()
        //{
        //    ViewBag.Title = new SelectList(db.Sp_Get_Ticket_Type().ToList(), "Id", "Name");
        //    //ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Department.ToString()), "Id", "Name");
        //    ViewBag.Department = new SelectList(db2.Employee_Dep_Comp.Where(x => x.Status == "Active").OrderBy(x => x.Department), "Id", "Department");
        //    return PartialView();
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateTicket(Ticket Tick)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Add_Ticket(Tick.AssignedTo, userid, Tick.Description, Tick.Department, Tick.Dep_Id, Tick.Title, Tick.Customer, Tick.Name, Tick.Mobile_No, Tick.Address, Tick.Deadline).FirstOrDefault();

            if (res == null || res.Ticket_Id == 0)
            {
                return Json(false.ToString());
            }

            //var firstrep = db.Sp_Get_Employee_UserId(Tick.AssignedTo).FirstOrDefault().Reporting_To;
            var firstrep = '1'; //db2.Emp_Profile.Where(x => x.EmployeeID == Tick.AssignedTo).FirstOrDefault().EmployeeID;

            //var reportingman = db.Sp_Get_Employee_Parameter(firstrep).FirstOrDefault();
            var hod = db.Sp_Get_Reporting_HeadOfDepartment(Tick.AssignedTo).ToList();
            var a = db.Sp_Add_TicketAttendee(userid, res.Ticket_Id).FirstOrDefault();
            var b = db.Sp_Add_TicketAttendee(Tick.AssignedTo, res.Ticket_Id).FirstOrDefault();
            foreach (var item in hod)
            {
                var e = db.Sp_Add_TicketAttendee(item.Id, res.Ticket_Id).FirstOrDefault();
            }
            //if (reportingman != null)
            //{
            //    var d = db.Sp_Add_TicketAttendee(reportingman.loginId, res.Ticket_Id).FirstOrDefault();
            //}

            var c = db.Sp_Add_Ticket_Comments("Created Ticket", res.Ticket_Id, userid, "Text");
            db.Sp_Add_Activity(userid, "Created New Tiket " + Tick.Title, "Create", "Activity_Record", "Tickets", userid);

            Sp_Get_Ticket_Parameter_Result Tik = new Sp_Get_Ticket_Parameter_Result()
            {
                Address = Tick.Address,
                AssignedTo = Tick.AssignedTo,
                CreatedBy = userid,
                Customer = Tick.Customer,
                Cust_Name = Tick.Name,
                Department = Tick.Department,
                Dep_Id = Tick.Dep_Id,
                Description = Tick.Description,
                Id = Convert.ToInt32(res.Ticket_Id),
                Mobile_No = Tick.Mobile_No,
                Status = "Pending",
                Ticket_No = res.Ticket_No,
                Title = Tick.Title
            };
            var attend = db.Sp_Get_TicketAttendee(res.Ticket_Id).ToList();
            foreach (var item in attend)
            {
                Notifier.Notify(userid, item.UserId, NotifierMsg.Ticket, new object[] { Tik }, NotifierMsg.Ticket.ToString());
            }
            if (Tick.Customer == true)
            {
                SmsService sms = new SmsService();
                sms.SendMsg("Dear Customer, Your complaint has been registered your Ticket is " + res.Ticket_No, Tick.Mobile_No);
            }
            return Json(new { Status = true, Ticket_No = res.Ticket_No, Ticket_Id = res.Ticket_Id });
        }
        public JsonResult UploadTickets(long Tick_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            int index = 0;
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
                index++;
                if (hpf.ContentLength == 0)
                    continue;
                var extension = Path.GetExtension(hpf.FileName);
                if (!Directory.Exists(Server.MapPath("~/Repository/Ticket/" + Tick_Id + "")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Repository/Ticket/" + Tick_Id + ""));
                }
                string savedFileName = Path.Combine(Server.MapPath("~/Repository/Ticket/" + Tick_Id + "/"), hpf.FileName);
                hpf.SaveAs(savedFileName);
                db.Sp_Add_Repository_File("", extension, hpf.FileName, "Ticket", Tick_Id, "/Repository/Ticket/" + Tick_Id + "/" + hpf.FileName, null, userid, 1);
            }
            return Json(new { Status = true, Msg = "Successfully Saved" });
        }
        public ActionResult AllTickets()
        {
            ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Department.ToString()), "Id", "Name");
            return View();
        }
        public ActionResult SearchTicket(DateTime? From, DateTime? To, int?[] Dep_Id)
        {
            string chids = "";
            if (Dep_Id != null)
            {
                chids = new XElement("ChPoDd", Dep_Id.Select(x => new XElement("Details",
                                 new XAttribute("Ids", x)
                                 ))).ToString();
            }
            var res = db.Sp_Get_Tickets(From, To, chids).ToList();
            return PartialView(res);
        }
        public ActionResult HODSearchTicket(DateTime? From, DateTime? To, long?[] UserId)
        {
            string chids = null;
            long userid = long.Parse(User.Identity.GetUserId());
            if (UserId != null)
            {
                if (UserId[0] != null)
                {

                    chids = new XElement("User", UserId.Select(x => new XElement("Ids",
                                     new XAttribute("Ids", x)
                                     ))).ToString();
                }
            }
            if (chids == null)
            {
                var empid = db.Sp_Get_Employee_UserId(userid).FirstOrDefault().Id;
                var hodusers = db.Sp_Get_ManagerStatus(empid).ToList();
                chids = new XElement("User", hodusers.Select(x => new XElement("Ids",
                                (x.Loginid == null) ? null : new XAttribute("Ids", x.Loginid)
                                ))).ToString();
            }
            var res = db.Sp_Get_HODTickets(From, To, chids).ToList();
            return PartialView(res);
        }
        public ActionResult MyTickets()
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var empid = db.Sp_Get_Employee_UserId(userid).FirstOrDefault().Id;
            ViewBag.Users = new SelectList(db.Sp_Get_ManagerStatus(empid).ToList(), "Loginid", "Name");
            db.Sp_Add_Activity(userid, "Accessed My Tickets", "Read", "Activity_Record", ActivityType.Page_Access.ToString(), userid);

            return View();
        }
        public ActionResult SearchMyTicket(DateSearch D)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Get_Tickets_CreatedMe(D.From, D.To, userid).ToList();
            var res2 = db.Sp_Get_Tickets_Assigned(D.From, D.To, userid).ToList();
            var res = new MyTickets { AssignedTickets = res2, CreateTickets = res1 };
            return PartialView(res);
        }
        public ActionResult ResolvedCustomersTickets()
        {

            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_Tickets_Customer(DateTime.Now, DateTime.Now, "Resolved");
            return PartialView(res);
        }
        public ActionResult PendingCustomersTickets()
        {

            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Get_Tickets_Customer(DateTime.Now, DateTime.Now, "Pending");
            return PartialView(res);
        }
        public ActionResult TicketTypes()
        {
            ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Department.ToString()), "Id", "Name");
            var res = db.Sp_Get_Ticket_Type().ToList();
            return View(res);
        }
        public JsonResult DepTicketsTypes(int Id)
        {
            var res = db.Sp_Get_TicketType_Dep(Id).ToList();
            return Json(res);
        }
        public JsonResult AddTicketType(int DepId, string Title)
        {
            var res = db.Sp_Add_Ticket_Type(DepId, Title).FirstOrDefault();

            long userid = long.Parse(User.Identity.GetUserId());

            db.Sp_Add_Activity(userid, "Added New Ticket Type", "Create", "Activity_Record", "Tickets", userid);
            return Json(res);
        }
        public JsonResult DeleteTicketType(long Id)
        {
            db.Sp_Delete_Ticket_Type(Id);

            long userid = long.Parse(User.Identity.GetUserId());

            db.Sp_Add_Activity(userid, "Deleted Ticket" + Id + " At " + DateTime.Now.ToString(), "Delete", "Activity_Record", "Tickets", userid);
            return Json(true);
        }
        // [Route("Tickets/TicketDetails/{Id:long}", Name = "TicketDetails")]
        public ActionResult TicketDetails(long Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            ViewBag.Userid = userid;
            var res1 = db.Sp_Get_Ticket_Parameter(Id).SingleOrDefault();
            ViewBag.Department = new SelectList(db.Comp_Dep_Desig.Where(x => x.Type == CompDepDes.Department.ToString()), "Id", "Name");
            var res2 = db.Repository_Files.Where(x => x.Module_Id == Id && x.Module == "Ticket").ToList();
            var res = new TicketDetails { Files = res2, Ticket = res1 };
            return View(res);
        }
        public JsonResult ForwardTicket(long Id, int Dep_Id, string Dep, long AssignedTo, string Reason)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res1 = db.Sp_Update_Ticket_Forward(Id, Dep, Dep_Id, AssignedTo);
            var Tick = db.Sp_Get_Ticket_Parameter(Id).SingleOrDefault();
            var res2 = db.Sp_Add_TicketAttendee(AssignedTo, Id).FirstOrDefault();
            db.Sp_Add_Ticket_Comments("Forward Ticket to : " + Dep + " to : " + Tick.AssignedTo_Name + ". Reason : " + Reason, Id, userid, "Text");

            db.Sp_Add_Activity(userid, "Forward Ticket to : " + Dep + " to : " + Tick.AssignedTo_Name + ". Reason : " + Reason + " At " + DateTime.Now.ToString(), "Update", "Activity_Record", "Tickets", userid);
            var res = db.Sp_Get_TicketAttendee(Id).ToList();
            foreach (var item in res)
            {
                Notifier.Notify(userid, item.UserId, NotifierMsg.TicketForward, new object[] { Tick }, NotifierMsg.Ticket.ToString());
            }

            return Json(true);
        }
        public JsonResult Reopenticket(long Id, string Reason)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Update_Ticket_Status(Id, "Pending");
            var Tick = db.Sp_Get_Ticket_Parameter(Id).SingleOrDefault();

            db.Sp_Add_Ticket_Comments(Tick.CreatedBy_Name + " Reopen the Ticket because : " + Reason, Id, userid, "Text");
            db.Sp_Add_Activity(userid, Tick.CreatedBy_Name + " Reopen the Ticket because : " + Reason + " At " + DateTime.Now.ToString(), "Delete", "Activity_Record", "Tickets", userid);

            var res1 = db.Sp_Get_TicketAttendee(Id).ToList();
            foreach (var item in res1)
            {
                Notifier.Notify(userid, item.UserId, NotifierMsg.TicketStatusUpdate, new object[] { Tick }, NotifierMsg.Ticket.ToString());
            }
            return Json(true);
        }
        public JsonResult UpdateStatus(string Status, long Ticket_Id)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            var res = db.Sp_Update_Ticket_Status(Ticket_Id, Status);
            var Tick = db.Sp_Get_Ticket_Parameter(Ticket_Id).SingleOrDefault();

            db.Sp_Add_Ticket_Comments("Update Status to : " + Status, Ticket_Id, userid, "Text");
            db.Sp_Add_Activity(userid, "Update Status to : " + Status + " At " + DateTime.Now.ToString(), "Delete", "Activity_Record", "Tickets", userid);

            var res1 = db.Sp_Get_TicketAttendee(Ticket_Id).ToList();
            foreach (var item in res1)
            {
                Notifier.Notify(userid, item.UserId, NotifierMsg.TicketStatusUpdate, new object[] { Tick }, NotifierMsg.Ticket.ToString());
            }

            if (Tick.Customer == true && Tick.Status == "Resolved")
            {
                SmsService sms = new SmsService();
                sms.SendMsg("Dear Customer, Your complaint no " + Tick.Ticket_No + " of " + Tick.Title + " has been Resolved.", Tick.Mobile_No);
            }
            return Json(true);
        }
        // Ticket Notification
        public ActionResult TicketNoti(long? id, NotifierMsg? tp, long? noti)
        {
            Thread notiReader = new Thread(() => Notifier.ReadNotification((long)noti));
            notiReader.Start();
            return RedirectToAction("TicketDetails", "Tickets", new { Id = id });
        }
    }
}