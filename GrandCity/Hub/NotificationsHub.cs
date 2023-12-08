using MeherEstateDevelopers.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeherEstateDevelopers
{
    public class NotificationHub : Hub
    {
        public static List<string> ConnectedUsers;
        //public override Task OnConnected()
        //{
        //    string __tempUser = Context.User.Identity.GetUserId<long>().ToString();
        //    if (!ConnectedUsers.Any(x => x == __tempUser))
        //    {
        //        ConnectedUsers.Add(__tempUser);
        //    }
        //    using (var db = new Grand_CityEntities())
        //    {
        //        long t = Context.User.Identity.GetUserId<long>();
        //        //var connId = long.Parse(Context.ConnectionId);
        //        var res = db.Sp_Get_UserRoleConfirmation("KioskScreen", (t)).FirstOrDefault();
        //        if (res == true)
        //        {
        //            var kiosks = db.Sp_Get_UsersByRole("KioskScreen").ToList();
        //            NotifyAllKiosks(kiosks.Select(x => x.UserId).ToList());
        //        }
        //    }
        //    return base.OnConnected();
        //}
        //public override Task OnReconnected()
        //{
        //    ConnectedUsers.Add(Context.ConnectionId);
        //    return base.OnReconnected();
        //}
        //public override Task OnDisconnected(bool stopCalled)
        //{
        //    string __tempUser = Context.User.Identity.GetUserId<long>().ToString();
        //    if (ConnectedUsers.Any(x => x == __tempUser))
        //    {
        //        ConnectedUsers.Remove(__tempUser);
        //    }
        //    using (var db = new Grand_CityEntities())
        //    {
        //        long t = Context.User.Identity.GetUserId<long>();
        //        //var connId = long.Parse(Context.ConnectionId);
        //        var res = db.Sp_Get_UserRoleConfirmation("KioskScreen", (t)).FirstOrDefault();
        //        if (res == true)
        //        {
        //            var kiosks = db.Sp_Get_UsersByRole("KioskScreen").ToList();
        //            NotifyAllKiosks(kiosks.Select(x => x.UserId).ToList());
        //        }
        //    }
        //    //ConnectedUsers.Remove(Context.ConnectionId);
        //    return base.OnDisconnected(stopCalled);
        //}
        public static void NotifyAllKiosks(List<long> users)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            List<string> converted = users.Select(x => x.ToString()).ToList();
            var data = new
            {
                Message = "_MIS_DEFAULT_ORDER_REFRESH_KIOSK_",
            };
            context.Clients.Users(converted).kioskRefresh(data);
        }

        public static void notifyUser(UserNotifierData details)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            var data = new
            {
                Message = details.notificationMsg,
                FromUser = details.fromUserId,
                creationTime = details.createdAt.ToString(),
                hit = details.hitUrl,
                id = details.notID,
                refreshPath = details.refreshUrl,
                refreshWidg = details.refreshWidget
            };
            context.Clients.User(details.toUserId.ToString()).sendNoti(data);
        }

        //public static void NotifyAllKiosks(List<long>users)
        //{
        //    IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
        //    //List<string> converted = users.Select(x => x.ToString()).ToList();
        //    var data = new
        //    {
        //        Message = "_MIS_DEFAULT_ORDER_REFRESH_KIOSK_",
        //    };
        //    foreach(var v in ConnectedUsers)
        //    {
        //        context.Clients.Client(v.Identifier).kioskRefresh(data);
        //    }
        //    //context.Clients..kioskRefresh(data);
        //}

        public static void NotifyTest(string infor)
        {
            IHubContext conte = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            conte.Clients.User("20059").NotifierTest(infor);
        }

        //public static void NotifyAllUsersInRole(string rolename, NotificationPackedInfo info, List<string> users)
        //{
        //    foreach (var u in users)
        //    {
        //        IHubContext conte = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
        //        conte.Clients.User(u).MISNotifier(info);
        //    }
        //}
    }
}