using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MeherEstateDevelopers.Models
{
    public static class Notifier
    {
        public static void Notify(long from, long to, NotifierMsg type, object[] data, string Type)
        {
            string name = CurrentUserName(from);
            string msg = TransformNotifierMessage(type, data, name);
            string url = TransformNotifierUrl(type, data);
            if (from == to)
            {
                return;
            }

            long? res = SaveNotification(from, to, msg, url, Type);

            if (to == from)
            {
                return;
            }

            if (res > 0)
            {
                NotificationHub.notifyUser(new UserNotifierData()
                {
                    notID = (long)res,
                    createdAt = DateTime.Now,
                    fromUserId = from,
                    hitUrl = url,
                    notificationMsg = msg,
                    refreshUrl = GetRefreshUrl(type),
                    refreshWidget = GetRefreshWidget(type),
                    toUserId = to
                });
            }

            using (Grand_CityEntities db = new Grand_CityEntities())
            {
                var eml = db.Users.Where(x => x.Id == to).Select(x => x.Email).FirstOrDefault();
                PostNotifierMail(data, type, TransformNotifierMailUrl(type, res.ToString(), data), eml, name, from, to);
            }
        }
        public static void Notify(long from, NotifyPerson to, NotifierMsg type, object[] data, string Type)
        {

            string name = CurrentUserName(from);
            string msg = TransformNotifierMessage(type, data, name);
            string url = TransformNotifierUrl(type, data);

            if (from == to.UserID)
            {
                return;
            }

            long? res = SaveNotification(from, to.UserID, msg, url, Type);

            if (to.UserID == from)
            {
                return;
            }

            if (res > 0)
            {
                NotificationHub.notifyUser(new UserNotifierData()
                {
                    notID = (long)res,
                    createdAt = DateTime.Now,
                    fromUserId = from,
                    hitUrl = url,
                    notificationMsg = msg,
                    refreshUrl = GetRefreshUrl(type),
                    refreshWidget = GetRefreshWidget(type),
                    toUserId = to.UserID
                });
                PostNotifierMail(data, type, TransformNotifierMailUrl(type, res.ToString(), data), to.UserEmail, name, from, to.UserID);
            }
        }
        public static void Notify(long from, NotifyTo recievers, NotifierMsg type, object[] data, string Type)
        {
            string name = CurrentUserName(from);
            string msg = TransformNotifierMessageManagement(type, data);
            string url = TransformNotifierUrl(type, data);

            List<NotifyPerson> notiRcpnts = null;
            using (Grand_CityEntities db = new Grand_CityEntities())
            {
                if (recievers == NotifyTo.Audit)
                {
                    notiRcpnts = (from u in db.Users where u.Roles.Any(x => x.Name == "Audit") && u.Active == 1 select new NotifyPerson() { UserEmail = u.Email, UserID = u.Id, UserName = u.Name, UserProfileImage = string.Empty }).Distinct().ToList();
                }
                else if (recievers == NotifyTo.Full_Paid)
                {
                    notiRcpnts = (from u in db.Users where u.Roles.Any(x => x.Name == "Full Paid Notification") && u.Active == 1 select new NotifyPerson() { UserEmail = u.Email, UserID = u.Id, UserName = u.Name, UserProfileImage = string.Empty }).Distinct().ToList();
                }
            }

            foreach (var to in notiRcpnts)
            {
                if (from == to.UserID)
                {
                    return;
                }

                long? res = SaveNotification(from, to.UserID, msg, url, Type);

                if (to.UserID == from)
                {
                    return;
                }

                if (res > 0)
                {
                    NotificationHub.notifyUser(new UserNotifierData()
                    {
                        notID = (long)res,
                        createdAt = DateTime.Now,
                        fromUserId = from,
                        hitUrl = url,
                        notificationMsg = msg,
                        refreshUrl = GetRefreshUrl(type),
                        refreshWidget = GetRefreshWidget(type),
                        toUserId = to.UserID
                    });
                }

                using (Grand_CityEntities db = new Grand_CityEntities())
                {
                    var eml = db.Users.Where(x => x.Id == to.UserID).Select(x => x.Email).FirstOrDefault();
                    PostNotifierMail(data, type, TransformNotifierMailUrl(type, res.ToString(), data), eml, name, from, to.UserID);
                }
            }
        }
        public static void Notify(long from, List<long> to, NotifierMsg type, object[] data, string Type)
        {
            //string msg = string.Empty;
            //string url = TransformNotifierUrl(type, data);

            ////Some specific types for which an alternate management message has to be transformed
            //if (type == NotifierMsg.MilestoneAssigned || type == NotifierMsg.MilestoneUpdateRequest)
            //{
            //    msg = TransformNotifierMessageManagement(type, data);
            //}
            ////Just regular user messages
            //else
            //{
            //    string name = CurrentUserName(from);
            //     msg = TransformNotifierMessage(type, data, name);

            //}

            //foreach (var v in to)
            //{
            //    if (v != from)
            //    {
            //        long? res = SaveNotification(from, v, msg, url, Type);
            //        if (res > 0)
            //        {

            //            NotificationHub.notifyUser(new UserNotifierData()
            //            {
            //                notID = (long)res,
            //                createdAt = DateTime.Now,
            //                fromUserId = from,
            //                hitUrl = url,
            //                notificationMsg = msg,
            //                refreshUrl = url,
            //                toUserId = v
            //            });
            //        }
            //    }
            //}
        }
        public static void Notify(long from, List<NotifyPerson> to, NotifierMsg type, object[] data, string Type)
        {

            string name = CurrentUserName(from);
            string msg = TransformNotifierMessage(type, data, name);
            string url = TransformNotifierUrl(type, data);

            foreach (var v in to)
            {
                if (v.UserID != from)
                {
                    long? res = SaveNotification(from, v.UserID, msg, url, Type);
                    if (res > 0)
                    {
                        NotificationHub.notifyUser(new UserNotifierData()
                        {
                            notID = (long)res,
                            createdAt = DateTime.Now,
                            fromUserId = from,
                            hitUrl = url,
                            notificationMsg = msg,
                            refreshUrl = GetRefreshUrl(type),
                            refreshWidget = GetRefreshWidget(type),
                            toUserId = v.UserID
                        });
                        PostNotifierMail(data, type, TransformNotifierMailUrl(type, res.ToString(), data), v.UserEmail, name, from, v.UserID);
                    }
                }
            }

        }
        private static long? SaveNotification(long from, long to, string msg, string hiturl, string Type)
        {
            using (Grand_CityEntities db = new Grand_CityEntities())
            {
                return db.Sp_Add_Notification(from, to, msg, hiturl, DateTime.Now, Type).FirstOrDefault();
            }
        }
        private static string CurrentUserName(long Id)
        {
            using (Grand_CityEntities db = new Grand_CityEntities())
            {
                return db.Sp_Get_UserDetails_Short(Id).SingleOrDefault().Name;
            }
        }
        private static string TransformNotifierMessage(NotifierMsg t, object[] moduleData, string Name)
        {
            if (t == NotifierMsg.Ticket)
            {
                return Name + " Generated a Ticket.";
            }
            else if (t == NotifierMsg.TicketComment)
            {
                Sp_Get_Ticket_Parameter_Result data = (Sp_Get_Ticket_Parameter_Result)moduleData[0];
                return Name + " Commented on Ticket no. " + data.Ticket_No;
            }
            else if (t == NotifierMsg.TicketForward)
            {
                Sp_Get_Ticket_Parameter_Result data = (Sp_Get_Ticket_Parameter_Result)moduleData[0];
                return Name + " Forwarded Ticket no. " + data.Ticket_No;
            }
            else if (t == NotifierMsg.TicketStatusUpdate)
            {
                Sp_Get_Ticket_Parameter_Result data = (Sp_Get_Ticket_Parameter_Result)moduleData[0];
                if (data.Status == "Resolved")
                {
                    return Name + " Resolve Ticket No. " + data.Ticket_No;
                }
                else if (data.Status == "In Progress")
                {
                    return Name + " Mark Ticket No. " + data.Ticket_No + " Pending";
                }
                else if (data.Status == "Pending")
                {
                    return Name + " Reopen Ticket No. " + data.Ticket_No;
                }
            }
            else if (t == NotifierMsg.Demand_Requisitions)
            {
                Inventory_Demand_Req re = (Inventory_Demand_Req)moduleData[0];
                return re.ApprovedBy + " Generate a Demand Requisition";
            }
            else if (t == NotifierMsg.Demand_Approved)
            {
                Inventory_Demand_Req re = (Inventory_Demand_Req)moduleData[0];
                return Name +  " Approve a Demand Requisition ";
            }
            else if (t == NotifierMsg.Issuance_Request)
            {
                NameValuestring re = (NameValuestring)moduleData[0];
                return  Name + " Generate a new demand order " + re.Name;
            }

            return string.Empty;
        }
        private static string TransformNotifierMessageManagement(NotifierMsg t, object[] moduleData)
        {
            if (t == NotifierMsg.Unverified)
            {
                Sp_Get_PlotData_Result Plot = (Sp_Get_PlotData_Result)moduleData[0];
                return "Plot Number " + Plot.Plot_No + " Block: " + Plot.Block_Name + " Type: " + Plot.Type + " is unverified";
            }
            if (t == NotifierMsg.Full_Paid_Plots)
            {
                Sp_Get_PlotData_Result Plot = (Sp_Get_PlotData_Result)moduleData[0];
                return "Plot Number " + Plot.Plot_No + " Block: " + Plot.Block_Name + " Type: " + Plot.Type + " is Full Paid Please Generate Allotment Letter";
            }
            if (t == NotifierMsg.Request_For_Verification_File)
            {
                File_Form File = (File_Form)moduleData[0];
                return "File Number " + File.FileFormNumber + " Block: " + File.Block + " Type: " + File.Type + " has been requested for verification";
            }
            if (t == NotifierMsg.Request_For_Verification_Plot)
            {
                Sp_Get_PlotData_Result Plot = (Sp_Get_PlotData_Result)moduleData[0];
                return "Plot Number " + Plot.Plot_No + " Block: " + Plot.Block_Name + " Type: " + Plot.Type + " has been requested for verification";
            }
            if (t == NotifierMsg.Pending_Purchase_Orders)
            {
                int Pending_PO = (int)moduleData[0];
                return "Total "+ Pending_PO + " are Purchase Orders pending.";
            }


            return string.Empty;
        }
        private static string TransformNotifierUrl(NotifierMsg t, object[] moduleData)
        {
            try
            {
                if (t == NotifierMsg.Ticket || t == NotifierMsg.TicketComment || t == NotifierMsg.TicketForward || t == NotifierMsg.TicketStatusUpdate)
                {
                    Sp_Get_Ticket_Parameter_Result data = (Sp_Get_Ticket_Parameter_Result)moduleData[0];
                    return "/Tickets/TicketNoti?id=" + data.Id.ToString();
                }
                else if (t == NotifierMsg.Unverified)
                {
                    Sp_Get_PlotData_Result data = (Sp_Get_PlotData_Result)moduleData[0];
                    return "/Plots/PlotInformation";
                }
                else if (t == NotifierMsg.Full_Paid_Plots)
                {
                    Sp_Get_PlotData_Result data = (Sp_Get_PlotData_Result)moduleData[0];
                    return "/Plots/NewAllotmentLetters";
                }
                else if (t == NotifierMsg.Demand_Requisitions || t == NotifierMsg.Demand_Approved)
                {
                    Inventory_Demand_Req data = (Inventory_Demand_Req)moduleData[0];
                    return "/ConstructProjectManagement/ProjectsList";
                }
                else if (t == NotifierMsg.Issuance_Request)
                {
                    NameValuestring data = (NameValuestring)moduleData[0];
                    return "/Inventory/Noti?id=" + data.Value + "&tp=" + t.ToString();
                }
                else if (t == NotifierMsg.Request_For_Verification_Plot)
                {
                    Sp_Get_PlotData_Result data = (Sp_Get_PlotData_Result)moduleData[0];
                    return "/Plots/PlotsVerification?Plotid=" + data.Id + "&tp=" + t.ToString();
                }
                else if (t == NotifierMsg.Request_For_Verification_File)
                {
                    File_Form data = (File_Form)moduleData[0];
                    return "/FileSystem/Noti?id=" + data.FileFormNumber + "&tp=" + t.ToString();
                }
                else if (t == NotifierMsg.Pending_Purchase_Orders)
                {
                    //File_Form data = (File_Form)moduleData[0];
                    //return "/Plots/Noti?id=" + data.Value + "&tp=" + t.ToString();
                    return "";
                }

            }
            catch (Exception ex)
            {
                return t.ToString() + "/exp";
            }

            return t.ToString();
        }
        private static string TransformNotifierMailUrl(NotifierMsg t, string notiID, object[] moduleData)
        {
            var reqContext = HttpContext.Current.Request.RequestContext;
            try
            {
                if (t == NotifierMsg.Ticket || t == NotifierMsg.TicketComment || t == NotifierMsg.TicketForward || t == NotifierMsg.TicketStatusUpdate)
                {
                    Sp_Get_Ticket_Parameter_Result data = (Sp_Get_Ticket_Parameter_Result)moduleData[0];
                    return new UrlHelper(reqContext).Action("TicketNoti", "Tickets", new { id = data.Id.ToString(), tp = t.ToString(), noti = notiID }, "http");
                }
                if (t == NotifierMsg.Demand_Approved)
                {
                    Inventory_Demand_Req data = (Inventory_Demand_Req)moduleData[0];
                    return new UrlHelper(reqContext).Action("Noti", "ConstructProjectManagement", new { id = data.Id.ToString(), tp = t.ToString(), noti = notiID }, "http");
                }
            }
            catch (Exception ex)
            {
                return t.ToString() + " exp";
            }

            return t.ToString();
        }
        private static void PostNotifierMail(object[] moduleData, NotifierMsg t, string url, string rec, string Name, long From, long To)
        {
            if (t == NotifierMsg.Ticket)
            {
                Sp_Get_Ticket_Parameter_Result Tick = (Sp_Get_Ticket_Parameter_Result)moduleData[0];
                string[] msgs = new string[4];
                msgs[0] = ConvertToMailText("Ticket ", Tick.Ticket_No);
                msgs[1] = ConvertToMailText("Created By ", Name);
                msgs[2] = ConvertToMailText("Date ", DateTime.Now.ToLongDateString());
                msgs[3] = ConvertToMailText("Title ", Tick.Title);
                postOffice.DispatchMail("Ticket : " + Tick.Ticket_No + " - " + Tick.Title, msgs, url, rec);
            }
            else if (t == NotifierMsg.TicketForward)
            {
                Sp_Get_Ticket_Parameter_Result Tick = (Sp_Get_Ticket_Parameter_Result)moduleData[0];
                string[] msgs = new string[3];
                msgs[0] = ConvertToMailText("Ticket ", Tick.Ticket_No);
                msgs[1] = ConvertToMailText("Forwarded By ", Name);
                msgs[2] = ConvertToMailText("Date ", DateTime.Now.ToLongDateString());
                postOffice.DispatchMail("Ticket : " + Tick.Ticket_No + " - " + Tick.Title, msgs, url, rec);
            }
            else if (t == NotifierMsg.TicketStatusUpdate)
            {

                Sp_Get_Ticket_Parameter_Result Tick = (Sp_Get_Ticket_Parameter_Result)moduleData[0];
                string[] msgs = new string[3];
                msgs[0] = ConvertToMailText("Ticket ", Tick.Ticket_No);
                if (Tick.Status == "Resolved")
                {
                    msgs[1] = ConvertToMailText(Name + " Status Update ", "Resolved the Ticket");
                }
                else if (Tick.Status == "In Progress")
                {
                    msgs[1] = ConvertToMailText(Name + " Status Update ", "Marked Ticket as Inprogress");
                }
                else if (Tick.Status == "Pending")
                {
                    msgs[1] = ConvertToMailText(Name + " Status Update ", "Reopen the Ticket");
                }
                msgs[2] = ConvertToMailText("Date ", DateTime.Now.ToLongDateString());
                postOffice.DispatchMail("Ticket : " + Tick.Ticket_No + " - " + Tick.Title, msgs, url, rec);
            }
            else if (t == NotifierMsg.Unverified)
            {
                Sp_Get_PlotData_Result plot = (Sp_Get_PlotData_Result)moduleData[0];
                string[] msgs = new string[3];
                msgs[0] = ConvertToMailText("Plot Unverified", plot.Plot_No + "-" + plot.Type + "-" + plot.Block_Name);
                msgs[1] = ConvertToMailText("Date ", DateTime.Now.ToLongDateString());
                postOffice.DispatchMail("Plot unverification : " + plot.Plot_No + "-" + plot.Type + "-" + plot.Block_Name, msgs, url, rec);
            }
            else if (t == NotifierMsg.Full_Paid_Plots)
            {
                Sp_Get_PlotData_Result plot = (Sp_Get_PlotData_Result)moduleData[0];
                string[] msgs = new string[3];
                msgs[0] = ConvertToMailText("Plot Full Paid", plot.Plot_No + "-" + plot.Type + "-" + plot.Block_Name);
                msgs[1] = ConvertToMailText("Date ", DateTime.Now.ToLongDateString());
                postOffice.DispatchMail("Plot Full Paid : " + plot.Plot_No + "-" + plot.Type + "-" + plot.Block_Name, msgs, url, rec);
            }
            else if (t == NotifierMsg.Request_For_Verification_File)
            {
                File_Form plot = (File_Form)moduleData[0];
                string[] msgs = new string[3];
                msgs[0] = ConvertToMailText("File Verification Request", plot.FileFormNumber + "-" + plot.Type + "-" + plot.Block);
                msgs[1] = ConvertToMailText("Date ", DateTime.Now.ToLongDateString());
                postOffice.DispatchMail("File Verification Request : " + plot.FileFormNumber + "-" + plot.Type + "-" + plot.Block, msgs, url, rec);
            }
        }
        private static string ConvertToMailText(string infoType, string infoText)
        {
            return "<b> " + infoType + " : </b> " + infoText;
        }
        public static void ReadNotification(long id)
        {
            using (Grand_CityEntities db = new Grand_CityEntities())
            {
                db.Sp_Update_Notification(id);
            }
        }
        private static string GetRefreshWidget(NotifierMsg type)
        {
            switch (type)
            {
                default:
                    return string.Empty;
            }
        }
        private static string GetRefreshUrl(NotifierMsg type)
        {
            switch (type)
            {
                default:
                    return string.Empty;
            }

        }
        private static Sp_Get_UserIdEmail_Result GetUserBasicInfo(long id)
        {
            long[] Tagusers = new long[1];
            Tagusers[0] = id;
            Sp_Get_UserIdEmail_Result res;

            var Uidxml = new XElement("Users", Tagusers.Select(x => new XElement("UserData", new XAttribute("UserId", x)))).ToString();
            using (Grand_CityEntities db = new Grand_CityEntities())
            {
                res = db.Sp_Get_UserIdEmail(Uidxml).FirstOrDefault();
            }

            return res;
        }
    }

   
}
