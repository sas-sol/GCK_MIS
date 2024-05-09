using MeherEstateDevelopers.Filters;
using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static MeherEstateDevelopers.MvcApplication;

namespace MeherEstateDevelopers.Controllers
{
    [LogAction]
    [Authorize]
    public class RemindersController : Controller
    {
        private Grand_CityEntities db = new Grand_CityEntities();

        [NoDirectAccess] public ActionResult NewReminder(string modId, string mode,string title)
        {
            ViewBag.modId = modId;
            ViewBag.mode = mode;
            ViewBag.ttl = title;
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult NewReminder(MIS_Reminders mir)
        {
            var uid = User.Identity.GetUserId<long>();
            var uname = db.Users.Where(x => x.Id == uid).Select(x => x.Name).FirstOrDefault();
            try
            {
                if(mir.RemindOn <= DateTime.Now)
                {
                    throw new Exception();
                }
                mir.CreatedAt = DateTime.Now;
                mir.Description = mir.UnParsedDesc;
                mir.CreatedBy_Name = uname;
                mir.CreatedBy_Id = uid;
                mir.Status = ReminderStatus.Open.ToString();
                mir.ReminderNo = "REM-" + db.Sp_Get_ReceiptNo("Reminders").FirstOrDefault();
                db.MIS_Reminders.Add(mir);
                db.SaveChanges();
                return Json(new { Status = true, RemNum = mir.ReminderNo });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, RemNum = string.Empty, ex });
            }
        }

        [NoDirectAccess] public ActionResult ReminderDetailsPartial(long rem)
        {
            var res = db.MIS_Reminders.Where(x => x.Id == rem).FirstOrDefault();
            return PartialView(res);
        }

        [NoDirectAccess] public ActionResult ReminderDetails(long rem)
        {
            var res = db.MIS_Reminders.Where(x => x.Id == rem).FirstOrDefault();
            return View(res);
        }

        public JsonResult UpdateReminder(MIS_Reminders mir)
        {
            return Json(false);
        }

        [NoDirectAccess] public ActionResult ReminderComments(long remId)
        {
            ViewBag.Fileid = remId;
            var res = db.Sp_Get_ReminderComments(remId).ToList();
            return PartialView(res);
        }

        [HttpPost]
        public JsonResult PostReminderComment(long Rem_Id, string Comment)
        {
            long userid = long.Parse(User.Identity.GetUserId());
            int index = 0;
            List<Reminder_Comments> fd = new List<Reminder_Comments>();
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[index] as HttpPostedFileBase;
                index++;
                if (hpf.ContentLength == 0)
                    continue;
                var imageName = Path.GetExtension(hpf.FileName);
                if (!Directory.Exists(Server.MapPath("~/RemindersData/" + Rem_Id + "")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/RemindersData/" + Rem_Id + ""));
                }
                string savedFileName = Path.Combine(Server.MapPath("~/RemindersData/" + Rem_Id + "/"), hpf.FileName);
                hpf.SaveAs(savedFileName);
                db.Sp_Add_ReminderComment(Rem_Id, hpf.FileName, "file", userid);
                Reminder_Comments f = new Reminder_Comments()
                {
                    Comment = Comment,
                    Ticket_Id = Rem_Id,
                    Msg_Type = "file"
                };
                fd.Add(f);
            }
            if (!string.IsNullOrEmpty(Comment))
            {
                db.Sp_Add_ReminderComment(Rem_Id, Comment, "Text", userid);
                Reminder_Comments f = new Reminder_Comments()
                {
                    Comment = Comment,
                    Ticket_Id = Rem_Id,
                    Msg_Type = "Text"
                };
                fd.Add(f);
            }
            return Json(fd);
        }

        [NoDirectAccess] public ActionResult RemindersWidget()
        {
            var uid = User.Identity.GetUserId<long>();
            var rems = db.MIS_Reminders.Where(x => x.CreatedBy_Id == uid && x.Status != "Closed" && x.RemindOn.Value <= DateTime.Now).ToList();
            return PartialView(rems);
        }

        public JsonResult CloseReminder(long remId)
        {
            try
            {
                var res = db.MIS_Reminders.Where(x => x.Id == remId).FirstOrDefault();
                res.Status = ReminderStatus.Closed.ToString();
                db.MIS_Reminders.Attach(res);
                db.Entry(res).Property(x => x.Status).IsModified = true;
                db.SaveChanges();

                var cmt = db.Sp_Add_ReminderComment(remId, "Reminder has been Closed.", "text", User.Identity.GetUserId<long>());

                return Json(true);
            }
            catch(Exception e)
            {
                return Json(false);
            }
        }

