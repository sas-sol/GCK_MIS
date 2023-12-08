using Microsoft.Owin;
using Owin;
using Hangfire;
using System;
using MeherEstateDevelopers.Models;
using System.Linq;
using MeherEstateDevelopers.Controllers;
using System.Collections.Generic;
using execldataimport.Controllers;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;
using System.Web.Hosting;
using System.Web;
using Hangfire.Annotations;
using System.Configuration;

[assembly: OwinStartupAttribute(typeof(MeherEstateDevelopers.Startup))]
namespace MeherEstateDevelopers
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();

            GlobalConfiguration.Configuration.UseSqlServerStorage("NewIdentity");
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangFireAuthorizationFilter() }
            });

            var server = new BackgroundJobServer();

            var manager = new RecurringJobManager();
            //manager.RemoveIfExists("daily-cancellation-sms");
            //manager.RemoveIfExists("Monthly-Generate-Electricity-Bills");
            //manager.RemoveIfExists("Monthly-Generate-Service-Bills");
            //manager.RemoveIfExists("reminders-dispatcher");
            
            //////////////////////////////////////////////////////////////////
            
            //manager.AddOrUpdate("daily-cancellation-sms", Hangfire.Common.Job.FromExpression(() => WarningMsg()), "0 1 1,15 * *");
            //manager.AddOrUpdate("Monthly-Generate-Electricity-Bills", Hangfire.Common.Job.FromExpression(() => GenerateElectircBills()), "30 0 1 * *");
            //manager.AddOrUpdate("Monthly-Generate-Service-Bills", Hangfire.Common.Job.FromExpression(() => GenerateSerrviceBills()), "30 0 1 * *");
            //manager.AddOrUpdate("reminders-dispatcher", Hangfire.Common.Job.FromExpression(() => DispatchReminders()), "*/30 * * * *");
        }
        public void Attendance_Wrapper_Connectivity()
        {
            if (WebAppEnvironment.LastAttendanceSync is null)
            {
                WebAppEnvironment.Attendance_Machine_Conn_Check_Count++;
                if (WebAppEnvironment.Attendance_Machine_Conn_Check_Count >= 2)
                {
                    WebAppEnvironment.Attendance_Machine_Conn_Check_Count_Notify_Max_Limit++;
                    if (WebAppEnvironment.Attendance_Machine_Conn_Check_Count_Notify_Max_Limit >= 5)
                    {
                        return;
                    }

                    foreach (var v in WebAppEnvironment.AttendanceNoticingBodies)
                    {
                        //EmailService e = new EmailService();
                        //e.SendEmail("System has detected that attendance wrapper for MIS is not working. This could be either due to internet connectivity issue or Wrapper has been closed. Please ensure that connectivity of the wrapper with MIS has been restored. For your information, attendance wrapper is hosted @ 192.168.5.9. This is an autonomous email from MIS. Please donot reply or respond to this mail.", v, "MIS Attendance Wrapper Connectivity Loss");
                    }
                    WebAppEnvironment.Attendance_Machine_Conn_Check_Count = 0;
                }
            }
            else
            {
                WebAppEnvironment.LastAttendanceSync = null;
                WebAppEnvironment.Attendance_Machine_Conn_Check_Count = 0;
            }
        }
        //public void Cancellation_SMS_ChroneJob()
        //{
        //    using (MeherEstateDevelopers.Models.Grand_CityEntities db = new Grand_CityEntities())
        //    {
        //        DateTime dt_7_Back = DateTime.Now.AddDays(-7);

        //        var suspected_Files = db.Sp_HF_Chrone_CancellationSMS().ToList();

        //        //Second Cancelled
        //        var Update_Cancelled_File = suspected_Files.Where(x => x.First_notice != null).Where(x => (x.First_notice.Value.Date <= dt_7_Back.Date) && (x.Sec_Notice is null)).Select(x => new { x.Id, tp = 1, dt = DateTime.Now, x.Mobile_1, x.First_notice, x.File_Form_Id, x.Sec_Notice }).ToList();

        //        //Third Cancelled
        //        Update_Cancelled_File.AddRange(suspected_Files.Where(x => x.Sec_Notice != null && x.First_notice != null).Where(x => x.Sec_Notice.Value.Date.AddDays(7) == DateTime.Now).Select(x => new { x.Id, tp = 2, dt = DateTime.Now, x.Mobile_1, x.First_notice, x.File_Form_Id, x.Sec_Notice }).ToList());

        //        foreach (var item in Update_Cancelled_File)
        //        {
        //            var msgtext =
        //                    "Respected Customer,\n\r" +
        //                    "This is to inform you that your installment is still pending against your plot. A reminder message was also sent to you on (" + string.Format("{0:dd-MMM-yyyy}", item.First_notice.Value.ToLongDateString()) + ") along with a letter. You are requested to submit due instalments within next 7 days, otherwise your plot will be cancelled.\n\r" +
        //                    "Best Regards,\n\r" +
        //                    "Meher Estate Developers.\n\r" +
        //                    "042 – 111 724 786\n\r";
        //            try
        //            {
        //                SmsService smsService = new SmsService();
        //                smsService.SendMsg(msgtext, item.Mobile_1);
        //                db.Sp_HF_ChroneJob_UpdateFileCancellation(item.Id, DateTime.Now, item.tp);
        //            }
        //            catch (Exception ex)
        //            {
        //                db.Sp_Add_ErrorLog(ex.Message + ex.InnerException, "", ex.StackTrace, "", "");
        //            }
        //        }
        //    }
        //}
        public void GenerateSerrviceBills()
        {
            ServiceChargesController sc = new ServiceChargesController();
            sc.PlotIdsSC();
        }
        public void GenerateElectircBills()
        {
            ServiceChargesController sc = new ServiceChargesController();
            sc.GenerateElectricBillInMeterReading();
        }
        public void AdjustBalance()
        {
            FiletransferController sc = new FiletransferController();
            sc.AdjustIntallments();
        }
        public void MakeNewAttendancePage()
        {
            DateTime currDate = DateTime.Now;
            using (MeherEstateDevelopers.Models.Grand_CityEntities db = new Grand_CityEntities())
            {
                var emps = db.Employees.Where(x => x.Status == "Registered").Select(x => new { x.Id, x.Name, x.Employee_Code, x.Department_Designation, x.Att_Exempted, x.Roster }).ToList();
                List<AttendanceData> newAttendance = new List<AttendanceData>();
                foreach (var e in emps)
                {
                    newAttendance.Add(new AttendanceData
                    {
                        AttendanceDate = currDate,
                        Employee_Code = e.Employee_Code,
                        Employee_Id = e.Id,
                        Employee_Name = e.Name,
                        Employee_Department = e.Department_Designation,
                        Day_Of_Week = currDate.DayOfWeek.ToString(),
                        IsInvalid = (e.Att_Exempted == true) ? 1 : 0,
                        WorkCode = (int)(e.Roster ?? (long)0)
                    });
                }
                db.AttendanceDatas.AddRange(newAttendance);
                db.SaveChanges();
            }
        }
        //public void WarningMsg()
        //{
        //    using (Grand_CityEntities db = new Grand_CityEntities())
        //    {
        //        var res = db.Sp_Get_WarningSms().ToList();
        //        foreach (var item in res)
        //        {
        //            FileSystemController f = new FileSystemController();
        //            try
        //            {
        //                //f.TestAdjustIntallments(item.FileFormNumber);
        //                Sp_Get_CurrentOwner_File_Id_Result res1 = db.Sp_Get_CurrentOwner_File_Id(item.Id).FirstOrDefault();

        //                if (res1.Balance_Amount < -20)
        //                {
        //                    var msgtext = "Dear " + res1.Name + ",\n\r" +
        //                    "Your pending payable balance is PKR " + string.Format("{0:n0}", (res1.Balance_Amount * -1)) + "- against File Number " + res1.FileFormNumber + " as at " + string.Format("{0:dd-MMM-yyyy}", DateTime.Now) + ". The due date for paying outstanding balance is 5th of every month. You are requested to clear your dues at earliest to avoid any further inconvenience.\n\r" +
        //                    "If you have already made the payment please ignore this message\n\r" +
        //                    "For further information call at our help line.\n\r" +
        //                     "Meher Estate Developers\n\r" +
        //                     "042 - 111 724 786";

        //                    try
        //                    {
        //                        SmsService smsService = new SmsService();
        //                        smsService.SendMsg(msgtext, item.Mobile_1);
        //                        db.Sp_Add_FileComments(item.Id, msgtext, 0, "Text");
        //                    }
        //                    catch (Exception ee)
        //                    {

        //                        db.Sp_Add_ErrorLog(ee.Message + ee.InnerException, "", ee.StackTrace, "", "");

        //                    }
        //                }
        //            }
        //            catch (Exception eee)
        //            {
        //                db.Sp_Add_ErrorLog(eee.Message + eee.InnerException, "", eee.StackTrace, "", "");

        //            }
        //        }
        //        //Plot Messages for All Blocks
        //        var pres = db.Plots.Where(x => x.Status == "Registered").Select(x => x.Id).ToList();
        //        foreach (var item in pres)
        //        {
        //            var res1 = db.Sp_Get_PlotData(item).FirstOrDefault();
        //            var res2 = db.Sp_Get_PlotLastOwner(item).FirstOrDefault();
        //            var res3 = db.Sp_Get_PlotInstallments(item).ToList();
        //            var res4 = db.Sp_Get_ReceivedAmounts(item, Modules.PlotManagement.ToString()).ToList();
        //            var discount = db.Discounts.Where(x => x.Module_Id == item && x.Plot_Is_Cancelled == null && x.Module == Modules.PlotManagement.ToString()).ToList();

        //            PlotsController p = new PlotsController();
        //            p.UpdatePlotInstallmentStatus(res3, res4, discount, item);

        //            try
        //            {
        //                var bal = db.File_Plot_Balance.Where(x => x.File_Plot_Id == item && x.Module == Modules.PlotManagement.ToString()).FirstOrDefault();
        //                bal.Balance_Amount = Math.Round(bal.Balance_Amount);
        //                if (bal.Balance_Amount < -20)
        //                {
        //                    var msgtext = "Dear " + res2.Name + "\n\r," +
        //                    "Your Over due amount is PKR " + string.Format("{0:n0}", (bal.Balance_Amount * -1)) + "- against Plot Number " + res1.Plot_No + " - " + res1.Block_Name + ". The due date for paying outstanding balance is 5th of every month. You are requested to clear your dues at earliest to avoid any further inconvenience.\n\r" +
        //                    "If you have Already Cleared your dues then ignore this message or Contact our Help Desk Dial UAN 111 724 786";
        //                    try
        //                    {
        //                        SmsService smsService = new SmsService();
        //                        smsService.SendMsg(msgtext, res2.Mobile_1);
        //                        db.Sp_Add_PlotComments(res1.Id, msgtext, 0, "Text");
        //                    }
        //                    catch (Exception ee)
        //                    {
        //                        EmailService e = new EmailService();
        //                        e.SendEmail(res1.Plot_No, "taimoor@sasystems.solutions", "Msg Not Sent");
        //                    }
        //                }
        //            }
        //            catch (Exception eee)
        //            {
        //                EmailService e = new EmailService();
        //                e.SendEmail(res1.Plot_No, "taimoor@sasystems.solutions", "Msg Not Sent");
        //            }
        //        }
                //var plres = db.Sp_Get_SherAfghan_Balance_Amount().ToList();
                //foreach (var v in plres)
                //{
                //    try
                //    {
                //        var msgtext = "Dear " + res1.Name + ",\n\r" +
                //            "Your pending payable balance is PKR " + string.Format("{0:n0}", (res1.Balance_Amount * -1)) + "- against File Number " + res1.FileFormNumber + " as at " + string.Format("{0:dd-MMM-yyyy}", DateTime.Now) + ". You are requested to clear your dues to avoid any furthur inconvenience.\n\r" +
                //            "If you have already made the payment please ignore this message\n\r" +
                //            "For further information call at our help line.\n\r" +
                //             "Meher Estate Developers\n\r" +
                //             "042 - 111 724 786";

                //        try
                //        {
                //            SmsService smsService = new SmsService();
                //            smsService.SendMsg(msgtext, item.Mobile_1);
                //            db.Sp_Add_FileComments(item.Id, msgtext, 0, "Text");
                //        }
                //        catch (Exception ee)
                //        {

                //            db.Sp_Add_ErrorLog(ee.Message + ee.InnerException, "", ee.StackTrace, "", "");

                //        }
                //    }
                //    catch (Exception ex)
                //    { 
                //    }
                //}
        //    }
        //}
        public void Transfer()
        {
            using (MeherEstateDevelopers.Models.Grand_CityEntities db = new Grand_CityEntities())
            {
                var res1 = db.Sp_Get_RegisterFileDueAmount().ToList();

                foreach (var item in res1)
                {
                    var msgtext = "Respected Customer, \n\r" +
                          "Your Plot No. " + item.FileFormNumber + " is eligible for the first balloting of Sher Afghan Block. To Qualify, Please pay your Balloting Charges Rs. " + item.Balance_Amount + " Before 20th Oct 2020. In case of failure you will not Qualify for the First Balloting.\n\r" +
                          "If you have already Paid the Balloting Installment Please ignore this Message.\n\r" +
                          "For further information call at our help line.\n\r" +
                          "Meher Estate Developers\n\r" +
                          "042 - 111 724 786";

                    try
                    {
                        SmsService smsService = new SmsService();
                        smsService.SendMsg(msgtext, item.Mobile_1);
                        var res2 = db.Sp_Add_FileComments(item.Id, msgtext, 0, "Text");
                        if (!string.IsNullOrEmpty(item.Mobile_2) || !string.IsNullOrWhiteSpace(item.Mobile_2))
                        {
                            smsService.SendMsg(msgtext, item.Mobile_2);
                        }
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
            }
        }
        public void DispatchReminders()
        {
            using (MeherEstateDevelopers.Models.Grand_CityEntities db = new Grand_CityEntities())
            {
                var res = db.Sp_Get_DispatchableReminders(null).ToList();
                var usrs = res.Select(x => x.CreatedBy_Id).Distinct().ToList();
                var emps = db.Employees.Where(x => usrs.Contains(x.loginId)).Select(x => new { x.Id, x.Email, x.Company_Email, x.Mobile_1, x.Mobile_2, x.loginId }).ToList();
                foreach (var v in res)
                {
                    if (v.EmailNotify == true)
                    {
                        //send email
                        string eml = emps.Where(x => x.loginId == v.CreatedBy_Id).Select(x => x.Email).FirstOrDefault();

                        if (string.IsNullOrEmpty(eml))
                        {
                            eml = emps.Where(x => x.loginId == v.CreatedBy_Id).Select(x => x.Company_Email).FirstOrDefault();
                        }
                        if (!string.IsNullOrEmpty(eml))
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
                    if (v.SMSNotify == true)
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
                            smsService.SendMsg("Reminder for " + v.ReminderNo + "\nTitle : " + v.Title+"\nPlease Login to MIS to view details of this reminder.\n\n(Sent by MIS)", phone);
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
        }
    }
}