        public JsonResult UpdateReminderDate(long remId, DateTime newRemindDate)
        {
            try
            {
                if(newRemindDate <= DateTime.Now)
                {
                    throw new Exception();
                }
                var res = db.MIS_Reminders.Where(x => x.Id == remId).FirstOrDefault();
                res.RemindOn = newRemindDate;
                res.IsNotified = null;
                db.MIS_Reminders.Attach(res);
                db.Entry(res).Property(x => x.RemindOn).IsModified = true;
                db.Entry(res).Property(x => x.IsNotified).IsModified = true;
                db.SaveChanges();
                var cmt = db.Sp_Add_ReminderComment(remId, "Reminder date has been updated to : " + newRemindDate.ToString() + ".", "text", User.Identity.GetUserId<long>());
                return Json(true);
                
            }catch(Exception ex)
            {
                return Json(false);
            }
        }

        public JsonResult UpdateReminderNotifyMode(long remId, string modeType, bool status)
        {
            try
            {
                var res = db.MIS_Reminders.Where(x => x.Id == remId).FirstOrDefault();
                if(modeType == "Email")
                {
                    res.EmailNotify = status;
                    db.MIS_Reminders.Attach(res);
                    db.Entry(res).Property(x => x.EmailNotify).IsModified = true;
                    db.SaveChanges();
                }
                else if(modeType == "SMS")
                {
                    res.SMSNotify = status;
                    db.MIS_Reminders.Attach(res);
                    db.Entry(res).Property(x => x.SMSNotify).IsModified = true;
                    db.SaveChanges();
                }
                if(status)
                {
                    var cmt = db.Sp_Add_ReminderComment(remId, "Reminder mode been updated to : " + modeType, "text", User.Identity.GetUserId<long>());
                }
                else
                {
                    var cmt = db.Sp_Add_ReminderComment(remId, modeType + " notification mode has been removed.", "text", User.Identity.GetUserId<long>());
                }
                return Json(true);

            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        [NoDirectAccess] public ActionResult Index()
        {
            return View();
        }
        
        [NoDirectAccess] public ActionResult GetRemindersByType(string type)
        {
            var uid = User.Identity.GetUserId<long>();
            var res = db.MIS_Reminders.Where(x => x.Status == type && x.CreatedBy_Id == uid).ToList();
            return PartialView(res);
        }

        [NoDirectAccess] public ActionResult GetUpcomingRemindersByModule(string module,long modId)
        {
            var res = db.MIS_Reminders.Where(x => x.ModuleType == module && x.ModuleId == modId).ToList();
            return PartialView(res);
        }

        public void DispatchReminders()
        {
            var res = db.Sp_Get_DispatchableReminders(null).ToList();
            var usrs = res.Select(x => x.CreatedBy_Id).Distinct().ToList();
            var emps = db.Employees.Where(x => usrs.Contains(x.loginId)).Select(x => new { x.Id, x.Email, x.Company_Email, x.Mobile_1, x.Mobile_2, x.loginId }).ToList();
            foreach(var v in res)
            {
                if(v.EmailNotify == true)
                {
                    //send email
                    string eml = emps.Where(x => x.loginId == v.CreatedBy_Id).Select(x => x.Email).FirstOrDefault();

                    if(string.IsNullOrEmpty(eml))
                    {
                        eml = emps.Where(x => x.loginId == v.CreatedBy_Id).Select(x => x.Company_Email).FirstOrDefault();
                    }
                    if(!string.IsNullOrEmpty(eml))
                    {
                        postOffice.DispatchMail("Reminder : " + v.ReminderNo, new string[] { "Title : " + v.Title, "Remind At : " + v.RemindOn.ToString() }, "http://192.168.11.138/Reminders/ReminderDetails?rem=" + v.Id.ToString(), eml);
                        //tag lagao history main
                        var cmtEml = db.Sp_Add_ReminderComment(v.Id, "Email for reminder sent at " + eml, "text", 0);
                    }
                    else
                    {
                        //tag lagao history main
                        var cmtEml = db.Sp_Add_ReminderComment(v.Id, "Failed to send notifying email at " + eml, "text", 0);
                    }
                }
                if(v.SMSNotify == true)
                {
                    //send sms here
                    string phone = emps.Where(x => x.loginId == v.CreatedBy_Id).Select(x => x.Mobile_1).FirstOrDefault();

                    if (string.IsNullOrEmpty(phone))
                    {
                        phone = emps.Where(x => x.loginId == v.CreatedBy_Id).Select(x => x.Mobile_2).FirstOrDefault();
                    }
                    if (!string.IsNullOrEmpty(phone))
                    {

                        //tag lagao history main
                        SmsService smsService = new SmsService();
                        smsService.SendMsg("Reminder for " + v.ReminderNo + "\nTitle : " + v.Title, phone);
                        var cmtSms = db.Sp_Add_ReminderComment(v.Id, "SMS for reminder sent at : " + phone, "text", 0);
                    }
                    else
                    {
                        //tag lagao history main
                        var cmtSms = db.Sp_Add_ReminderComment(v.Id, "Failed to send notifying SMS at " + phone, "text", 0);
                    }
                }
                var resdes = db.Sp_Update_ReminderNotify(v.Id);
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